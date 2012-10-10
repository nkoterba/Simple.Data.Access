using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Simple.Data.Access.Test
{
    [TestFixture]
    public class Database2003OpenerTests : BaseTests
    {
        [Test]
        public void TestOpenWithDefaultConnection()
        {
            var db = Database.Open();
            Assert.IsNotNull(db);

            var count = db.Users.All().ToList<User>().Count;
            Assert.AreEqual(count, 6);
        }

        [Test]
        public void TestOpenNamedConnection()
        {
            var db = Database.OpenNamedConnection("Access2003");
            Assert.IsNotNull(db);

            var count = db.Users.All().ToList<User>().Count;
            Assert.AreEqual(count, 6);
        }

        [Test]
        public void TestOpenConnection()
        {
            var db = Database.OpenConnection(Properties.Settings.Default.Access2003ConnectionString);
            Assert.IsNotNull(db);

            var count = db.Users.All().ToList<User>().Count;
            Assert.AreEqual(count, 6);
        }

        [Test]
        public void TestOpenFile()
        {
            var db = Database.OpenFile(@"..\..\Data\Access2003TestDatabase.mdb");
            Assert.IsNotNull(db);

            var count = db.Users.All().ToList<User>().Count;
            Assert.AreEqual(count, 6);
        }
    }
}
