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
	public class StaffAuthAttribute : Attribute, IAsyncActionFilter
	{

		#region Properties

		private readonly bool mIgnoreError;

		#endregion

		#region Contructor

		public StaffAuthAttribute()
		{

		}

		public StaffAuthAttribute(bool ignoreError)
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

				context.HttpContext.Items.Add(StaffAuthContextItems.Authorized, false);
				context.HttpContext.Items.Add(StaffAuthContextItems.AuthMessage, authMessage);
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

				context.HttpContext.Items.TryAdd(StaffAuthContextItems.Authorized, false);
				context.HttpContext.Items.TryAdd(StaffAuthContextItems.AuthMessage, authMessage);
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

				context.HttpContext.Items.TryAdd(StaffAuthContextItems.Authorized, false);
				context.HttpContext.Items.TryAdd(StaffAuthContextItems.AuthMessage, authMessage);
				await next();
				return;
			}

			var claims = jwtToken.GetTokenClaims().ToList();

			var dbContext = context.HttpContext.RequestServices.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;

			var staff = await dbContext.Staff.FindAsync(claims.FirstOrDefault(c => c.Type.Equals("staffId")).Value.GetInt());
			if(staff == null)
			{
				authMessage = "Invalid staff";
				if (!mIgnoreError)
				{
					context.Result = GetResult(authMessage);
					return;
				}

				context.HttpContext.Items.TryAdd(StaffAuthContextItems.Authorized, false);
				context.HttpContext.Items.TryAdd(StaffAuthContextItems.AuthMessage, authMessage);
				await next();
				return;
			}
			
		
			context.HttpContext.Items.TryAdd(StaffAuthContextItems.Authorized, true);
			context.HttpContext.Items.TryAdd(StaffAuthContextItems.Staff, staff);

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
	public readonly struct StaffAuthContextItems
	{
		public static string Authorized { get; } = "Authorized";
		public static string AuthMessage { get; } = "AuthMessage";
		public static string Staff { get; } = "Staff";
	}

	#endregion

}
