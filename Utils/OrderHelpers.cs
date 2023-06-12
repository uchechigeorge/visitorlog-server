using System.Linq;
using System.Linq.Expressions;

namespace VisitorLog.Server.Utils
{

  public static class DynamicOrder
  {
    
    /// <summary>
    /// Custom expre
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="propertyName"></param>
    /// <param name="descending"></param>
    /// <param name="anotherLevel"></param>
    /// <returns></returns>
    private static IOrderedQueryable<T> OrderingHelper<T>(IQueryable<T> source, string propertyName, bool descending, bool anotherLevel)
    {
      ParameterExpression param = Expression.Parameter(typeof(T), string.Empty); // I don't care about some naming
      MemberExpression property = Expression.PropertyOrField(param, propertyName);
      LambdaExpression sort = Expression.Lambda(property, param);
      MethodCallExpression call = Expression.Call(
          typeof(Queryable),
          (!anotherLevel ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
          new[] { typeof(T), property.Type },
          source.Expression,
          Expression.Quote(sort));
      return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
    }

    /// <summary>
    /// Sorts the elements of a sequence in an ascending order according to a specified key string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The sequence to sort</param>
    /// <param name="propertyName">String representation of the property to perform the sort query on</param>
    /// <returns></returns>
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
    {
      return OrderingHelper(source, propertyName, false, false);
    }

    /// <summary>
    /// Sorts the elements of a sequence in a specified order according to a specified key string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The sequence to sort</param>
    /// <param name="propertyName">String representation of the property to perform the sort query on</param>
    /// <param name="descending">Flag to determine if to sort elements in descending order</param>
    /// <returns></returns>
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, bool descending)
    {
      return OrderingHelper(source, propertyName, descending, false);
    }

    /// <summary>
    /// Sorts the elements of a sequence in a descending order according to a specified key string.
    /// </summary>
    /// <param name="source">The sequence to sort</param>
    /// <param name="propertyName">String representation of the property to perform the sort query on</param>
    /// <returns></returns>
    public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
    {
      return OrderingHelper(source, propertyName, true, false);
    }

    /// <summary>
    /// Performs a subsequent ordering of elements in a sequence in an ascending order according to a specified key string
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The sequence to sort</param>
    /// <param name="propertyName">String representation of the property to perform the sort query on</param>
    /// <returns></returns>
    public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName)
    {
      return OrderingHelper(source, propertyName, false, true);
    }

    /// <summary>
    /// Performs a subsequent ordering of elements in a sequence in a specified order according to a specified key string
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The sequence to sort</param>
    /// <param name="propertyName">String representation of the property to perform the sort query on</param>
    /// <param name="descending">Flag to determine if to sort elements in descending order</param>
    /// <returns></returns>
    public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName, bool descending)
    {
      return OrderingHelper(source, propertyName, descending, true);
    }

    /// <summary>
    /// Performs a subsequent ordering of elements in a sequence in a descending order according to a specified key string
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The sequence to sort</param>
    /// <param name="propertyName">String representation of the property to perform the sort query on</param>
    /// <returns></returns>
    public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string propertyName)
    {
      return OrderingHelper(source, propertyName, true, true);
    }

  }

}