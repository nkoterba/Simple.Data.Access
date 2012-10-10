using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Dynamic;
using System.Linq;
using NUnit.Framework;
using Simple.Data.Access.Test.Data;
using Simple.Data.Ado;

namespace Simple.Data.Access.Test
{
    /// <summary>
    /// Summary description for InsertTests
    /// </summary>
    [TestFixture]
    public class InsertTests : BaseTests
    {

			[Test]
			public void TestInsert()
			{
				var db = Database.OpenFile(_helper.ConnectionString);

				db.Users.Insert(Name: "Susanna", LongText: "foo", Age: 29, YesNo: true);

				var user = db.Users.FindByName("Susanna");
				Assert.AreEqual("Susanna", user.Name);
				Assert.AreEqual("foo", user.LongText);
				Assert.AreEqual(29, user.Age);
			}

			[Test]
			public void TestInsertBoolAsNumber()
			{
				var db = Database.OpenFile(_helper.ConnectionString);

				db.Users.Insert(Name: "Susanna", LongText: "foo", Age: 29, YesNo: 1);

				var user = db.Users.FindByName("Susanna");
				Assert.AreEqual("Susanna", user.Name);
				Assert.AreEqual("foo", user.LongText);
				Assert.AreEqual(29, user.Age);
			}

			[Test]
			public void TestInsertBooleanAsNumber()
			{
				var db = Database.OpenFile(_helper.ConnectionString);

				db.Users.Insert(Name: "Susanna", LongText: "foo", Age: 29, YesNo: 1);

				var user = db.Users.FindByName("Susanna");
				Assert.AreEqual("Susanna", user.Name);
				Assert.AreEqual("foo", user.LongText);
				Assert.AreEqual(29, user.Age);
			}

        [Test]
        public void TestInsertWithNamedArguments()
        {
            var db = Database.OpenConnection(_helper.ConnectionString);

            db.Users.Insert(Name: "Ford", LongText: "hoopy", Age: 29);

            var user = db.Users.FindBy(Name: "Ford", LongText: "hoopy", Age: 29);

            Assert.IsNotNull(user);
            Assert.AreEqual("Ford", user.Name);
            Assert.AreEqual("hoopy", user.LongText);
            Assert.AreEqual(29, user.Age);
        }

        [Test]
        public void TestInsertWithStaticTypeObject()
        {
            var db = Database.OpenConnection(_helper.ConnectionString);

            var user = new User
                           {
                               Name = "Zaphod",
                               LongText = "zarquon",
                               Age = 42,
                               DateTime = DateTime.Now,
                               Byte = 1,
                               Currency = 1,
                               Decimal = 1,
                               Double = 1,
                               Hyperlink = "hyper",
                               Integer = 1,
                               LongInteger = 1,
                               Single = 1,
                               //YesNo = false,
                               ReplicationID = Guid.NewGuid().ToString(),
                               OleObject = "new oleObject"
                           };
            db.Users.Insert(user);

            var actual = db.Users.FindByName("Zaphod");

            Assert.IsNotNull(actual);
            Assert.AreEqual("Zaphod", actual.Name);
            Assert.AreEqual("zarquon", actual.LongText);
            Assert.AreEqual(42, actual.Age);
        }

        [Test]
        public void TestMultiInsertWithStaticTypeObjects()
        {
            var db = Database.OpenConnection(_helper.ConnectionString);

            var users = new[]
                            {
                                new User { Name = "Slartibartfast", LongText = "bistromathics", Age = 777 },
                                new User { Name = "Wowbagger", LongText = "teatime", Age = int.MaxValue }
                            };

            db.Users.Insert(users);

            IList<User> actuals = db.Users.FindAllByName(new[] {"Slartibartfast", "Wowbagger"}).ToList<User>();

            Assert.AreEqual(2, actuals.Count);
            Assert.AreNotEqual(0, actuals[0].Id);
            Assert.AreEqual("Slartibartfast", actuals[0].Name);
            Assert.AreEqual("bistromathics", actuals[0].LongText);
            Assert.AreEqual(777, actuals[0].Age);

            Assert.AreNotEqual(0, actuals[1].Id);
            Assert.AreEqual("Wowbagger", actuals[1].Name);
            Assert.AreEqual("teatime", actuals[1].LongText);
            Assert.AreEqual(int.MaxValue, actuals[1].Age);
        }

        [Test]
        public void TestInsertWithDynamicTypeObject()
        {
            var db = Database.OpenConnection(_helper.ConnectionString);

            dynamic user = new ExpandoObject();
            user.Name = "Marvin";
            user.LongText = "diodes";
            user.Age = 42000000;

            db.Users.Insert(user);

            var actual = db.Users.FindByAge(42000000);

            Assert.IsNotNull(user);
            Assert.AreEqual("Marvin", actual.Name);
            Assert.AreEqual("diodes", actual.LongText);
            Assert.AreEqual(42000000, actual.Age);
        }

        [Test]
        public void TestMultiInsertWithDynamicTypeObjects()
        {
            var db = Database.OpenConnection(_helper.ConnectionString);

            dynamic user1 = new ExpandoObject();
            user1.Name = "Prak";
            user1.LongText = "truth";
            user1.Age = 30;

            dynamic user2 = new ExpandoObject();
            user2.Name = "Eddie";
            user2.LongText = "tea";
            user2.Age = 1;

            var users = new[] { user1, user2 };

            db.Users.Insert(users);

            IList<dynamic> actuals = db.Users.FindAllByLongText(new[] {"truth", "tea"} ).ToList();

            Assert.AreEqual(2, actuals.Count);
            Assert.AreNotEqual(0, actuals[0].Id);
            Assert.AreEqual("Prak", actuals[0].Name);
            Assert.AreEqual("truth", actuals[0].LongText);
            Assert.AreEqual(30, actuals[0].Age);

            Assert.AreNotEqual(0, actuals[1].Id);
            Assert.AreEqual("Eddie", actuals[1].Name);
            Assert.AreEqual("tea", actuals[1].LongText);
            Assert.AreEqual(1, actuals[1].Age);
        }

        [Test]
        public void InsertBigStringIntoNTextColumn()
        {
            var bigString = new string('X', 8192);
            var db = Database.OpenConnection(_helper.ConnectionString);
            db.Users.Insert(LongText: bigString);

            var row = db.Users.FindByLongText(bigString);

            Assert.NotNull(row);
            Assert.AreNotEqual(0, row.Id);
            Assert.AreEqual(bigString, row.LongText);
        }
    }
}
