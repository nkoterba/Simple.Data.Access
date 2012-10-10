using System.Data;
using System.Data.OleDb;
using Simple.Data.Ado.Schema;

namespace Simple.Data.Access
{
	public class AccessColumn : Column
	{
		private readonly OleDbType _OleDbType;

		public AccessColumn(string actualName, Table table) : 
			base(actualName, table)
		{
		}

		public AccessColumn(string actualName, Table table, OleDbType oleDbType) : 
			base(actualName, table, OleDbToDbTypeConverter.GetDbType(oleDbType))
		{
			_OleDbType = oleDbType;
		}

		public AccessColumn(string actualName, Table table, bool isIdentity) : 
			base(actualName, table, isIdentity)
		{
		}

		public AccessColumn(string actualName, Table table, bool isIdentity, OleDbType oleDbType, int maxLength)
			: base(actualName, table, isIdentity, OleDbToDbTypeConverter.GetDbType(oleDbType), maxLength)
		{
			_OleDbType = oleDbType;
		}

		public OleDbType OleDbType
		{
			get { return _OleDbType; }
		}


		public override bool IsBinary
		{
			get
			{
				return OleDbType == OleDbType.Binary || OleDbType == OleDbType.Boolean ||
				       OleDbType == OleDbType.VarBinary;
			}
		}
	}
}