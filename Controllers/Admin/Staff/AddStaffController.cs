using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using VisitorLog.Server.Attributes;
using VisitorLog.Server.Utils;

namespace VisitorLog.Server.Controllers.Admin
{
	public partial class StaffController : ControllerBase
	{
		// POST: api/admin/staff
		[AdminAuth]
		[HttpPost]
		public async Task<ActionResult> Add(AddStaffBody body)
		{
			try
			{
				
				if (string.IsNullOrWhiteSpace(body.Password))
				{
					body.Password = Security.GenerateRandomString();
				}

				var emailExists = await mDbContext.Staff.AnyAsync(s => s.Email == body.Email);
				if (emailExists)
				{
					return BadRequest(new { Status = 400, Message = "Duplicate emails" });
				}

				var staff = new Models.Staff
				{
					Address = body.Address,
					Dob = body.Dob,
					Email = body.Email,
					FullName = body.FullName,
					Gender = body.Gender,
					Password = body.Password.HashPassword(),
					PhoneNumber = body.PhoneNumber,
					Position = body.Position,
					Degree = body.Degree,
				};

				mDbContext.Staff.Add(staff);
				await mDbContext.SaveChangesAsync();

				return Ok(new 
				{ 
					Status = 200, 
					Message = "Ok",
					Data = new
					{
						body.Password,
					}
				});

			}
			catch (Exception ex)
			{
				return StatusCode(500, new { Status = 500, Message = ex.ToString() });
			}
		}


		public class AddStaffBody
		{
			[Required]
			public string FullName { get; set; }
			[Required]
			public string Email { get; set; }
			public string Password { get; set; }
			public string Address { get; set; }
			public string Position { get; set; }
			public string PhoneNumber { get; set; }
			public string Gender { get; set; }
			public string Degree { get; set; }
			public DateTime? Dob { get; set; }
		}

	}
}
