using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisitorLog.Server.Models;
using VisitorLog.Server.Utils;

namespace VisitorLog.Server.Services
{
  public static partial class VisitorService
  {

    #region Methods

    /// <summary>
    /// Custom query for fetching visitor data
    /// </summary>
    /// <param name="visitor"></param>a
    /// <param name="parameters"></param>
    /// <returns></returns>
    public async static Task<ViewQuery<Visitor>> GetVisitors(this DbSet<Visitor> visitor, GetVisitorParams parameters)
    {

      // Available column options for sort
      var sortOptions = new Dictionary<string, string>
      {
        { "Name", "" },
        { "DateCreated", "" }
      };


      // Available column options for search
      var searchColumnsOptions = new Dictionary<string, string>
      {
        { "Name", "" },
        { "PhoneNumber", "" },
        { "Email", "" },
      };

      /// Default columns for search
      var defaultSearchColumn = "Name";

      var query = visitor.Where(a => true).Include("Staff");

      if (!string.IsNullOrWhiteSpace(parameters.VisitorId))
      {
        var isValidId = int.TryParse(parameters.VisitorId, out int visitorId);
        if (isValidId)
        {
					query = query.Where(u => u.Id == visitorId);
        }
      }
      
      if (!string.IsNullOrWhiteSpace(parameters.StaffId))
      {
        var isValidId = int.TryParse(parameters.StaffId, out int staffId);
        if (isValidId)
        {
					query = query.Where(u => u.StaffId == staffId);
        }
      }

      if (!string.IsNullOrWhiteSpace(parameters.StartDate) && !string.IsNullOrWhiteSpace(parameters.EndDate))
      {
        var isValidStart = DateTime.TryParse(parameters.StartDate, out DateTime startDate);
        var isValidEnd = DateTime.TryParse(parameters.EndDate, out DateTime endDate);

        if (isValidStart && isValidEnd)
        {
          query = query.Where(v => v.VDate >= startDate && v.VDate <= endDate);
				}
			}

      if (View.HasSearchParameters(parameters.SearchColumns, parameters.SearchStrings))
      {
        var searchBuilder = View.GetSearchQueryBuilder(parameters.SearchColumns, parameters.SearchStrings, parameters.SearchOperators, parameters.SearchStack, searchColumnsOptions, defaultSearchColumn);

        var filter = searchBuilder.GetExpression<Visitor>();
        query = query.Where(filter);
      }

      var total = await query.CountAsync();
      query = query.OrderBy(View.GetSort(parameters.Sort, sortOptions), View.IsDescendingOrder(parameters.Order));
      query = query.Skip(parameters.PageSize.GetNaturalInt() * (parameters.Page.GetNaturalInt() - 1)).Take(parameters.PageSize.GetNaturalInt());

      return new ViewQuery<Visitor> { Query = query, Total = total };
    }



    public class GetVisitorParams : View.GetParams
    {
      public string VisitorId { get; set; }
      public string StaffId { get; set; }
      public string StartDate { get; set; }
      public string EndDate { get; set; }
    }

    #endregion

  }
}
