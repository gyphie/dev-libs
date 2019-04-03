namespace dev_libs.db_query
{
	public class QueryParameter
	{
		private QueryParameter() { }

		internal static QueryParameter InputParameter(string name, object value, System.Data.SqlDbType dbType)
		{
			QueryParameter param = new QueryParameter();
			param.Name = "@" + name.TrimStart('@');
			param.Value = value;
			param.ParameterType = ParameterType.Input;
			param.ValueType = dbType;
			return param;
		}
		internal static QueryParameter InputOutputParameter(string name, object value, System.Data.SqlDbType dbType)
		{
			QueryParameter param = new QueryParameter();
			param.Name = "@" + name.TrimStart('@');
			param.Value = value;
			param.ParameterType = ParameterType.InputOutput;
			param.ValueType = dbType;
			return param;
		}
		internal static QueryParameter OutputParameter(string name, System.Data.SqlDbType dbType)
		{
			QueryParameter param = new QueryParameter();
			param.Name = "@" + name.TrimStart('@');
			param.ParameterType = ParameterType.Output;
			param.ValueType = dbType;
			return param;
		}
		internal static QueryParameter ReturnParameter(string name, System.Data.SqlDbType dbType)
		{
			QueryParameter param = new QueryParameter();
			param.Name = "@" + name.TrimStart('@');
			param.ParameterType = ParameterType.Return;
			param.ValueType = dbType;
			return param;
		}

		public string Name { get; set; }
		public object Value { get; set; }
		public ParameterType ParameterType { get; set; }
		public System.Data.SqlDbType ValueType { get; set; }

		public T Out<T>(T defaultValue)
		{
			return DataLoader.Get(this.Value, defaultValue);
		}
	}

	public enum ParameterType
	{
		Input,
		Output,
		Return,
		InputOutput
	}

	public static class ParameterTypeExtension
	{
		public static bool In(this ParameterType parameterType, params ParameterType[] these) {
			foreach (var oneOfThese in these)
			{
				if (parameterType == oneOfThese) return true;
			}

			return false;
		}
	}
}
