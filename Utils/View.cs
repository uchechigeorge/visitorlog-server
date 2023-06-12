
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace VisitorLog.Server.Utils
{
	public static class View
	{

		#region Methods

		/// <summary>
		/// Checks if search parameters are provided for search query
		/// </summary>
		/// <param name="columns"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static bool HasSearchParameters(List<string> columns, List<string> values)
		{
			if (columns == null)
			{
				columns = new List<string>();
			}

			if (values == null)
			{
				values = new List<string>();
			}

			return values.Any(value => !string.IsNullOrWhiteSpace(value)) || columns.Any(column => !string.IsNullOrWhiteSpace(column));
		}

		/// <summary>
		/// Gets a valid sort column from list of column options, otherwise the default value
		/// </summary>
		/// <param name="column"></param>
		/// <param name="options">Column options</param>
		/// <param name="defaultColumn">The default sort column</param>
		/// <returns></returns>
		public static string GetSort(string column, Dictionary<string, string> options, string defaultColumn = "DateModified")
		{
			if (string.IsNullOrWhiteSpace(column)) return defaultColumn;

			//bool containsKey = options.TryGetValue(value, out string selected);

			var selected = options.FirstOrDefault(option => option.Key.ToLower() == column.ToLower());
			var contains = options.Any(option => option.Key.ToLower() == column.ToLower());

			return contains ? (string.IsNullOrWhiteSpace(selected.Value) ? selected.Key : selected.Value) : defaultColumn;
		}

		/// <summary>
		/// Returns true for descending order and vice versa
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsDescendingOrder(object value)
		{
			return ((string)value)?.ToLower() == "desc" || value.IsTruthy();
		}

		/// <summary>
		/// Builds a search query
		/// </summary>
		/// <param name="columns">List of columns to build search query</param>
		/// <param name="values">The search values to build search query</param>
		/// <param name="operators">Operator/conditional to apply for a search query. 
		/// If value provided is invalid, <see cref="RegExOperators.Contains"/> is used</param>
		/// <param name="stack">Type of stack to apply to next search filter. 
		/// If invalid value is provided, <see cref="SearchStackType.And"/> is used</param>
		/// <param name="searchColumnsOptions">Available columns for search</param>
		/// <param name="defaultColumn">This parameter is user for an invalid column name 
		/// (ie. a value in the columns list is not contained in the avaliable columns for search) or no column name is provided
		/// </param>
		/// <returns></returns>
		public static List<SearchRule> GetSearchQueryBuilder(
			List<string> columns, List<string> values, List<string> operators, List<string> stack,
			Dictionary<string, string> searchColumnsOptions, string defaultColumn = "")
		{
			// If any of the list is null, initialize..to avoid null reference errrors
			if (columns == null)
			{
				columns = new List<string>();
			}
			if (values == null)
			{
				values = new List<string>();
			}
			if (operators == null)
			{
				operators = new List<string>();
			}
			if (stack == null)
			{
				stack = new List<string>();
			}

			var numOfRows = Math.Max(columns.Count, values.Count);

			// Build filter rules
			var filters = new List<SearchRule>();
			for (int i = 0; i < numOfRows; i++)
			{
				filters.Add(new SearchRule
				{
					Column = GetSearchColumn(columns?.Count > i ? columns[i] : defaultColumn, searchColumnsOptions, defaultColumn),
					Value = values?.Count > i ? values[i] : string.Empty,
					Operator = operators?.Count > i ? (RegExOperators)operators[i].GetInt() : RegExOperators.Contains,
					Stack = stack?.Count > i ? (SearchStackType)stack[i].GetInt() : SearchStackType.And,
				});
			}

			return filters;
		}

		/// <summary>
		/// Gets a valid column for a search query
		/// </summary>
		/// <param name="column">Provided column</param>
		/// <param name="options">Available column options</param>
		/// <param name="defaultColumn">Column name to replace if column provided is invalid(ie. is not contained in the column options)</param>
		/// <returns></returns>
		public static string GetSearchColumn(string column, Dictionary<string, string> options, string defaultColumn)
		{
			// Check if the column supplied exists on the key-value pairs options
			var selected = options.FirstOrDefault(option => option.Key.ToLower() == column.ToLower());
			var contains = options.Any(option => option.Key.ToLower() == column.ToLower());

			// If the key-value pair does not have a value, use the key. Otherwise, use value
			return contains ? (string.IsNullOrWhiteSpace(selected.Value) ? selected.Key : selected.Value) : defaultColumn;
		}

		/// <summary>
		/// Gets expression for filter rules to be used in, most probably, in Where clause
		/// </summary>
		/// <typeparam name="T">Type of entity</typeparam>
		/// <param name="rules">Rules to build query on</param>
		/// <returns></returns>
		public static Expression<Func<T, bool>> GetExpression<T>(this List<SearchRule> rules)
		{
			var entity = Expression.Parameter(typeof(T), "user");
			Expression expr = null;

			for (int i = 0; i < rules.Count; i++)
			{
				var rule = rules[i];

				// Initialize left side expression, ie. the property of the entity
				MemberExpression left;

				// Check if it is a nested property
				if (rule.Column.Contains('.'))
				{
					// Split properties
					var properties = rule.Column.Split('.');
					// Initialize left expression with first property
					left = Expression.Property(entity, properties[0]);

					// Re assign left expression with subsequent nested properties
					for (int j = 1; j < properties.Length; j++)
					{
						left = Expression.Property(left, properties[j]);
					}

				}
				// else, use as is
				else 
				{
					left = Expression.Property(entity, rule.Column);
				}

				var right = Expression.Constant(rule.Value);
				Expression e1 = rule.Operator.GetExpression(left, right);

				expr = (expr == null || i == 0) ? e1 : (rules[i - 1].Stack == View.SearchStackType.Or ? Expression.Or(expr, e1) : Expression.AndAlso(expr, e1));
			}

			var result = Expression.Lambda<Func<T, bool>>(expr, entity);
			return result;
		}

		/// <summary>
		/// Gets an expression for an operator with right and left expressions
		/// </summary>
		/// <param name="source">Operator on which expression is built on</param>
		/// <param name="left">The left expression</param>
		/// <param name="right">The right expression</param>
		/// <param name="strict">In case of invalid comparisons(eg. comparing int and string values), this indicates if to ignore and set the expression to true(ie. when paramter is false; default action) or set is as a failed expression(ie. when paramter is set to true)</param>
		/// <returns></returns>
		public static Expression GetExpression(this RegExOperators source, Expression left, Expression right, bool strict = false)
		{
			// Initialize expression

			Expression expr = null;
			switch (source)
			{
				case RegExOperators.Contains:

					// If left expression is of string type
					if (left.Type == typeof(string))
					{
						// Call string.Contains(string value) method
						expr = Expression.Call(
						Expression.Convert(left, typeof(string)),
						typeof(string).GetMethod("Contains", new[] { typeof(string) }),
						Expression.Convert(right, typeof(string)));
					}
					else
					{
						// In case of other type(ie. not string), return value based on strict parameter
						expr = Expression.Constant(!strict);
					}
					break;
				case RegExOperators.Equal:
					// Compile and get value of right expression
					var rightCompiled_Equal = Expression.Lambda<Func<string>>(right).Compile()();
					// Check if left property is a numeric type and the right expression is a valid number
					var isValidNumber_Equal = double.TryParse(rightCompiled_Equal, out double rightValue_Equal) && left.Type.IsNumericType();
					try
					{
						// Check if left and right expression are equal
						if (isValidNumber_Equal)
							expr = Expression.Equal(left, Expression.Convert(Expression.Constant(rightValue_Equal), left.Type));
						else
							expr = Expression.Equal(left, Expression.Convert(right, left.Type));
					}
					catch (Exception)
					{
						// In case of error (eg. invalid type operation b/w int vs string), return value based on strict parameter
						expr = Expression.Constant(!strict);
					}

					break;
				case RegExOperators.NotEqual:
					// Compile and get value of right expression
					var rightCompiled_NEqual = Expression.Lambda<Func<string>>(right).Compile()();
					// Check if left property is a numeric type and the right expression is a valid number
					var isValidNumber_NEqual = double.TryParse(rightCompiled_NEqual, out double rightValue_NEqual) && left.Type.IsNumericType();

					try
					{
						// Check if left and right expression are not equal
						if (isValidNumber_NEqual)
							expr = Expression.NotEqual(left, Expression.Convert(Expression.Constant(rightValue_NEqual), left.Type));
						else
							expr = Expression.NotEqual(left, Expression.Convert(right, left.Type));
					}
					catch (Exception)
					{
						// In case of error (eg. invalid type operation b/w int vs string), return value based on strict parameter
						expr = Expression.Constant(!strict);
					}

					break;
				case RegExOperators.StartsWith:
					// If left property is of string type
					if (left.Type == typeof(string))
					{
						// Call string.StartsWith(string value) method
						expr = Expression.Call(
						Expression.Convert(left, typeof(string)),
						typeof(string).GetMethod("StartsWith", new[] { typeof(string) }),
						Expression.Convert(right, typeof(string)));
					}
					else
					{
						// In case of other types(ie. not string), return value based on strict parameter
						expr = Expression.Constant(!strict);
					}

					break;
				case RegExOperators.EndsWith:
					// If left property is of string type
					if (left.Type == typeof(string))
					{
						// Call string.EndsWith(string value) method
						expr = Expression.Call(
						Expression.Convert(left, typeof(string)),
						typeof(string).GetMethod("EndsWith", new[] { typeof(string) }),
						Expression.Convert(right, typeof(string)));
					}
					else
					{
						// In case of other types(ie. not string), return value based on strict parameter
						expr = Expression.Constant(!strict);
					}

					break;
				case RegExOperators.IsEmpty:
					// Check if left property is a nullable type
					var isNullable_Empty = !left.Type.IsValueType || Nullable.GetUnderlyingType(left.Type) != null;
					// If property is nullable, ...
					if (isNullable_Empty)
					{
						// ... and if property is of string type
						if (left.Type == typeof(string))
						{
							// Call string.IsNullOrEmpty(string value) method
							expr = Expression.Call(
								typeof(string).GetMethod("IsNullOrEmpty", new[] { typeof(string) }),
								left);
						}
						else
						{
							// ... check if left property has null value
							expr = Expression.Equal(left, Expression.Constant(null, typeof(object)));
						}
					}
					else
					{
						// In case of types that are not nullable, return value based on strict parameter
						expr = Expression.Constant(!strict);
					}

					break;
				case RegExOperators.IsNotEmpty:
					// Check if left property is a nullable type
					var isNullable_NEmpty = !left.Type.IsValueType || Nullable.GetUnderlyingType(left.Type) != null;
					// If property is nullable, ...
					if (isNullable_NEmpty)
					{
						// ... and if property is of string type
						if (left.Type == typeof(string))
						{
							// Check if string is not null and is not equal to string.Empty
							expr = Expression.AndAlso(
								Expression.NotEqual(left, Expression.Constant(null, typeof(object))),
								Expression.NotEqual(left, Expression.Constant(string.Empty, typeof(string)))
							);
						}
						else
						{
							// ... check if left property does not have null value
							expr = Expression.NotEqual(left, Expression.Constant(null, typeof(object)));
						}
					}
					else
					{
						// In case of types that are not nullable, return value based on strict parameter
						expr = Expression.Constant(!strict);
					}

					break;
				case RegExOperators.IsGreaterThan:
					// Compile and get value right expression
					var rightCompiled_GT = Expression.Lambda<Func<string>>(right).Compile()();

					// Check if left property is a numeric type && right value is a valid number
					var isValidNumber = double.TryParse(rightCompiled_GT, out double rightValue_GT) && left.Type.IsNumericType();

					try
					{
						// If valid number, check if right expression is greater than left expression
						if (isValidNumber)
							expr = Expression.GreaterThan(left, Expression.Convert(Expression.Constant(rightValue_GT), left.Type));
						else
							// If other types, check to greater value. Will probably throw an error :)
							// TODO: Add valid > operator checks for types like DateTime, string
							expr = Expression.GreaterThan(left, Expression.Convert(right, left.Type));
					}
					catch (Exception)
					{
						// In case of any erro, return value based on strict parameter
						expr = Expression.Constant(!strict);
					}

					break;
				case RegExOperators.IsLessThan:
					// Compile and get value right expression
					var rightCompiled_LT = Expression.Lambda<Func<string>>(right).Compile()();

					// Check if left property is a numeric type && right value is a valid number
					isValidNumber = double.TryParse(rightCompiled_LT, out double rightValue_LT);
					try
					{
						// If valid number, check if right expression is less than left expression
						if (isValidNumber)
							expr = Expression.LessThan(left, Expression.Convert(Expression.Constant(rightValue_LT), left.Type));
						else
							// If other types, check to greater value.Will probably throw an error :)
							// TODO: Add valid < operator checks for types like DateTime, string
							expr = Expression.LessThan(left, Expression.Convert(right, left.Type));
					}
					catch (Exception)
					{
						// In case of any erro, return value based on strict parameter
						expr = Expression.Constant(!strict);
					}

					break;
				case RegExOperators.IsAnyOf:
					// Compile and get values for right value
					var rightCompiled_AnyOf = Expression.Lambda<Func<string>>(right).Compile()();
					// Split right value by , char
					var rightArrVals = rightCompiled_AnyOf.Split(',').ToList();

					// If left property is of string type
					if (left.Type == typeof(string))
					{
						// Call List<string>().Contains(string value) method
					  expr = Expression.Call(
							Expression.Constant(rightArrVals, typeof(List<string>)),
							typeof(List<string>).GetMethod("Contains", new[] { typeof(string) }),
							left
						);
					}
					else
					{
						// In case of other types, return value based on strict parameter
						expr = Expression.Constant(!strict);
					}

					break;
				default:
					expr = Expression.Constant(!strict);
					break;
			}

			return expr;
		}


		#endregion

		#region Helpers

		/// <summary>
		/// Base params for making GET requests
		/// </summary>
		public class GetParams
		{
			/// <summary>
			/// The row index. Not zero index(ie. starts from 1)
			/// </summary>
			public int Page { get; set; }
			/// <summary>
			/// Number of rows to return
			/// </summary>
			public int PageSize { get; set; }
			/// <summary>
			/// Column name to apply sort
			/// </summary>
			public string Sort { get; set; }
			/// <summary>
			/// The sort direction (ie. 'asc' or 'desc')
			/// </summary>
			public string Order { get; set; }
			/// <summary>
			/// List of string to perform a search query for
			/// </summary>
			public List<string> SearchStrings { get; set; } = new List<string>();
			/// <summary>
			/// List of columns to perform a search query for
			/// </summary>
			public List<string> SearchColumns { get; set; } = new List<string>();
			/// <summary>
			/// List of operators to operators to perform a search query for.
			/// Valid values are referenced from <see cref="RegExOperators"/>.
			/// Eg. contains, is greater, is empty
			/// </summary>
			public List<string> SearchOperators { get; set; }  = new List<string>();
			/// <summary>
			/// The type of stacking in the filters applied to the search query: "or" or "and"
			/// </summary>
			public List<string> SearchStack { get; set; } = new List<string>();
		}

		/// <summary>
		/// Filter RegEx operator options
		/// </summary>
		public enum RegExOperators
		{
			Contains,
			Equal,
			NotEqual,
			StartsWith,
			EndsWith,
			IsEmpty,
			IsNotEmpty,
			IsGreaterThan,
			IsLessThan,
			IsAnyOf,
		}

		/// <summary>
		/// Search stack options
		/// </summary>
		public enum SearchStackType
		{
			And,
			Or
		}

		/// <summary>
		/// 
		/// </summary>
		public class SearchRule
		{
			/// <summary>
			/// The column to apply search filter against
			/// </summary>
			public string Column { get; set; }
			/// <summary>
			/// The search filter value
			/// </summary>
			public string Value { get; set; }
			/// <summary>
			/// Type of operator/conditional to apply to search filter
			/// </summary>
			public RegExOperators Operator { get; set; }
			/// <summary>
			/// The type of stack to add to search filter
			/// </summary>
			public SearchStackType Stack { get; set; }
		}



	}

	public class ViewQuery<T>
	{
		public IQueryable<T> Query { get; set; }
		public int Total { get; set; }
	}

		#endregion

}
