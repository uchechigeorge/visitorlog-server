
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VisitorLog.Server.Attributes;
using VisitorLog.Server.Utils;

namespace VisitorLog.Server.Controllers.Admin
{


  public partial class AdminController : ControllerBase
  {

    #region Methods

    [AdminAuth(ignoreError: true)]
    [HttpPost]
    public async Task<ActionResult> Add(AddAdminBody body)
    {
      try
      {
        HttpContext.Items.TryGetValue(AdminAuthContextItems.AuthMessage, out object authMessage);
        HttpContext.Items.TryGetValue(AdminAuthContextItems.Authorized, out object authorized);

        var adminKey = HttpContext.Request.Headers["x-admin-key"];

        var config = HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;

        bool adminKeyActive = bool.TryParse(config["Security:AdminKeyActive"], out bool _) && bool.Parse(config["Security:AdminKeyActive"]);
        bool hasAdminKey = (adminKey == config["Security:AdminKey"]) && adminKeyActive;

        if (!hasAdminKey && !(bool)authorized)
        {
          return BadRequest(new { Status = 400, Message = authMessage });
        }

        var existingAdmin = await mDbContext.Admins.Where(u => u.Username == body.Username).FirstOrDefaultAsync();

        if (existingAdmin != null)
        {
          return BadRequest(new { Status = 401, Message = "Duplicate username" });
        }

        var insertedAdmin = new Models.Admin
        {
          Username = body.Username,
          Password = body.Password.HashPassword(),
          Name = body.Name,
          AccessId = body.Access ?? Models.AdminAccess.Full,
        };

        mDbContext.Admins.Add(insertedAdmin);
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

    #endregion

    #region Helpers

    public class AddAdminBody
    {
      [Required]
      public string Username { get; set; }
      [Required]
      public string Password { get; set; }
      public string Name { get; set; }
      public Models.AdminAccess? Access { get; set; }
    }

    #endregion

  }

}