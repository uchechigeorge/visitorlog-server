using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VisitorLog.Server.Attributes;
using VisitorLog.Server.Services;
using VisitorLog.Server.Utils;

namespace VisitorLog.Server.Controllers.Admin
{
	public partial class AuthController : ControllerBase
	{
		// GET: api/admin/auth/reset-password
		[AdminAuth]
		[HttpPost("reset-password")]
		public async Task<ActionResult> ResetPassword(ResetPasswordBody body)
		{
			try
			{
				HttpContext.Items.TryGetValue(AdminAuthContextItems.Admin, out dynamic _authenticatedAdmin);
				var authenticatedAdmin = (Models.Admin)_authenticatedAdmin;

				var adminId = authenticatedAdmin.Id;
				var adminPasswordCheck = await mDbContext.Admins.Where(u => u.Id == adminId && u.Password == body.OldPassword.HashPassword()).FirstOrDefaultAsync();

				if (adminPasswordCheck == null)
				{
					return BadRequest(new { Status = 400, Message = "Invalid password" });
				}

				mDbContext.Entry(adminPasswordCheck).State = EntityState.Detached;

				var admin = new Models.Admin
				{
					Id = adminId,
					Password = body.NewPassword.HashPassword(),
					DateModified = DateTime.UtcNow,
				};
				mDbContext.Admins.Attach(admin);
				mDbContext.Entry(admin).Property(p => p.Password).IsModified = true;

				await mDbContext.SaveChangesAsync();
				mDbContext.Entry(admin).State = EntityState.Detached;

				return Ok(new
				{
					Status = 200,
					Message = "Ok",
					Data = new
					{
						Credentials = await mDbContext.Admins.GetAdminCredentials(authenticatedAdmin.Id, mDbContext),
					}
				});

			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Status = 500, Message = ex.ToString() });
			}
		}

		public class ResetPasswordBody
		{
			[Required]
			public string OldPassword { get; set; }
			[Required]
			public string NewPassword { get; set; }
		}

	}
}
