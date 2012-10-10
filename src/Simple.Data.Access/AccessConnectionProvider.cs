using System.ComponentModel.Composition;
using System.Data;
using System.Data.OleDb;
using Simple.Data.Ado;
using Simple.Data.Ado.Schema;

namespace Simple.Data.Access
{
	[Export(typeof (IConnectionProvider))]
	[Export("mdb", typeof (IConnectionProvider))]
	[Export("accdb", typeof (IConnectionProvider))]
	[Export("System.Data.OleDb", typeof (IConnectionProvider))]
	public class AccessConnectionProvider : IConnectionProvider
	{
		private string _connectionString;

		#region IConnectionProvider Members

		public void SetConnectionString(string connectionString)
		{
			_connectionString = connectionString;

			if (!_connectionString.StartsWith("Data Source="))
				_connectionString = "Data Source=" + _connectionString;

			// If the connection string does not have the OleDB Provider appended to it, prepend it
			if (!_connectionString.StartsWith("Provider"))
			{
				if (_connectionString.EndsWith("mdb"))
					_connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" + _connectionString;
				else
					_connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" + _connectionString;
			}
		}

		// Could do something here to check connection string for .mdb or .accdb and 
		// store the version of Access being accessed
		// .mdb would be 2000-2003 Access databases
		// .accdb would be 2007-2010 Access databases
		public IDbConnection CreateConnection()
		{
			return new OleDbConnection(_connectionString);
		}

		public string ConnectionString
		{
			get { return _connectionString; }
		}

		public string GetIdentityFunction()
		{
			// While MS Access does appear to have a global identity function (SELECT @@IDENTITY)
			// it does not appear to be something you could directly call or that
			// will work with the query statement found in the Simple.Data.ADO adapter
			// See: http://msdn.microsoft.com/en-us/library/ks9f57t0(v=VS.80).aspx
			// How to get command: http://support.microsoft.com/kb/815629

			// Will return null
			return null;
		}

		public IProcedureExecutor GetProcedureExecutor(AdoAdapter adapter, ObjectName procedureName)
		{
			return new AccessProcedureExectuor(adapter, procedureName);
		}

		public ISchemaProvider GetSchemaProvider()
		{
			return new AccessSchemaProvider(this);
		}

		// No BEGIN or END keywords found in SQL reference for MS Access
		// Assuming this means compound statements not supported
		// http://msdn.microsoft.com/en-us/library/bb259125(v=office.12).aspx
		public bool SupportsCompoundStatements
		{
			get { return false; }
		}

		// See: 2007+ http://msdn.microsoft.com/en-us/library/bb177892(v=office.12).aspx
		// See: 2003 http://office.microsoft.com/en-us/access-help/create-procedure-statement-HP001032219.aspx
		// Not sure how well Access supports Stored Procs
		public bool SupportsStoredProcedures
		{
			get { return true; }
		}

		#endregion
	}
}