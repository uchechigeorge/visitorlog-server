using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VisitorLog.Server.Attributes;
using VisitorLog.Server.Services;
using VisitorLog.Server.Utils;

namespace VisitorLog.Server.Controllers.Staff
{
	public partial class AuthController : ControllerBase
	{
		// POST: api/staff/auth/reset-password
		[StaffAuth]
		[HttpPost("reset-password")]
		public async Task<ActionResult> ResetPassword(ResetPasswordBody body)
		{
			try
			{
				HttpContext.Items.TryGetValue(StaffAuthContextItems.Staff, out dynamic _authenticatedStaff);
				var authenticatedStaff = (Models.Staff)_authenticatedStaff;

				var staffId = authenticatedStaff.Id;
				var passwordCheck = await mDbContext.Staff.Where(u => u.Id == staffId && u.Password == body.OldPassword.HashPassword()).FirstOrDefaultAsync();

				if (passwordCheck == null)
				{
					return BadRequest(new { Status = 400, Message = "Invalid password" });
				}

				mDbContext.Entry(passwordCheck).State = EntityState.Detached;

				var staff = new Models.Staff
				{
					Id = staffId,
					Password = body.NewPassword.HashPassword(),
					DateModified = DateTime.UtcNow,
				};
				mDbContext.Staff.Attach(staff);
				mDbContext.Entry(staff).Property(p => p.Password).IsModified = true;
				mDbContext.Entry(staff).Property(p => p.DateModified).IsModified = true;

				await mDbContext.SaveChangesAsync();
				mDbContext.Entry(staff).State = EntityState.Detached;

				return Ok(new
				{
					Status = 200,
					Message = "Ok",
					Data = new
					{
						Credentials = await mDbContext.Staff.GetStaffCredentials(authenticatedStaff.Id, mDbContext),
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
