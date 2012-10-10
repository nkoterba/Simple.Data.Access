using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using Simple.Data.Ado;
using Simple.Data.Ado.Schema;

namespace Simple.Data.Access
{
	public class AccessSchemaProvider : ISchemaProvider
	{
		private const int COLUMN_FLAGS_PRIMARY_KEY_VALUE = 90;
		private readonly IConnectionProvider _connectionProvider;

		public AccessSchemaProvider(IConnectionProvider connectionProvider)
		{
			if (connectionProvider == null)
				throw new ArgumentNullException("connectionProvider");

			_connectionProvider = connectionProvider;
		}

		public IConnectionProvider ConnectionProvider
		{
			get { return _connectionProvider; }
		}

		#region ISchemaProvider Members

		public IEnumerable<Table> GetTables()
		{
			EnumerableRowCollection<Table> tables = GetOleDbSchema(OleDbSchemaGuid.Tables).AsEnumerable().Select(SchemaRowToTable);
			return tables;
		}

		// How to determine values returned in COLUMN_FLAGS table: http://msdn.microsoft.com/en-us/library/ms716934(VS.85).aspx
		// More background info: http://social.msdn.microsoft.com/Forums/en-US/adodotnetdataproviders/thread/3fa2bc10-8720-49db-9d5d-0fa9554b65fe/
		// Primary key is determined by DATA_TYPE being integer and COLUMN_FLAGS = 90
		public IEnumerable<Column> GetColumns(Table table)
		{
			return Enumerable.Select(GetColumnsDataTable(table).AsEnumerable(),
			                         row =>
			                         new AccessColumn(row["COLUMN_NAME"].ToString(),
			                                          table,
			                                          Convert.ToInt32(row["COLUMN_FLAGS"]) == COLUMN_FLAGS_PRIMARY_KEY_VALUE &&
			                                          (OleDbType) (Convert.ToInt32(row["DATA_TYPE"])) == OleDbType.Integer,
			                                          (OleDbType) (Convert.ToInt32(row["DATA_TYPE"])),
			                                          row["CHARACTER_MAXIMUM_LENGTH"] == DBNull.Value
			                                          	? 0
			                                          	: Convert.ToInt32(row["CHARACTER_MAXIMUM_LENGTH"])));
		}

		// Stored procedures that return a 'view' of data are treated like views/tables
		//
		public IEnumerable<Procedure> GetStoredProcedures()
		{
			return GetOleDbSchema(OleDbSchemaGuid.Procedures)
				.AsEnumerable()
				.Select(
					row =>
						{
							var procedure = new Procedure(row["PROCEDURE_NAME"].ToString(), row["PROCEDURE_NAME"].ToString(),
							                              GetDefaultSchema());
							String procDef = row["PROCEDURE_DEFINITION"].ToString();

							return procedure;
						});
		}

		public IEnumerable<Parameter> GetParameters(Procedure storedProcedure)
		{
			return
				ExtractParameters(
					GetOleDbSchema(OleDbSchemaGuid.Procedures, null, null, storedProcedure.Name).AsEnumerable().First()[
						"PROCEDURE_DEFINITION"].ToString());
		}

		public string QuoteObjectName(string unquotedName)
		{
			if (unquotedName == null) throw new ArgumentNullException("unquotedName");
			if (unquotedName.StartsWith("[")) return unquotedName;
			return string.Concat("[", unquotedName, "]");
		}

		public string NameParameter(string baseName)
		{
			if (baseName == null) throw new ArgumentNullException("baseName");
			if (baseName.Length == 0) throw new ArgumentException("Base name must be provided");
			return (baseName.StartsWith("@")) ? baseName : "@" + baseName;
		}

		public string GetDefaultSchema()
		{
			return String.Empty;
		}

		public Key GetPrimaryKey(Table table)
		{
			if (table == null) throw new ArgumentNullException("table");
			return new Key(GetPrimaryKeys(table.ActualName).AsEnumerable()
			               	.Where(
			               		row =>
			               		AreEqual(row["TABLE_SCHEMA"], table.Schema) && row["TABLE_NAME"].ToString() == table.ActualName)
			               	.OrderBy(row => Convert.ToInt32(row["ORDINAL"]))
			               	.Select(row => row["COLUMN_NAME"].ToString()));
		}

		public IEnumerable<ForeignKey> GetForeignKeys(Table table)
		{
			if (table == null) throw new ArgumentNullException("table");
			List<IGrouping<string, DataRow>> groups = GetForeignKeys(table.ActualName)
				.Where(row => AreEqual(row["TABLE_SCHEMA"], table.Schema)
				              && row["TABLE_NAME"].ToString() == table.ActualName)
				.GroupBy(row => row["CONSTRAINT_NAME"].ToString())
				.ToList();

			foreach (var group in groups)
			{
				yield return new ForeignKey(new ObjectName(group.First()["TABLE_SCHEMA"], group.First()["TABLE_NAME"]),
				                            group.Select(row => row["COLUMN_NAME"].ToString()),
				                            new ObjectName(group.First()["UNIQUE_TABLE_SCHEMA"], group.First()["UNIQUE_TABLE_NAME"]),
				                            group.Select(row => row["UNIQUE_COLUMN_NAME"].ToString()));
			}
		}

