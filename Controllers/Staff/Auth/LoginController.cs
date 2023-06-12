using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VisitorLog.Server.Services;
using VisitorLog.Server.Utils;

namespace VisitorLog.Server.Controllers.Staff
{
	public partial class AuthController : ControllerBase
	{
		// POST: api/staff/auth/login
		[HttpPost("login")]
		public async Task<ActionResult> Login(LoginBody body)
		{
			try
			{
				var exists = await mDbContext.Staff.Where(a => a.Email == body.Email && a.Password == body.Password.HashPassword()).FirstOrDefaultAsync();

				if (exists == null)
				{
					return BadRequest(new { Status = 400, Message = "Invalid credentials" });	
				}

				var token = exists.GenerateAuthToken();

				return Ok(new
				{
					Status = 200,
					Message = "Ok",
					Data = new
					{
						token,
						credentials = await mDbContext.Staff.GetStaffCredentials(exists.Id, mDbContext),
					}
				});

			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Status = 500, ex.Message });
			}
		}

		public class LoginBody
		{
			[Required]
			public string Email { get; set; }

			[Required]
			public string Password { get; set; }
		}

	}
}
