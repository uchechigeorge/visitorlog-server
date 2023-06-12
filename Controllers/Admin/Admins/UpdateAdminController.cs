
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VisitorLog.Server.Attributes;

namespace VisitorLog.Server.Controllers.Admin
{


	public partial class AdminController : ControllerBase
  {

    [AdminAuth]
    [HttpPatch("{id}")]
    public async Task<ActionResult> Update(int id, UpdateAdminBody body)
    {
      try
      {
        var exists = await mDbContext.Admins.FindAsync(id);

        if (exists == null)
        {
          return BadRequest(new { Status = 400, Message = "Invalid admin" });
        }

        exists.AccessId = body.Access ?? Models.AdminAccess.Full;
        exists.DateModified = DateTime.UtcNow;
        
        mDbContext.Entry(exists).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        await mDbContext.SaveChangesAsync();

        return Ok(new
        {
          Status = 200,
          Message = "Ok"
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { Status = 500, ex.Message });
      }
    }


    public class UpdateAdminBody
    {
      public Models.AdminAccess? Access { get; set; }
		}

  }

}