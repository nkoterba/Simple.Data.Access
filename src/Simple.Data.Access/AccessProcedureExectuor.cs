using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using Simple.Data.Ado;
using Simple.Data.Ado.Schema;
using ResultSet = System.Collections.Generic.IEnumerable<System.Collections.Generic.IDictionary<string, object>>;

namespace Simple.Data.Access
{
	internal class AccessProcedureExectuor : IProcedureExecutor
	{
		private readonly AdoAdapter _adapter;
		private readonly ObjectName _procedureName;

		public AccessProcedureExectuor(AdoAdapter adapter, ObjectName procedureName)
		{
			_adapter = adapter;
			_procedureName = procedureName;
		}

		#region IProcedureExecutor Members

		public IEnumerable<ResultSet> Execute(
			IDictionary<string, object> suppliedParameters)
		{
			Procedure procedure =
				DatabaseSchema.Get(_adapter.ConnectionProvider, _adapter.ProviderHelper).FindProcedure(_procedureName);
			if (procedure == null)
				throw new UnresolvableObjectException(_procedureName.ToString());

			bool executeReader = procedure.Parameters.Where(p => p.Direction == ParameterDirection.InputOutput ||
			                                                     p.Direction == ParameterDirection.Output ||
			                                                     p.Direction == ParameterDirection.ReturnValue).Count() >
			                     0;

			using (IDbConnection conn = _adapter.ConnectionProvider.CreateConnection())
			{
				conn.Open();
				using (IDbCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = procedure.QualifiedName;
					cmd.CommandType = CommandType.StoredProcedure;
					AddCommandParameters(procedure, cmd, suppliedParameters);
					try
					{
						IEnumerable<ResultSet> result = Enumerable.Empty<ResultSet>();

						cmd.WriteTrace();
						if (executeReader)
						{
							using (IDataReader rdr = cmd.ExecuteReader())
							{
								//var rdrAdvanced = RetrieveReturnValue(procedure, rdr, suppliedParameters);
								//rdrAdvanced = RetrieveOutputParameterValues(procedure, rdr, suppliedParameters, rdrAdvanced);

								//if (!rdrAdvanced)
								//{
								//    result = rdr.ToMultipleDictionaries();
								//}
							}
						}
						else
						{
							cmd.ExecuteNonQuery();
						}

						return result;
					}
					catch (DbException ex)
					{
						throw new AdoAdapterException(ex.Message, cmd);
					}
				}
			}
		}

		public IEnumerable<IEnumerable<IDictionary<string, object>>> Execute(
			IDictionary<string, object> suppliedParameters, IDbTransaction transaction)
		{
			throw new NotImplementedException();
		}

		#endregion

		private void AddCommandParameters(Procedure procedure, IDbCommand cmd, IDictionary<string, object> suppliedParameters)
		{
			foreach (var parameter in procedure.Parameters
				.Where(param => param.Direction == ParameterDirection.Input ||
				                param.Direction == ParameterDirection.InputOutput)
				.Select((value, index) => new {Parameter = value, Index = index}))
			{
				string name = parameter.Parameter.Name;
				object value;

				if (String.IsNullOrEmpty(name) || !suppliedParameters.TryGetValue(name, out value))
				{
					name = String.Concat("_", parameter.Index);
					if (!suppliedParameters.TryGetValue(name, out value))
					{
						throw new SimpleDataException(String.Format("Could not find a value for parameter index {0} named {1}",
						                                            parameter.Index, parameter.Parameter.Name));
					}
				}

				// No need to use parameter names here.  Position is what is important.
				cmd.Parameters.Add(new OleDbParameter
				                   	{
				                   		ParameterName = name,
				                   		Value = value
				                   	});
			}
		}
	}
}