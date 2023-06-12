using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using VisitorLog.Server.Attributes;

namespace VisitorLog.Server.Controllers.Admin
{
	public partial class VisitorController : ControllerBase
	{
		// PATCH: api/admin/visitors
		[AdminAuth]
		[HttpPatch("{id}")]
		public async Task<ActionResult> Update(int id, UpdateVisitorBody body)
		{
			try
			{
				var exists = await mDbContext.Visitors.FindAsync(id);
				if (exists == null)
				{
					return BadRequest(new { Status = 400, Message = "Invalid visitor" });
				}

				exists.Name = body.Name;
				exists.Email = body.Email;
				exists.Gender = body.Gender;
				exists.PhoneNumber = body.PhoneNumber;
				exists.Purpose = body.Purpose;
				exists.VDate = body.VDate;
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


		public class UpdateVisitorBody
		{
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
