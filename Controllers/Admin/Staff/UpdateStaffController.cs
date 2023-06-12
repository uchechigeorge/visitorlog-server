using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using VisitorLog.Server.Attributes;

namespace VisitorLog.Server.Controllers.Admin
{
	public partial class StaffController : ControllerBase
	{
		// PATCH: api/admin/staff
		[AdminAuth]
		[HttpPatch("{id}")]
		public async Task<ActionResult> Update(int id, UpdateStaffBody body)
		{
			try
			{
				var exists = await mDbContext.Staff.FindAsync(id);
				if (exists == null)
				{
					return BadRequest(new { Status = 400, Message = "Invalid staff" });
				}

				var emailExists = await mDbContext.Staff.AnyAsync(s => s.Email == body.Email && s.Id != id);
				if (emailExists)
				{
					return BadRequest(new { Status = 400, Message = "Duplicate emails" });
				}

				exists.Address = body.Address;
				exists.Dob = body.Dob;
				exists.Email = body.Email;
				exists.FullName = body.FullName;
				exists.Gender = body.Gender;
				exists.PhoneNumber = body.PhoneNumber;
				exists.Position = body.Position;
				exists.Degree = body.Degree;
				exists.DateModified = DateTime.UtcNow;

				await mDbContext.SaveChangesAsync();

				await mDbContext.SaveChangesAsync();

				return Ok(new 
				{ 
					Status = 200, 
					Message = "Ok",
				});

			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Status = 500, Message = ex.ToString() });
			}
		}


		public class UpdateStaffBody
		{
			[Required]
			public string FullName { get; set; }
			public string Email { get; set; }
			public string Address { get; set; }
			public string Position { get; set; }
			public string PhoneNumber { get; set; }
			public string Gender { get; set; }
			public string Degree { get; set; }
			public DateTime? Dob { get; set; }
		}

	}
}
