using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VisitorLog.Server.Services;
using VisitorLog.Server.Utils;

namespace VisitorLog.Server.Controllers.Admin
{
	public partial class AuthController : ControllerBase
	{
		// GET: api/<GetVisitorsController>
		[HttpPost("login")]
		public async Task<ActionResult> Login(LoginBody body)
		{
			try
			{
				var exists = await mDbContext.Admins.Where(a => a.Username == body.Username && a.Password == body.Password.HashPassword()).FirstOrDefaultAsync();

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
						credentials = await mDbContext.Admins.GetAdminCredentials(exists.Id, mDbContext),
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
			public string Username { get; set; }

			[Required]
			public string Password { get; set; }
		}

	}
}
