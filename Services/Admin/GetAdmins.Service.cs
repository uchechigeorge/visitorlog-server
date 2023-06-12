using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisitorLog.Server.Models;
using VisitorLog.Server.Utils;

namespace VisitorLog.Server.Services
{
  public static partial class AdminService
  {

    #region Methods

    /// <summary>
    /// Custom query for fetching user data
    /// </summary>
    /// <param name="admin"></param>a
    /// <param name="parameters"></param>
    /// <returns></returns>
    public async static Task<ViewQuery<Admin>> GetAdmins(this DbSet<Admin> admin, GetAdminParams parameters)
    {

      // Available column options for sort
      var sortOptions = new Dictionary<string, string>
      {
        { "Name", "" },
        { "Username", "" },
        { "DateCreated", "" }
      };


      // Available column options for search
      var searchColumnsOptions = new Dictionary<string, string>
      {
        { "Name", "" },
        { "Username", "" },
      };

      /// Default columns for search
      var defaultSearchColumn = "Username";

      var query = admin.Where(a => true);

      if (!string.IsNullOrWhiteSpace(parameters.AdminId))
      {
        var isValidId = int.TryParse(parameters.AdminId, out int adminId);
        if (isValidId)
        {
					query = query.Where(u => u.Id == adminId);
        }
      }

      if (View.HasSearchParameters(parameters.SearchColumns, parameters.SearchStrings))
      {
        var searchBuilder = View.GetSearchQueryBuilder(parameters.SearchColumns, parameters.SearchStrings, parameters.SearchOperators, parameters.SearchStack, searchColumnsOptions, defaultSearchColumn);

        var filter = searchBuilder.GetExpression<Admin>();
        query = query.Where(filter);
      }

      var total = await query.CountAsync();
      query = query.OrderBy(View.GetSort(parameters.Sort, sortOptions), View.IsDescendingOrder(parameters.Order));
      query = query.Skip(parameters.PageSize.GetNaturalInt() * (parameters.Page.GetNaturalInt() - 1)).Take(parameters.PageSize.GetNaturalInt());

      return new ViewQuery<Admin> { Query = query, Total = total };
    }



    public class GetAdminParams : View.GetParams
    {
      public string AdminId { get; set; }
    }

    #endregion

  }
}
