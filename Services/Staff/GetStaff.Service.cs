using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisitorLog.Server.Models;
using VisitorLog.Server.Utils;

namespace VisitorLog.Server.Services
{
  public static partial class StaffService
  {

    #region Methods

    /// <summary>
    /// Custom query for fetching staff data
    /// </summary>
    /// <param name="staff"></param>a
    /// <param name="parameters"></param>
    /// <returns></returns>
    public async static Task<ViewQuery<Staff>> GetStaff(this DbSet<Staff> staff, GetStaffParams parameters)
    {

      // Available column options for sort
      var sortOptions = new Dictionary<string, string>
      {
        { "Name", "" },
        { "FullName", "" },
        { "DateCreated", "" }
      };


      // Available column options for search
      var searchColumnsOptions = new Dictionary<string, string>
      {
        { "Email", "" },
        { "FullName", "" },
        { "Address", "" },
        { "PhoneNumber", "" },
      };

      /// Default columns for search
      var defaultSearchColumn = "FullName";

      var query = staff.Where(a => true);

      if (!string.IsNullOrWhiteSpace(parameters.StaffId))
      {
        var isValidId = int.TryParse(parameters.StaffId, out int staffId);
        if (isValidId)
        {
					query = query.Where(u => u.Id == staffId);
        }
      }

      if (View.HasSearchParameters(parameters.SearchColumns, parameters.SearchStrings))
      {
        var searchBuilder = View.GetSearchQueryBuilder(parameters.SearchColumns, parameters.SearchStrings, parameters.SearchOperators, parameters.SearchStack, searchColumnsOptions, defaultSearchColumn);

        var filter = searchBuilder.GetExpression<Staff>();
        query = query.Where(filter);
      }

      var total = await query.CountAsync();
      query = query.OrderBy(View.GetSort(parameters.Sort, sortOptions), View.IsDescendingOrder(parameters.Order));
      query = query.Skip(parameters.PageSize.GetNaturalInt() * (parameters.Page.GetNaturalInt() - 1)).Take(parameters.PageSize.GetNaturalInt());

      return new ViewQuery<Staff> { Query = query, Total = total };
    }



    public class GetStaffParams : View.GetParams
    {
      public string StaffId { get; set; }
    }

    #endregion

  }
}
