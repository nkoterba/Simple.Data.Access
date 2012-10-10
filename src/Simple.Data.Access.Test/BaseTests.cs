using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Simple.Data.Access.Test
{
    /// <summary>
    /// Summary description for FindTests
    /// </summary>
    [TestFixture]
    public abstract class BaseTests
    {
        protected DatabaseHelper _helper;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _helper = new DatabaseHelper(AccessDatabaseType.Access2003);
        }

        [SetUp]
        public void SetUp()
        {
            _helper.Reset();
        }
    }
}
