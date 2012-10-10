using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Simple.Data.Ado;

namespace Simple.Data.Access
{
	[Export(typeof(Operators))]
	public class AccessOperators : Operators
	{
		/// <summary>
		/// Gets Access' unique not equal operator '&lt;&gt;'
		/// </summary>
		public override string NotEqual
		{
			get { return "<>"; }
		}

		/// <summary>
		/// Gets Access' unique Modulo operator 'MOD'
		/// </summary>
		public override string Modulo
		{
			get { return "MOD"; }
		}
	}
}
