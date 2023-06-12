using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VisitorLog.Server.Attributes;
using VisitorLog.Server.Services;
using VisitorLog.Server.Utils;

namespace VisitorLog.Server.Controllers.Admin
{
	public partial class AuthController : ControllerBase
	{
		// GET: api/admin/auth/sessions-login
		[AdminAuth]
		[HttpGet("sessions-login")]
		public async Task<ActionResult> SessionLogin([FromQuery]string refresh)
		{
			try
			{
				HttpContext.Items.TryGetValue(AdminAuthContextItems.Admin, out dynamic _authenticatedAdmin);
				var authenticatedAdmin = (Models.Admin)_authenticatedAdmin;

				string token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];

				if (refresh.IsTruthy())
				{
					token = authenticatedAdmin.GenerateAuthToken();
				}

				return Ok(new
				{
					Status = 200,
					Message = "Ok",
					Data = new
					{
						Token = token,
						Credentials = await mDbContext.Admins.GetAdminCredentials(authenticatedAdmin.Id, mDbContext),
					}
				});

			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Status = 500, Message = ex.ToString() });
			}
		}

	}
}
