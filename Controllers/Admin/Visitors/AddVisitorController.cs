using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using VisitorLog.Server.Attributes;

namespace VisitorLog.Server.Controllers.Admin
{
	public partial class VisitorController : ControllerBase
	{
		// POST: api/admin/visitor
		[AdminAuth]
		[HttpPost]
		public async Task<ActionResult> Add(AddStaffBody body)
		{
			try
			{

				var staffExists = await mDbContext.Staff.FindAsync(body.StaffId);
				if (staffExists == null)
				{
					return BadRequest(new { Status = 400, Message = "Invalid staff" });
				}

				var visitor = new Models.Visitor
				{
					Email = body.Email,
					Gender = body.Gender,
					Name = body.Name,
					PhoneNumber = body.PhoneNumber,
					Purpose = body.Purpose,
					StaffId = body.StaffId,
					VDate = body.VDate ?? DateTime.UtcNow,
				};

				mDbContext.Visitors.Add(visitor);
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


		public class AddStaffBody
		{
			public int StaffId { get; set; }
			[Required]
			public string Name { get; set; }
			public DateTime? VDate { get; set; }
			public string Purpose { get; set; }
			public string Email { get; set; }
			public string PhoneNumber { get; set; }
			public string Gender { get; set; }
		}

	}
}
