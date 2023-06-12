
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VisitorLog.Server.Attributes;

namespace VisitorLog.Server.Controllers.Admin
{


	public partial class StaffController : ControllerBase
  {

    [AdminAuth]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
      try
      {

        var exists = await mDbContext.Staff.FindAsync(id);

        if (exists == null)
        {
          return BadRequest(new { Status = 400, Message = "Invalid staff" });
        }

        mDbContext.Staff.Remove(exists);
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

  

  }

}