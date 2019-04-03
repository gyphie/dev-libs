using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections;
using System.Configuration;

namespace dev_libs.db_query
{
	public static class Query
	{
		public static string DefaultConnectionStringName { get; set; } = "local";

		private static Dictionary<System.Type, SqlDbType> typeMap;
		static Query()
		{
			Query.typeMap = new Dictionary<System.Type, SqlDbType>(15) {
				{ typeof(string), SqlDbType.NVarChar },
				{ typeof(char), SqlDbType.NVarChar },
				{ typeof(char[]), SqlDbType.NVarChar },
				{ typeof(byte), SqlDbType.TinyInt },
				{ typeof(byte[]), SqlDbType.VarBinary },
				{ typeof(Int16), SqlDbType.SmallInt },
				{ typeof(Int32), SqlDbType.Int },
				{ typeof(Int64), SqlDbType.BigInt },
				{ typeof(bool), SqlDbType.Bit },
				{ typeof(DateTime), SqlDbType.DateTime2 },
				{ typeof(DateTimeOffset), SqlDbType.DateTimeOffset },
				{ typeof(decimal), SqlDbType.Decimal },
				{ typeof(float), SqlDbType.Real },
				{ typeof(double), SqlDbType.Float },
				{ typeof(TimeSpan), SqlDbType.Time }
			};
		}

		public static string GetConnectionString(string name = null)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				name = Query.DefaultConnectionStringName;
			}

			string connectionString = string.Empty;

			try
			{
				connectionString = ConfigurationManager.ConnectionStrings[name].ConnectionString;
			}
			catch { }

			if (string.IsNullOrWhiteSpace(connectionString))
			{
				throw new Exception($"Connection String \"{name}\" was not found");
			}