		#endregion

		private static Table SchemaRowToTable(DataRow row)
		{
			return new Table(row["TABLE_NAME"].ToString(), row["TABLE_SCHEMA"].ToString(),
			                 row["TABLE_TYPE"].ToString().Contains("TABLE") ? TableType.Table : TableType.View);
		}

		private IEnumerable<Parameter> ExtractParameters(string procDefinition)
		{
			var retVal = new List<Parameter>();

			// Sample format
			// PARAMETERS inUserID Long, inUserAge Long; UPDATE Users SET Age = inUserAge WHERE ID = inUserID;
			// PARAMETERS inUserName Text ( 40 ), inUserAge Long; INSERT INTO Users ( Name, Age )\r\nVALUES (inUserName, inUserAge);

			int paramEnd = procDefinition.IndexOf(';');

			string[] parameters = procDefinition.Substring(11, paramEnd - 11).Split(',');

			for (int i = 0; i < parameters.Length; i++)
			{
				string[] paramParts = parameters[i].Trim().Split(' ');

				string paramName = paramParts[0];
				string paramType = paramParts[1].ToLower();

				Type t = DataTypeToClrType(paramType);

				var p = new Parameter(paramName, t, ParameterDirection.Input);

				retVal.Add(p);
			}

			return retVal;
		}

		private static bool AreEqual(object string1, object string2)
		{
			if (string1 == DBNull.Value) string1 = null;
			if (string2 == DBNull.Value) string2 = null;
			if (string1 == null && string2 == null) return true;
			if (string1 == null || string2 == null) return false;
			return string1.ToString().Trim().Equals(string2.ToString().Trim());
		}

		public Type DataTypeToClrType(string dataType)
		{
			return AccessTypeResolver.GetClrType(dataType);
		}

		private DataTable GetColumnsDataTable(Table table)
		{
			return GetOleDbSchema(OleDbSchemaGuid.Columns, null, null, table.ActualName);
		}

		private DataTable GetPrimaryKeys()
		{
			return GetOleDbSchema(OleDbSchemaGuid.Primary_Keys);
		}

		private DataTable GetPrimaryKeys(string tableName)
		{
			return GetOleDbSchema(OleDbSchemaGuid.Primary_Keys, null, null, tableName);
		}

		private DataTable GetForeignKeys()
		{
			return GetOleDbSchema(OleDbSchemaGuid.Foreign_Keys);
		}

		private EnumerableRowCollection<DataRow> GetForeignKeys(string tableName)
		{
			return GetOleDbSchema(OleDbSchemaGuid.Foreign_Keys, null, null, tableName).AsEnumerable();
		}

		// OleDbSchemaGuid Class http://msdn.microsoft.com/en-us/library/fk1yszdc.aspx
		// Retrieving schema http://support.microsoft.com/kb/309488
		// Values returned http://msdn.microsoft.com/en-us/library/cc668764.aspx
		private DataTable GetOleDbSchema(Guid guid, params Object[] restrictions)
		{
			DataTable dataTable;
			using (var cn = ConnectionProvider.CreateConnection() as OleDbConnection)
			{
				cn.Open();
				dataTable = cn.GetOleDbSchemaTable(guid, restrictions);
			}

			return dataTable;
		}

		private IEnumerable<DataRow> GetSchema(string collectionName, params string[] constraints)
		{
			DataTable dataTable;
			using (IDbConnection cn = ConnectionProvider.CreateConnection())
			{
				cn.Open();
				dataTable = cn.GetSchema(collectionName, constraints);
			}

			return dataTable.AsEnumerable();
		}

		#region Nested type: DBCOLUMNFLAGS

		/// <summary>
		/// This enumeration represents the bitmask values of the COLUMN_FLAGS value used below.
		/// From: http://sources.team-mediaportal.com/svn/public/trunk/Common-MP-TVE3/External/Gentle.NET/Source/Gentle.Provider.Jet/JetAnalyzer.cs
		/// And
		/// http://www.geekpedia.com/Thread47448_Where-Are-The-Constants-Of-COLUMNFLAGS-enumerated-Type-Defined.html
		/// And
		/// http://www.imiscommunity.com/imiscommunity/docs/iMISNET/Asi.Data.MappingType.DbColumnFlags.html
		/// </summary>
		[Flags]
		private enum DBCOLUMNFLAGS
		{
			ISBOOKMARK = 0x1, // 1
			MAYDEFER = 0x2, // 2
			WRITE = 0x4, // 4
			WRITEUNKNOWN = 0x8, // 8
			ISFIXEDLENGTH = 0x10, // 16
			ISNULLABLE = 0x20, // 32
			MAYBENULL = 0x40, // 64
			ISLONG = 0x80, // 128
			ISROWID = 0x100,
			ISROWVER = 0x200,
			CACHEDEFERRED = 0x1000,
			SCALEISNEGATIVE = 0x4000,
			RESERVED = 0x8000,
			ISROWURL = 0x10000,
			ISDEFAULTSTREAM = 0x20000,
			ISCOLLECTION = 0x40000,
			ISSTREAM = 0x80000,
			ISROWSET = 0x100000,
			ISROW = 0x200000,
			ROWSPECIFICCOLUMN = 0x400000
		}

		#endregion
	}
}