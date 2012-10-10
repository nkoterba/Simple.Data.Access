using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Simple.Data.Access.Test
{
    [TestFixture]
    public class SchemaProviderTest : BaseTests
    {
        [Test]
        public void NullConnectionProviderCausesException()
        {
            Assert.Throws<ArgumentNullException>(() => new AccessSchemaProvider(null));
        }

        [Test]
        public void ProceduresIsNotEmpty()
        {
            var provider = new AccessConnectionProvider();
            provider.SetConnectionString(_helper.ConnectionString);
            Assert.AreNotEqual(0, new AccessSchemaProvider(provider).GetStoredProcedures().Count());
        }
    }
}
