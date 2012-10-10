using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace Simple.Data.Access
{
	public class OleDbToDbTypeConverter
	{
		// Type mappings: http://msdn.microsoft.com/en-us/library/yy6y35y8%28v=vs.110%29.aspx
		private static readonly Dictionary<OleDbType, DbType> typeLookup = new Dictionary<OleDbType, DbType>
																																	{
																																		{OleDbType.Boolean, DbType.Boolean},
																																		{OleDbType.UnsignedTinyInt, DbType.Byte},
																																		{OleDbType.VarBinary, DbType.Binary},
																																		// OleDbType.Char unmatched according to link above
																																		//{OleDbType.Char, ?}
																																		{OleDbType.DBTimeStamp, DbType.DateTime},
																																		{OleDbType.Decimal, DbType.Decimal},
																																		{OleDbType.Double, DbType.Double},
																																		{OleDbType.Single, DbType.Single},
																																		// Make Guid actually AnsiString
																																		// {OleDbType.Guid, DbType.Guid},
																																		{OleDbType.Guid, DbType.AnsiString},
																																		{OleDbType.SmallInt, DbType.Int16},
																																		{OleDbType.Integer, DbType.Int32},
																																		{OleDbType.BigInt, DbType.Int64},
																																		{OleDbType.Variant, DbType.Object},
																																		{OleDbType.VarWChar, DbType.String},
																																		{OleDbType.UnsignedSmallInt, DbType.UInt16},
																																		{OleDbType.UnsignedInt, DbType.UInt32},
																																		{OleDbType.UnsignedBigInt, DbType.UInt64},
																																		{OleDbType.VarChar, DbType.AnsiString},
																																		{OleDbType.Char, DbType.AnsiStringFixedLength},
																																		{OleDbType.Currency, DbType.Currency},
																																		{OleDbType.DBDate, DbType.Date},
																																		{OleDbType.TinyInt, DbType.SByte},
																																		{OleDbType.WChar, DbType.StringFixedLength},
																																		{OleDbType.DBTime, DbType.Time}
																																	};

		/// <summary>
		/// Returns the matching DBType for this OleDBtype
		/// </summary>
		/// <param name="type">OleDbType to convert</param>
		/// <returns>Corresponding DbType</returns>
		public static DbType GetDbType(OleDbType type)
		{
			if (!typeLookup.ContainsKey(type))
				return default(DbType); // Default is ANSIString = 0

			return typeLookup[type];
		}

		/// <summary>
		/// Returns the matching OleDbType for this DbType
		/// </summary>
		/// <param name="type">DbType to convert</param>
		/// <returns>Corresponding OleDbType</returns>
		public static OleDbType GetOleDbType(DbType type)
		{
			// Get value out of dictionary, then find corresponding key
			var val = typeLookup.Values.Single(v => v == type);

			// Return key that matches val
			return typeLookup.Keys.Single(k => typeLookup[k] == val);
		}
	}
}
