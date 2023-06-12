using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace VisitorLog.Server.Utils
{
  public static class Security
  {

		#region Hash password

		/// <summary>
		/// Extension method to generate hashed password with SHA512 and salt values from Config settings
		/// </summary>
		/// <param name="password">Password to hash</param>
		/// <returns></returns>
		public static string HashPassword(this string password)
		{
			var sha512 = new SHA512CryptoServiceProvider();
			var utf8 = new UTF8Encoding();
			var sb = new StringBuilder();

			string startSalt = IoCContainer.Configuration["Security:StartSalt"] ?? "";
			string endSalt = IoCContainer.Configuration["Security:EndSalt"] ?? "";
			foreach (var b in sha512.ComputeHash(utf8.GetBytes(startSalt + password + endSalt)))
				sb.Append(b.ToString("X2"));

			return sb.ToString();
		}
		
		#endregion

		#region Generate token

		/// <summary>
		/// Generates new token
		/// </summary>
		/// <returns></returns>
		public static string GenerateToken(this string value)
		{
			var random = new Random();
			int num = random.Next(1000000000, 2000000000);

			var sha512 = new SHA512CryptoServiceProvider();
			var utf8 = new UTF8Encoding();
			var sb = new StringBuilder();

			foreach (var b in sha512.ComputeHash(utf8.GetBytes(num + "")))
				sb.Append(b.ToString("X2"));

			return sb.ToString();
		}
		
		/// <summary>
		/// Generates new token
		/// </summary>
		/// <returns></returns>
		public static string GenerateToken()
		{
			var random = new Random();
			int num = random.Next(1000000000, 2000000000);

			var sha512 = new SHA512CryptoServiceProvider();
			var utf8 = new UTF8Encoding();
			var sb = new StringBuilder();

			foreach (var b in sha512.ComputeHash(utf8.GetBytes(num + "")))
				sb.Append(b.ToString("X2"));

			return sb.ToString();
		}

		#endregion

		#region Generate string

		/// <summary>
		/// Generates random string
		/// </summary>
		/// <returns>The string generated</returns>
		public static string GenerateRandomString(int length = 10, GenerateStringTypes type = GenerateStringTypes.AlphaNumeric, bool caseSensitive = true, bool includeSymbols = false)
		{
			string upperCaseLetters = "QWERTYUIOPASDFGHJKLZXCVBNM";
			string lowerCaseLetters = "qwertyuiopasdfghjklzxcvbnm";
			string numbers = "0123456789";
			string symbols = "!@#$%*,/-=+_;:~";

			string chars = "";

			switch (type)
			{
				case GenerateStringTypes.Alpha:
					chars = upperCaseLetters;
					if (caseSensitive)
					{
						chars += lowerCaseLetters;
					}
					if (includeSymbols)
					{
						chars += symbols;
					}
					break;
				case GenerateStringTypes.AlphaNumeric:
					chars = upperCaseLetters + numbers;
					if (caseSensitive)
					{
						chars += lowerCaseLetters;
					}
					if (includeSymbols)
					{
						chars += symbols;
					}
					break;
				case GenerateStringTypes.Numeric:
					chars = numbers;
					break;
				default:
					break;
			}

			var randomString = new StringBuilder();
			var random = new Random();
			for (int i = 0; i <= length; i++)
			{
				int randNum = random.Next(0, chars.Length - 1);
				randomString.Append(chars[randNum]);
			}

			return randomString.ToString();
		}

		#endregion

		#region Encrypt

		/// <summary>
		/// Encrypts string
		/// </summary>
		/// <param name="plainText">Text to encrypt</param>
		/// <returns></returns>
		public static string EncryptText(this string plainText)
		{
			string cryptKey = IoCContainer.Configuration["Security:CryptKey"];

			byte[] iv = new byte[16];
			byte[] array;

			using (Aes aes = Aes.Create())
			{
				aes.Key = Encoding.UTF8.GetBytes(cryptKey);
				aes.IV = iv;

				ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
						{
							streamWriter.Write(plainText);
						}

						array = memoryStream.ToArray();
					}
				}
			}

			return BitConverter.ToString(array).Replace("-", "");

		}

		/// <summary>
		/// Encrypts string
		/// </summary>
		/// <param name="plainText">Text to encrypt</param>
		/// <returns></returns>
		public static byte[] EncryptTextToArray(this string plainText)
		{
			string cryptKey = IoCContainer.Configuration["Security:CryptKey"];

			byte[] iv = new byte[16];
			byte[] array;

			using (Aes aes = Aes.Create())
			{
				aes.Key = Encoding.UTF8.GetBytes(cryptKey);
				aes.IV = iv;

				ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
						{
							streamWriter.Write(plainText);
						}

						array = memoryStream.ToArray();
					}
				}
			}

			return array;

		}

		/// <summary>
		/// Decrypts string
		/// </summary>
		/// <param name="cipherText">Text to decrypt</param>
		/// <returns></returns>
		public static string DecryptText(this string cipherText)
		{
			string cryptKey = IoCContainer.Configuration["Security:CryptKey"];

			byte[] iv = new byte[16];
			//byte[] Convert.FromBase64String(cipherText);
			byte[] buffer = StringToByteArray(cipherText);

			using (Aes aes = Aes.Create())
			{
				aes.Key = Encoding.UTF8.GetBytes(cryptKey);
				aes.IV = iv;
				ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader streamReader = new StreamReader(cryptoStream))
						{
							return streamReader.ReadToEnd();
						}
					}
				}
			}

		}

		private static byte[] StringToByteArray(this string hex)
		{
			return Enumerable.Range(0, hex.Length)
											 .Where(x => x % 2 == 0)
											 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
											 .ToArray();
		}

		#endregion

		#region Helpers

		public enum GenerateStringTypes
		{
			AlphaNumeric,
			Alpha,
			Numeric,
		}

		#endregion

	}

}