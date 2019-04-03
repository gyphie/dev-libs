using System;
public static class DbValueMap
{
	public static bool IsNull<T>(T value)
	{
		Type valueType = typeof(T);
		if (Convert.IsDBNull(value))
		{
			return true;
		}

		if (valueType == typeof(DateTime))
		{
			return value.Equals(DateTime.MinValue);
		}
		else if (valueType == typeof(Int32))
		{
			return value.Equals(Int32.MinValue);
		}
		else if (valueType == typeof(decimal))
		{
			return value.Equals(decimal.MinValue);
		}
		else if (valueType == typeof(double))
		{
			return value.Equals(double.MinValue);
		}
		else if (valueType == typeof(string))
		{
			return value == null;
		}
		else if (valueType == typeof(bool))
		{
			return false;   // Don't have a special null value (this means setting a DB bit value to null requires passing DBNull)
		}
		else if (valueType == typeof(Guid))
		{
			return valueType.Equals(Guid.Empty);
		}
		else if (valueType == typeof(short))
		{
			return valueType.Equals(short.MinValue);
		}
		else if (valueType == typeof(float))
		{
			return valueType.Equals(float.MinValue);
		}
		else if (valueType == typeof(long))
		{
			return valueType.Equals(long.MinValue);
		}
		else if (valueType == typeof(char))
		{
			return valueType.Equals('_');   // Selected a character that is unlikely to be used when storing a single character in the DB
		}
		else
		{
			return value == null;
		}
	}

	public static object ToDBNull<T>(this T value)
	{
		if (Convert.IsDBNull(value) || (object)value == null)
		{
			return DBNull.Value;
		}

		Type valueType = typeof(T);

		if (valueType == typeof(DateTime) && (DateTime)(object)value == DateTime.MinValue)
		{
			return DBNull.Value;
		}
		else if (valueType == typeof(Int32) && (Int32)(object)value == Int32.MinValue)
		{
			return DBNull.Value;
		}
		else if (valueType == typeof(decimal) && (decimal)(object)value == decimal.MinValue)
		{
			return DBNull.Value;
		}
		else if (valueType == typeof(double) && (double)(object)value == double.MinValue)
		{
			return DBNull.Value;
		}
		else if (valueType == typeof(Guid) && (Guid)(object)value == Guid.Empty)
		{
			return DBNull.Value;
		}
		else if (valueType == typeof(short) && (short)(object)value == short.MinValue)
		{
			return DBNull.Value;
		}
		else if (valueType == typeof(float) && (float)(object)value == float.MinValue)
		{
			return DBNull.Value;
		}
		else if (valueType == typeof(long) && (long)(object)value == long.MinValue)
		{
			return DBNull.Value;
		}
		else if (valueType == typeof(char) && (char)(object)value == '_')
		{
			return DBNull.Value;
		}

		return value;
	}

}