			return connectionString;
		}

		private static SqlDbType MapType<T>(T value)
		{
			if (value is ICollection<byte>) return SqlDbType.VarBinary;

			if (Query.typeMap.TryGetValue(typeof(T), out var dbType))
			{
				return dbType;
			}
			else
			{
				return SqlDbType.NVarChar;
			}
		}

		public static List<T> ExecuteReader<T>(string command, Func<DataLoader, T> mapper, params QueryParameter[] queryParameters)
		{
			return ExecuteReader(command, Type.StoredProc, Query.DefaultConnectionStringName, mapper, queryParameters);
		}
		public static List<T> ExecuteReader<T>(string command, Type queryType, Func<DataLoader, T> mapper, params QueryParameter[] queryParameters)
		{
			return ExecuteReader(command, queryType, Query.DefaultConnectionStringName, mapper, queryParameters);
		}
		public static List<T> ExecuteReader<T>(string command, Type queryType, string connectionStringName, Func<DataLoader, T> mapper, params QueryParameter[] queryParameters)
		{
			return ExecuteReader(command, queryType, connectionStringName, 30, mapper, queryParameters);
		}
		public static List<T> ExecuteReader<T>(string command, Type queryType, string connectionStringName, int timeOutSeconds, Func<DataLoader, T> mapper, params QueryParameter[] queryParameters)
		{
			List<T> results = new List<T>();
			using (SqlConnection conn = new SqlConnection(Query.GetConnectionString(connectionStringName)))
			{
				conn.Open();

				using (SqlCommand cmd = new SqlCommand(command, conn))
				{
					cmd.CommandType = queryType == Type.StoredProc ? CommandType.StoredProcedure : CommandType.Text;
					cmd.CommandTimeout = timeOutSeconds;

					var parameters = Query.BuildSqlParameters(queryParameters);
					cmd.Parameters.AddRange(parameters);

					using (IDataReader dr = cmd.ExecuteReader())
					{
						// Get any output parameters.
						foreach (var op in queryParameters.Where(p => p.ParameterType.In(ParameterType.Output, ParameterType.InputOutput, ParameterType.Return)))
						{
							op.Value = cmd.Parameters[op.Name].Value;
						}

						DataLoader dl = new DataLoader(dr); // We only instantiate a DataLoader once and advancing the reader outside it affects the dataloader as well
						while (dr.Read())
						{
							var obj = mapper(dl);
							if (obj != null)
							{
								results.Add(obj);
							}
						}
					}

				}
			}

			return results;
		}

		public static void ExecuteNonQuery(string command, params QueryParameter[] queryParameters)
		{
			ExecuteNonQuery(command, Type.StoredProc, Query.DefaultConnectionStringName, queryParameters);
		}
		public static void ExecuteNonQuery(string command, Type queryType, params QueryParameter[] queryParameters)
		{
			ExecuteNonQuery(command, queryType, Query.DefaultConnectionStringName, queryParameters);
		}
		public static void ExecuteNonQuery(string command, Type queryType, string connectionStringName, params QueryParameter[] queryParameters)
		{
			ExecuteNonQuery(command, queryType, connectionStringName, 30, queryParameters);
		}

		public static void ExecuteNonQuery(string command, Type queryType, string connectionStringName, int timeOutSeconds, params QueryParameter[] queryParameters)
		{
			using (SqlConnection conn = new SqlConnection(Query.GetConnectionString(connectionStringName)))
			{
				conn.Open();

				using (SqlCommand cmd = new SqlCommand(command, conn))
				{
					cmd.CommandType = queryType == Type.StoredProc ? CommandType.StoredProcedure : CommandType.Text;
					cmd.CommandTimeout = timeOutSeconds;

					var parameters = Query.BuildSqlParameters(queryParameters);
					cmd.Parameters.AddRange(parameters);

					cmd.ExecuteNonQuery();

					// Get any output parameters.
					foreach (var op in queryParameters.Where(p => p.ParameterType.In(ParameterType.Output, ParameterType.InputOutput, ParameterType.Return)))
					{
						op.Value = cmd.Parameters[op.Name].Value;
					}
				}
			}
		}

		private const int defaultTimeout = 30;
		public static T ExecuteScalar<T>(string command, params QueryParameter[] queryParameters)
		{
			return ExecuteScalar<T>(command, Type.StoredProc, Query.DefaultConnectionStringName, defaultTimeout, queryParameters);
		}
		public static T ExecuteScalar<T>(string command, T defaultValue, params QueryParameter[] queryParameters)
		{
			return ExecuteScalar<T>(command, defaultValue, Type.StoredProc, Query.DefaultConnectionStringName, defaultTimeout, queryParameters);
		}
		public static T ExecuteScalar<T>(string command, Type queryType, params QueryParameter[] queryParameters)
		{
			return ExecuteScalar<T>(command, queryType, Query.DefaultConnectionStringName, defaultTimeout, queryParameters);
		}
		public static T ExecuteScalar<T>(string command, T defaultValue, Type queryType, params QueryParameter[] queryParameters)
		{
			return ExecuteScalar<T>(command, defaultValue, queryType, Query.DefaultConnectionStringName, defaultTimeout, queryParameters);
		}
		public static T ExecuteScalar<T>(string command, Type queryType, string connectionStringName, params QueryParameter[] queryParameters)
		{
			return ExecuteScalar<T>(command, queryType, connectionStringName, defaultTimeout, queryParameters);
		}
		public static T ExecuteScalar<T>(string command, string connectionStringName, params QueryParameter[] queryParameters)
		{
			return ExecuteScalar<T>(command, Query.Type.StoredProc, connectionStringName, defaultTimeout, queryParameters);
		}
		public static T ExecuteScalar<T>(string command, T defaultValue, string connectionStringName, params QueryParameter[] queryParameters)
		{
			return ExecuteScalar<T>(command, defaultValue, Query.Type.StoredProc, connectionStringName, defaultTimeout, queryParameters);
		}
		public static T ExecuteScalar<T>(string command, Type queryType, string connectionStringName, int timeOutSeconds, params QueryParameter[] queryParameters)
		{
			return ExecuteScalar<T>(command, default(T), queryType, connectionStringName, defaultTimeout, queryParameters);
		}
		public static T ExecuteScalar<T>(string command, T defaultValue, Type queryType, string connectionStringName, int timeOutSeconds, params QueryParameter[] queryParameters)
		{
			using (SqlConnection conn = new SqlConnection(Query.GetConnectionString(connectionStringName)))
			{
				conn.Open();

				using (SqlCommand cmd = new SqlCommand(command, conn))
				{
					cmd.CommandType = queryType == Type.StoredProc ? CommandType.StoredProcedure : CommandType.Text;
					cmd.CommandTimeout = timeOutSeconds;

					var parameters = Query.BuildSqlParameters(queryParameters);
					cmd.Parameters.AddRange(parameters);

					object result = cmd.ExecuteScalar();

					// Get any output parameters.
					foreach (var op in queryParameters.Where(p => p.ParameterType.In(ParameterType.Output, ParameterType.InputOutput, ParameterType.Return)))
					{
						op.Value = cmd.Parameters[op.Name].Value;
					}

					return DataLoader.Get<T>(result, defaultValue);
				}
			}
		}

		public static DataSet ExecuteDataSet(string command, params QueryParameter[] queryParameters)
		{
			return ExecuteDataSet(command, Type.StoredProc, Query.DefaultConnectionStringName, queryParameters);
		}
		public static DataSet ExecuteDataSet(string command, Type queryType, params QueryParameter[] queryParameters)
		{
			return ExecuteDataSet(command, queryType, Query.DefaultConnectionStringName, queryParameters);
		}
		public static DataSet ExecuteDataSet(string command, Type queryType, string connectionStringName, params QueryParameter[] queryParameters)
		{
			return ExecuteDataSet(command, queryType, connectionStringName, 30, queryParameters);
		}
		public static DataSet ExecuteDataSet(string command, Type queryType, string connectionStringName, int timeOutSeconds, params QueryParameter[] queryParameters)
		{
			DataSet results = new DataSet();
			using (SqlConnection conn = new SqlConnection(Query.GetConnectionString(connectionStringName)))
			{
				conn.Open();

				using (SqlCommand cmd = new SqlCommand(command, conn))
				{
					cmd.CommandType = queryType == Type.StoredProc ? CommandType.StoredProcedure : CommandType.Text;
					cmd.CommandTimeout = timeOutSeconds;

					var parameters = Query.BuildSqlParameters(queryParameters);
					cmd.Parameters.AddRange(parameters);

					IDataAdapter da = new SqlDataAdapter(cmd);
					da.Fill(results);

					// Get any output parameters.
					foreach (var op in queryParameters.Where(p => p.ParameterType.In(ParameterType.Output, ParameterType.InputOutput, ParameterType.Return)))
					{
						op.Value = cmd.Parameters[op.Name].Value;
					}
				}
			}

			return results;
		}


		public static QueryParameter Param<T>(string name, T value, SqlDbType? valueType = null)
		{
			SqlDbType valType = valueType ?? Query.MapType(value);
			if (DbValueMap.IsNull(value)) return QueryParameter.InputParameter(name, DBNull.Value, valType);
			return QueryParameter.InputParameter(name, value, valType);
		}
		public static QueryParameter Param(string name, bool? value)
		{
			if (value == null || !value.HasValue) return QueryParameter.InputParameter(name, DBNull.Value, SqlDbType.Bit);
			return QueryParameter.InputParameter(name, value.Value, SqlDbType.Bit);
		}

		public static QueryParameter InOut<T>(string name, T value, SqlDbType? valueType = null)
		{
			SqlDbType valType = valueType ?? Query.MapType(value);
			if (DbValueMap.IsNull(value)) return QueryParameter.InputOutputParameter(name, DBNull.Value, valType);
			return QueryParameter.InputOutputParameter(name, value, valType);
		}
		public static QueryParameter InOut(string name, bool? value)
		{
			if (value == null || !value.HasValue) return QueryParameter.InputOutputParameter(name, DBNull.Value, SqlDbType.Bit);
			return QueryParameter.InputOutputParameter(name, value.Value, SqlDbType.Bit);
		}

		#region Other Parameters
		public static QueryParameter Out(string name, SqlDbType? valueType = null)
		{
			SqlDbType valType = valueType ?? SqlDbType.NVarChar;
			return QueryParameter.OutputParameter(name, valType);
		}
		public static QueryParameter Return(SqlDbType? valueType = null)
		{
			SqlDbType valType = valueType ?? SqlDbType.NVarChar;
			return QueryParameter.ReturnParameter("__returnValue", valType);
		}
		#endregion

		public enum Type
		{
			SQL,
			StoredProc
		}

		private static SqlParameter[] BuildSqlParameters(QueryParameter[] queryParameters)
		{
			var parameters = new List<SqlParameter>(queryParameters.Length);

			foreach (var qp in queryParameters)
			{
				if (qp.ParameterType == ParameterType.Output)
				{
					SqlParameter outParam = new SqlParameter(qp.Name, qp.Value);
					outParam.Direction = ParameterDirection.Output;
					outParam.Size = -1;
					outParam.SqlDbType = qp.ValueType;
					parameters.Add(outParam);
				}
				else if (qp.ParameterType == ParameterType.Return)
				{
					SqlParameter returnParam = new SqlParameter(qp.Name, SqlDbType.Int);
					returnParam.Direction = ParameterDirection.ReturnValue;
					returnParam.Size = -1;
					returnParam.SqlDbType = qp.ValueType;
					parameters.Add(returnParam);
				}
				else if (qp.ParameterType == ParameterType.InputOutput)
				{
					SqlParameter inOutParam = new SqlParameter(qp.Name, qp.Value);
					inOutParam.Direction = ParameterDirection.InputOutput;
					inOutParam.SqlDbType = qp.ValueType;

					if (qp.Value == DBNull.Value)
					{
						inOutParam.Size = -1;
					}

					parameters.Add(inOutParam);
				}
				else
				{
					// Handle special case of Table value parameters (built from a collection)
					if (qp.Value is ICollection<int>)
					{
						var tableParam = new SqlParameter(qp.Name, CollectionToDataTable(qp.Value as ICollection));
						tableParam.Direction = ParameterDirection.Input;
						tableParam.SqlDbType = SqlDbType.Structured;
						tableParam.TypeName = "dbo.IntegerListType";
						parameters.Add(tableParam);
					}
					else if (qp.Value is ICollection && qp.ValueType == SqlDbType.NVarChar)
					{
						var tableParam = new SqlParameter(qp.Name, CollectionToDataTable(qp.Value as ICollection));
						tableParam.Direction = ParameterDirection.Input;
						tableParam.SqlDbType = SqlDbType.Structured;
						tableParam.TypeName = "dbo.StringListType";
						parameters.Add(tableParam);
					}
					else
					{
						// Create a regular parameter
						SqlParameter parameter = new SqlParameter(qp.Name, qp.Value);
						parameter.Direction = ParameterDirection.Input;
						parameter.SqlDbType = qp.ValueType;
						parameters.Add(parameter);
					}
				}
			}

			return parameters.ToArray();
		}

		private static DataTable CollectionToDataTable(ICollection collection)
		{
			var dt = new DataTable();
			dt.Columns.Add("value");

			if (collection != null)
			{
				foreach (var value in collection)
				{
					var row = dt.NewRow();
					row["value"] = value;
					dt.Rows.Add(row);
				}
			}

			return dt;
		}
	}
}
