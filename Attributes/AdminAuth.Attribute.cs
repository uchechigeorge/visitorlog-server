using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisitorLog.Server.Models;
using VisitorLog.Server.Utils;

namespace VisitorLog.Server.Attributes
{
	public class AdminAuthAttribute : Attribute, IAsyncActionFilter
	{

		#region Properties

		private readonly bool mIgnoreError;

		#endregion

		#region Contructor

		public AdminAuthAttribute()
		{

		}

		public AdminAuthAttribute(bool ignoreError = false, bool isMaster = false)
		{
			mIgnoreError = ignoreError;
		}

		#endregion

		#region Methods

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			string authMessage = string.Empty;

			if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var tokenVal))
			{
				authMessage = "No token";
				if(!mIgnoreError)
				{
					context.Result = GetResult(authMessage);
					return;
				}

				context.HttpContext.Items.Add(AdminAuthContextItems.Authorized, false);
				context.HttpContext.Items.Add(AdminAuthContextItems.AuthMessage, authMessage);
				await next();
				return;
			}

			string token = tokenVal.ToString();

			if (token.Split(' ').Length != 2 || string.IsNullOrWhiteSpace(token.Split(' ')[1]))
			{
				authMessage = "No token";
				if (!mIgnoreError)
				{
					context.Result = GetResult(authMessage);
					return;
				}

				context.HttpContext.Items.TryAdd(AdminAuthContextItems.Authorized, false);
				context.HttpContext.Items.TryAdd(AdminAuthContextItems.AuthMessage, authMessage);
				await next();
				return;
			}

			var jwtToken = token.Split(' ')[1];

			if (!jwtToken.IsJwtTokenValid())
			{
				authMessage = "Invalid token";
				if (!mIgnoreError)
				{
					context.Result = GetResult(authMessage);
					return;
				}

				context.HttpContext.Items.TryAdd(AdminAuthContextItems.Authorized, false);
				context.HttpContext.Items.TryAdd(AdminAuthContextItems.AuthMessage, authMessage);
				await next();
				return;
			}

			//var claims = JwtHandler.GetTokenClaims(jwtToken).ToList();
			var claims = jwtToken.GetTokenClaims().ToList();

			var dbContext = context.HttpContext.RequestServices.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;

			var admin = await dbContext.Admins.FindAsync(claims.FirstOrDefault(c => c.Type.Equals("adminId")).Value.GetInt());

			if(admin == null)
			{
				authMessage = "Invalid admin";
				if (!mIgnoreError)
				{
					context.Result = GetResult(authMessage);
					return;
				}

				context.HttpContext.Items.TryAdd(AdminAuthContextItems.Authorized, false);
				context.HttpContext.Items.TryAdd(AdminAuthContextItems.AuthMessage, authMessage);
				await next();
				return;
			}
			
			context.HttpContext.Items.TryAdd(AdminAuthContextItems.Authorized, true);
			context.HttpContext.Items.TryAdd(AdminAuthContextItems.Admin, admin);

			await next();
		}

		private JsonResult GetResult(string message, int status = StatusCodes.Status401Unauthorized)
		{
			return new JsonResult(new { status, message }) { StatusCode = status };
		}

		#endregion

	}

	#region Helpers

	/// <summary>
	/// List of option keys that are used to store items in Http context
	/// </summary>
	public readonly struct AdminAuthContextItems
	{
		public static string Authorized { get; } = "Authorized";
		public static string AuthMessage { get; } = "AuthMessage";
		public static string Admin { get; } = "Admin";
	}

	#endregion

}
