using System;
using System.Data;

namespace dev_libs.db_query
{
	public class DataLoader
	{
		public DataLoader(IDataReader reader)
		{
			this.reader = reader;
		}

		private IDataReader reader;

		public T Get<T>(string columnName, T defaultValue, bool columnRequired = true)
		{
			if (!columnRequired)
			{
				try
				{
					reader.GetOrdinal(columnName);
				}
				catch (IndexOutOfRangeException)
				{
					return defaultValue;
				}
			}

			return DataLoader.Get<T>(reader[columnName], defaultValue);
		}

		public static T Get<T>(object objValue, T defaultValue)
		{
			Type valueType = typeof(T);
			if (Convert.IsDBNull(objValue) || objValue == null)
			{
				return defaultValue;
			}
			
			if (valueType == typeof(DateTime))
			{
				return (T)(object)Convert.ToDateTime(objValue);
			}
			else if (valueType == typeof(Int32))
			{
				return (T)(object)Convert.ToInt32(objValue);
			}
			else if (valueType == typeof(decimal))
			{
				return (T)(object)Convert.ToDecimal(objValue);
			}
			else if (valueType == typeof(double))
			{
				return (T)(object)Convert.ToDouble(objValue);
			}
			else if (valueType == typeof(string))
			{
				return (T)(object)Convert.ToString(objValue)?.Trim();
			}
			else if (valueType == typeof(bool))
			{
				return (T)(object)Convert.ToBoolean(objValue);
			}
			else
			{
				return (T)objValue;
			}
		}
	}
}
