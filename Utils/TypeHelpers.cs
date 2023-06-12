using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace VisitorLog.Server.Utils
{
	public static class TypeHelpers
	{

		#region Methods 

		/// <summary>
		/// Checks for valid integer from a string value and return it otherwise a default value
		/// </summary>
		/// <param name="value"></param>
		/// <param name="defaultValue">The default value</param>
		/// <returns></returns>
		public static int GetInt(this string value, int defaultValue = 0)
		{
			return int.TryParse(value, out int _) ? int.Parse(value) : defaultValue;
		}

		/// <summary>
		/// Returns a natural number from an integer value
		/// </summary>
		/// <param name="source"></param>
		/// <param name="defaultValue"></param>
		/// <param name="absoluteValue">If set to true, returns absolute value for negative numbers</param>
		/// <returns></return>
		public static int GetNaturalInt(this int source, int defaultValue = 1, bool absoluteValue = false)
		{
			if (absoluteValue)
			{
				source = Math.Abs(source);
			}

			return source < 1 ? defaultValue : source;
		}

		/// <summary>
		/// Checks if a string/object has a truthy value
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsTruthy(this object value)
		{
			return value != null
			&& (value == (object)true
				|| (string)value == 1.ToString()
				|| (string)value == bool.TrueString.ToLower());
		}

		/// <summary>
		/// Mutates an object, <see cref="data"/>, of a specific type to another object of a different specified type
		/// </summary>
		/// <typeparam name="T">The type of data to mutate to</typeparam>
		/// <param name="data">The data to be mutated</param>
		/// <returns></returns>
		public static T MutateObject<T>(this object data)
		 where T : new()
		{
			return JsonConvert.DeserializeObject<T>(
						JsonConvert.SerializeObject(data, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
						new JsonSerializerSettings
						{
							Error = (object sender, ErrorEventArgs errorArgs) =>
							{
								errorArgs.ErrorContext.Handled = true;
							},
						});
		}

		/// <summary>
		/// Checks if an object is of numeric type
		/// </summary>
		/// <param name="o">Object to check</param>
		/// <returns></returns>
		public static bool IsNumericType(this object o)
		{
			switch (Type.GetTypeCode(o.GetType()))
			{
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				default:
					return false;
			}
		}
		
		/// <summary>
		/// Checks if a type is of numeric type
		/// </summary>
		/// <param name="type">Type to check</param>
		/// <returns></returns>
		public static bool IsNumericType(this Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				default:
					return false;
			}
		}

		#endregion

	}
}
