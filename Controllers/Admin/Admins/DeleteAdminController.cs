
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VisitorLog.Server.Attributes;

namespace VisitorLog.Server.Controllers.Admin
{


	public partial class AdminController : ControllerBase
  {


    [AdminAuth]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Add(int id)
    {
      try
      {

        var exists = await mDbContext.Admins.FindAsync(id);

        if (exists == null)
        {
          return BadRequest(new { Status = 400, Message = "Invalid admin" });
        }

        mDbContext.Admins.Remove(exists);
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