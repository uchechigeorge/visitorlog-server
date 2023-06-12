
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace VisitorLog.Server.Utils
{
  
  public static class JwtExtensions
  {

    #region Public Methods

    /// <summary>
    /// Validates whether a given token is valid or not, and returns true in case the token is valid otherwise it will return false;
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static bool IsJwtTokenValid(this string token)
    {
      if (string.IsNullOrEmpty(token))
        throw new ArgumentException("Given token is null or empty.");

      var tokenValidationParameters = GetTokenValidationParameters();

      var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
      try
      {
        var tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// Generates token with claims.
    /// </summary>
    /// <param name="claims"></param>
    /// <returns>Generated token.</returns>
    public static string GenerateToken(this Claim[] claims, int? minutesExpiration = null)
    {
      if (claims == null || claims.Length == 0)
        throw new ArgumentException("Arguments to create token are not valid.");

      SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddMinutes(minutesExpiration ?? Convert.ToInt32(IoCContainer.Configuration["Jwt:MinutesExpiration"])),
        SigningCredentials = new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
      };

      JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
      SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
      string token = jwtSecurityTokenHandler.WriteToken(securityToken);

      return token;
    }

		/// <summary>
		/// Receives the claims of token by given token as string.
		/// </summary>
		/// <remarks>
		/// Pay attention, once the token is FAKE the method will throw an exception.
		/// </remarks>
		/// <param name="token"></param>
		/// <returns>IEnumerable of claims for the given token.</returns>
		public static IEnumerable<Claim> GetTokenClaims(this string token)
    {
      if (string.IsNullOrEmpty(token))
        throw new ArgumentException("Given token is null or empty.");

      TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();

      JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
      try
      {
        ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
        return tokenValid.Claims;
      }
      catch (Exception)
      {
        throw new SecurityTokenDecryptionFailedException("Invalid token");
      }
    }

    #endregion

    #region Private Methods

    private static SecurityKey GetSymmetricSecurityKey()
    {
      byte[] symmetricKey = Convert.FromBase64String(IoCContainer.Configuration["Jwt:SecretKey"]);
      return new SymmetricSecurityKey(symmetricKey);
    }

    private static TokenValidationParameters GetTokenValidationParameters()
    {
      return new TokenValidationParameters()
      {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidIssuer = IoCContainer.Configuration["Jwt:Issuer"],
        ValidAudience = IoCContainer.Configuration["Jwt:Audience"],
        IssuerSigningKey = GetSymmetricSecurityKey()
      };
    }

    #endregion
    
  }
}