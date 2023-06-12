
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisitorLog.Server.Attributes;
using VisitorLog.Server.Services;
using VisitorLog.Server.Utils;

namespace VisitorLog.Server.Controllers.Admin
{

  public partial class StaffController : ControllerBase
  {

    #region Methods

    [HttpGet]
    [AdminAuth]
    public async Task<ActionResult> Get()
    {
      try
      {

        int page = HttpContext.Request.Query["page"].ToString().GetInt();
        int pageSize = HttpContext.Request.Query["pagesize"].ToString().GetInt(50);
        string sort = HttpContext.Request.Query["sort"];
        string order = HttpContext.Request.Query["order"];
        IEnumerable<string> searchString = HttpContext.Request.Query["searchstring"];
        IEnumerable<string> searchColumns = HttpContext.Request.Query["searchcolumn"];
        IEnumerable<string> searchStack = HttpContext.Request.Query["searchstack"];
        IEnumerable<string> searchOperator = HttpContext.Request.Query["searchoperator"];

        var view = await mDbContext.Staff.GetStaff(new StaffService.GetStaffParams
        {
          Page = page,
          PageSize = pageSize,
          Sort = sort,
          Order = order,
          SearchStrings = searchString.ToList(),
          SearchColumns = searchColumns.ToList(),
          SearchOperators = searchOperator.ToList(),
          SearchStack = searchStack.ToList(),
        });


      var data = (await view.Query.ToListAsync()).ToList();
        
        return Ok(new
        {
          Status = 200,
          Message = "Ok",
          Data = data.MutateObject<List<StaffResponse>>(),
          Meta = new
          {
            view.Total,
          }
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { Status = 500, ex.Message });
      }
    }

    [HttpGet("{id}")]
    [AdminAuth]
    public async Task<ActionResult> GetOne(string id)
    {
      try
      {

        var view = await mDbContext.Staff.GetStaff(new StaffService.GetStaffParams { StaffId = id });

        var data = await view.Query.ToListAsync();

        if (data.Count < 1)
        {
          return NotFound(new { status = 401, message = "Not found" });
        }

        return Ok(new
        {
          Status = 200,
          Message = "Ok",
          Data = data[0].MutateObject<StaffResponse>(),
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { Status = 500, ex.Message });
      }
    }

    #endregion

    #region Helpers

    public class StaffResponse
    {
      public string Id { get; set; }
      public string FullName { get; set; }
      public string Email { get; set; }
      public string Address { get; set; }
      public string Position { get; set; }
      public string PhoneNumber { get; set; }
      public string Gender { get; set; }
      public string Degree { get; set; }
      public DateTime? Dob { get; set; }
      public DateTime DateModified { get; set; }
      public DateTime DateCreated { get; set; }
    }

    #endregion

  }

}