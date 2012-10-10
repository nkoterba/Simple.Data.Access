using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Simple.Data.Ado;

namespace Simple.Data.Access.Test
{
    [TestFixture]
    class ConnectionProviderTest
    {
        [Test]
        public void AccessSupportsStoredProcedures()
        {
            IConnectionProvider target = new AccessConnectionProvider();
            Assert.IsTrue(target.SupportsStoredProcedures);
        }

        [Test]
        public void AccessDoesNotSupportCompoundStatements()
        {
            IConnectionProvider target = new AccessConnectionProvider();
            Assert.IsFalse(target.SupportsCompoundStatements);
        }
    }
}
