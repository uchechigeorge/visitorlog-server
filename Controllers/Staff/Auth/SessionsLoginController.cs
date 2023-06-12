using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VisitorLog.Server.Attributes;
using VisitorLog.Server.Services;
using VisitorLog.Server.Utils;

namespace VisitorLog.Server.Controllers.Staff
{
	public partial class AuthController : ControllerBase
	{
		// GET: api/staff/auth/sessions-login
		[StaffAuth]
		[HttpGet("sessions-login")]
		public async Task<ActionResult> SessionLogin([FromQuery]string refresh)
		{
			try
			{
				HttpContext.Items.TryGetValue(StaffAuthContextItems.Staff, out dynamic _authenticatedStaff);
				var authenticatedStaff = (Models.Staff)_authenticatedStaff;

				string token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];

				if (refresh.IsTruthy())
				{
					token = authenticatedStaff.GenerateAuthToken();
				}

				return Ok(new
				{
					Status = 200,
					Message = "Ok",
					Data = new
					{
						Token = token,
						Credentials = await mDbContext.Staff.GetStaffCredentials(authenticatedStaff.Id, mDbContext),
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
