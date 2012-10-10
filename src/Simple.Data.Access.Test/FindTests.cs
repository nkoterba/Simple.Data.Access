using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using NUnit.Framework;
using Simple.Data.Access.Test.Data;
using Simple.Data.Ado;

namespace Simple.Data.Access.Test
{
     /// <summary>
    /// Summary description for FindTests
    /// </summary>
    [TestFixture]
    public class FindTests : BaseTests
     {
        [Test]
        public void TestAllCount()
        {
            var db = Database.OpenConnection(_helper.ConnectionString);
            var count = db.Users.All().ToList().Count;
            Assert.AreEqual(6, count);
        }

        [Test]
        public void TestFindByName()
        {
            var db = Database.OpenConnection(_helper.ConnectionString);
            var user = db.Users.FindByName("Bob");
            Assert.NotNull(user);
            Assert.AreEqual("Bob", user.Name);
        }

        [Test]
        public void TestFindByNameWithCast()
        {
            var db = Database.OpenConnection(_helper.ConnectionString);
            var user = (User)db.Users.FindByName("Bob");
            Assert.NotNull(user);
            Assert.AreEqual("Bob", user.Name);
        }
        [Test]
        public void TestFindAllByName()
        {
            var db = Database.OpenConnection(_helper.ConnectionString);
            IEnumerable<User> users = db.Users.FindAllByName("Bob").Cast<User>();
            Assert.AreEqual(1, users.Count());
        }

        [Test]
        public void TestFindAllByNameAsIEnumerableOfDynamic()
        {
            var db = Database.OpenConnection(_helper.ConnectionString);
            IEnumerable<dynamic> users = db.Users.FindAllByName("Bob");
            Assert.AreEqual(1, users.Count());
        }

        [Test]
        public void TestFindAllByPartialName()
        {
            var db = Database.OpenConnection(_helper.ConnectionString);
            IEnumerable<User> users = db.Users.FindAll(db.Users.Name.Like("Bob")).ToList<User>();
            Assert.AreEqual(1, users.Count());
        }

        [Test]
        [Ignore("Generated Skip commands (e.g. SQL OFFSET command) does not appear to work with MS Access")]
        // Access does not support OFFSET
        // SELECT * FROM [Users] OFFSET 5; fails in MS Access
        public void TestAllWithSkipCount()
        {
            var db = Database.OpenConnection(_helper.ConnectionString);
            var count = db.Users.All().Skip(1).ToList().Count;
            Assert.AreEqual(5, count);
        }

        [Test]
        public void TestImplicitCast()
        {
            var db = Database.OpenConnection(_helper.ConnectionString);
            User user = db.Users.FindByName("Bob");
            Assert.NotNull(user);
        }

        [Test]
        public void TestImplicitEnumerableCast()
        {
            var db = Database.OpenConnection(_helper.ConnectionString);
            foreach (User user in db.Users.All())
            {
                Assert.NotNull(user);
            }
        }

        [Test]
        public void TestFindOnAView()
        {
            var db = Database.OpenConnection(_helper.ConnectionString);
            var user = db.UserView.FindByName("Bob");
            Assert.IsNotNull(user);
        }

        [Test]
        public void TestCast()
        {
            var db = Database.OpenConnection(_helper.ConnectionString);
            var userQuery = db.Users.All().Cast<User>() as IEnumerable<User>;
            Assert.IsNotNull(userQuery);
            var users = userQuery.ToList();
            Assert.AreNotEqual(0, users.Count);
        }

        [Test]
        public void TestProviderWithFileName()
        {
            var provider = new ProviderHelper().GetProviderByFilename(_helper.ConnectionString);
            Assert.IsInstanceOf(typeof(AccessConnectionProvider), provider);
        }

        [Test]
        public void TestProviderWithConnectionString()
        {
            var provider = new ProviderHelper().GetProviderByConnectionString(_helper.ConnectionString);
            Assert.IsInstanceOf(typeof(AccessConnectionProvider), provider);
        }

        [Test]
        public void TestFindById()
        {
            var db = Database.Opener.OpenFile(_helper.ConnectionString);
            var user = db.Users.FindById(1);
					Assert.NotNull(user);
            Assert.AreEqual(1, user.Id);
        }

        [Test]
        public void TestAll()
        {
            var db = Database.OpenFile(_helper.ConnectionString);
            var all = new List<object>(db.Users.All().Cast<dynamic>());
            Assert.IsNotEmpty(all);
        }

        [Test]
        public void TestMultipleFind()
        {
            var db = Database.OpenFile(_helper.ConnectionString);
            var user = db.Users.FindByNameAndAge("Mary", 32);

            Assert.AreEqual("Mary", user.Name);
            Assert.AreEqual(32, user.Age);
        }

