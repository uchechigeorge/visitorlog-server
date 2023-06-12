using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitorLog.Server.Attributes;
using VisitorLog.Server.Services;
using VisitorLog.Server.Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VisitorLog.Server.Controllers.Admin
{
	public partial class VisitorController : ControllerBase
	{

    #region Methods

    [HttpGet]
    [AdminAuth]
    public async Task<ActionResult> Get()
    {
      try
      {

        string staffId = HttpContext.Request.Query["staffid"];
        int page = HttpContext.Request.Query["page"].ToString().GetInt();
        int pageSize = HttpContext.Request.Query["pagesize"].ToString().GetInt(50);
        string startDate = HttpContext.Request.Query["startdate"];
        string endDate = HttpContext.Request.Query["enddate"];
        string sort = HttpContext.Request.Query["sort"];
        string order = HttpContext.Request.Query["order"];
        IEnumerable<string> searchString = HttpContext.Request.Query["searchstring"];
        IEnumerable<string> searchColumns = HttpContext.Request.Query["searchcolumn"];
        IEnumerable<string> searchStack = HttpContext.Request.Query["searchstack"];
        IEnumerable<string> searchOperator = HttpContext.Request.Query["searchoperator"];

        var view = await mDbContext.Visitors.GetVisitors(new VisitorService.GetVisitorParams
        {
          Page = page,
          PageSize = pageSize,
          Sort = sort,
          Order = order,
          SearchStrings = searchString.ToList(),
          SearchColumns = searchColumns.ToList(),
          SearchOperators = searchOperator.ToList(),
          SearchStack = searchStack.ToList(),
          StaffId = staffId,
          StartDate = startDate,
          EndDate = endDate,
        });

        var data = (await view.Query.ToListAsync()).ToList();

        return Ok(new
        {
          Status = 200,
          Message = "Ok",
          Data = data.MutateObject<List<VisitorResponse>>(),
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

        var view = await mDbContext.Visitors.GetVisitors(new VisitorService.GetVisitorParams { VisitorId = id });

        var data = await view.Query.ToListAsync();

        if (data.Count < 1)
        {
          return NotFound(new { status = 401, message = "Not found" });
        }

        return Ok(new
        {
          Status = 200,
          Message = "Ok",
          Data = data[0].MutateObject<VisitorResponse>(),
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { Status = 500, ex.Message });
      }
    }

    #endregion

    #region Helpers

    public class VisitorResponse
    {
      public int Id { get; set; }
      public string Name { get; set; }
      public DateTime? VDate { get; set; }
      public string Purpose { get; set; }
      public int? StaffId { get; set; }
      public string PhoneNumber { get; set; }
      public string Email { get; set; }
      public string Gender { get; set; }
      public DateTime DateModified { get; set; }
      public DateTime DateCreated { get; set; }
      public VisitorStaff Staff { get; set; } = new VisitorStaff();
    }

    public class VisitorStaff
    {
      public int Id { get; set; }
      public string FullName { get; set; }
    }

    #endregion
  }
}