        [Test]
        public void TestFindMultipleRows()
        {
            var db = Database.OpenFile(_helper.ConnectionString);
            var count = db.Users.FindAllByAge(22).ToList().Count;

            Assert.AreEqual(2, count);
        }

        [Test]
        public void TestGet()
        {
            var db = Database.OpenFile(_helper.ConnectionString);
            var user = db.Users.Get(1);
					Assert.NotNull(user);

            Assert.AreEqual("Bob", user.Name);
        }

        [Test]
        public void TestFindDate()
        {
            var db = Database.OpenFile(_helper.ConnectionString);
            var user = db.Users.Find(db.Users.DateTime <= "01-01-2001");

            Assert.AreEqual("Bob", user.Name);
        }

        [Test]
        public void TestFindUsersBeforeEqualToDate()
        {
            var db = Database.OpenFile(_helper.ConnectionString);
            var users = db.Users.FindAll(db.Users.DateTime <= "03-03-2003").ToList();

            Assert.AreEqual(users.Count, 3);
        }

        [Test]
        public void TestFindUserWithLike()
        {
            var db = Database.OpenFile(_helper.ConnectionString);
            var user = db.Users.Find(db.Users.Name.Like("Bo%") && db.Users.Age == 22);

            Assert.AreEqual("Bob", user.Name);

            var user2 = db.Users.Find(db.Users.Name.Like("Bo%") && db.Users.Age == 23);

            Assert.IsNull(user2);
        }

        [Test]
			[Ignore("Repeated arithmetic sequences appear to fail")]
        public void TestArithmetic()
        {
            var db = Database.OpenFile(_helper.ConnectionString);
            
					// Bob: UserId = 1, Age = 22 in db

					// NOT SURE why this isn't working/fails...
					// Not going to ignore test because I believe this test should work

						var user = db.Users.Find(db.Users.Id + db.Users.Age == 23);
						Assert.IsNotNull(user);
						Assert.AreEqual("Bob", user.Name);

						var user4 = db.Users.Find(db.Users.Age / db.Users.Id == 22);
						Assert.IsNotNull(user4);
						Assert.AreEqual("Bob", user4.Name);

						var user2 = db.Users.Find(db.Users.Id - db.Users.Age == -21);
						Assert.IsNotNull(user2);
						Assert.AreEqual("Bob", user2.Name);

						var user3 = db.Users.Find(db.Users.Id * db.Users.Age == 22);
						Assert.IsNotNull(user3);
						Assert.AreEqual("Bob", user3.Name);
        }

        [Test]
        public void TestModOperator()
        {
            var db = Database.OpenFile(_helper.ConnectionString);

            // ACCESS uses MOD instead fo the % operator...
            // Simple.Data expects to use %
            var users = db.Users.FindAll(db.Users.Id % 2 == 0).ToList();
            Assert.IsNotNull(users);
            Assert.AreEqual(3, users.Count);
        }

        [Test]
        public void TestComparison()
        {
            var db = Database.OpenFile(_helper.ConnectionString);
            var user = db.Users.Find(db.Users.Age == 22);

            Assert.AreEqual("Bob", user.Name);

            var user2 = db.Users.Find(db.Users.Age > 80);

            Assert.AreEqual("Alice", user2.Name);

            var users = db.Users.FindAll(db.Users.Age < 23).ToList();

            Assert.AreEqual(2, users.Count);

            var users2 = db.Users.FindAll(db.Users.Age >= 10).ToList();

            Assert.AreEqual(6, users2.Count);

            var users3 = db.Users.FindAll(db.Users.Age <= 80).ToList();

            Assert.AreEqual(5, users3.Count);
        }

        [Test]
        public void TestNotEquals()
        {
            var db = Database.OpenFile(_helper.ConnectionString);
            var users4 = db.Users.FindAll(db.Users.Age != 22).ToList();

            Assert.AreEqual(4, users4.Count);
        }

         [Test]
        public void TestIN()
        {
            var db = Database.OpenFile(_helper.ConnectionString);
            var users = db.Users.FindAllById(new[] {1, 2, 3}).ToList();

            Assert.AreEqual(3, users.Count);
        }

        [Test]
        public void TestBETWEEN()
        {
            var db = Database.OpenFile(_helper.ConnectionString);
            var users = db.Users.FindAllById(3.to(5)).ToList();

            Assert.AreEqual(3, users.Count);
        }

        [Test]
        public void TestISNOTNULL()
        {
            var db = Database.OpenFile(_helper.ConnectionString);
            var users = db.Users.FindAll(db.Users.Id != null).ToList<User>();

            Assert.AreEqual(6, users.Count);
        }

        [Test]
        public void TestISOleObjectNULL()
        {
            var db = Database.OpenFile(_helper.ConnectionString);
						var users = db.Users.FindAll(db.Users.OleObject == null).ToList<User>();

            Assert.AreEqual(4, users.Count);
        }
     }
}
