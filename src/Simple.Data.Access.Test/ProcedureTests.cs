using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Simple.Data.Access.Test
{
	[TestFixture]
	internal class ProcedureTests : BaseTests
	{

		[Test]
		public void TestRetrieveProcedure()
		{
			var db = Database.OpenConnection(_helper.ConnectionString);
			var count = db.sp_SELECTUSERS.All().ToList<User>().Count;

			Assert.AreEqual(6, count);
		}

		[Test]
		public void TestRetrieveCountProcedureWithNoParams()
		{
			var db = Database.OpenConnection(_helper.ConnectionString);
			var rec = db.sp_COUNTUSERS.All().First();
			Assert.AreEqual(6, rec.NumUsers);
		}

		[Test]
		[Ignore("Access does not seem to like SELECT statement stored procedures with inputs parameters or I'm testing it incorrectly")]
		public void TestRetrieveCountProcedureWithParams()
		{
			var db = Database.OpenConnection(_helper.ConnectionString);
			var rec = db.sp_COUNTUSERSWITHAGE(23).First();
			Assert.AreEqual(6, rec.NumUsers);
		}

		[Test]
		public void TestDeleteProcedure()
		{
			var db = Database.OpenConnection(_helper.ConnectionString);
			var rec = db.sp_DELETEUSER(1);

			// Should only be 5 users left
			var count = db.Users.All().ToList().Count;
			Assert.AreEqual(5, count);
		}

		[Test]
		public void TestDeleteNameProcedure()
		{
			var db = Database.OpenConnection(_helper.ConnectionString);
			var rec = db.sp_DELETEUSER(inUserId: 1);

			// Should only be 5 users left
			var count = db.Users.All().ToList().Count;
			Assert.AreEqual(5, count);
		}

		[Test]
		public void TestUpdateProcedure()
		{
			var db = Database.OpenConnection(_helper.ConnectionString);

			var user = db.Users.FindById(1);
			Assert.NotNull(user);
			Assert.AreEqual(22, user.Age);

			var rec = db.sp_UPDATEUSER_AGE(1, 50);

			var user2 = db.Users.FindById(1);
			Assert.NotNull(user2);
			Assert.AreEqual(50, user2.Age);
		}

		[Test]
		public void TestUpdateNameProcedure()
		{
			var db = Database.OpenConnection(_helper.ConnectionString);

			var user = db.Users.FindById(1);
			Assert.NotNull(user);
			Assert.AreEqual(22, user.Age);

			var rec = db.sp_UPDATEUSER_AGE(inUserAge: 50, inUserID: 1);

			var user2 = db.Users.FindById(1);
			Assert.NotNull(user2);
			Assert.AreEqual(50, user2.Age);
		}

		[Test]
		public void TestCreateProcedure()
		{
			var db = Database.OpenConnection(_helper.ConnectionString);

			var rec = db.sp_ADDUSER_NAME_AGE("Mango", 55);

			var user2 = db.Users.FindByName("Mango");
			Assert.NotNull(user2);
			Assert.AreEqual(55, user2.Age);
		}

		[Test]
		public void TestCreateNameProcedure()
		{
			var db = Database.OpenConnection(_helper.ConnectionString);

			var rec = db.sp_ADDUSER_NAME_AGE(inUserAge: 55, inUserName: "Mango");

			var user2 = db.Users.FindByName("Mango");
			Assert.NotNull(user2);
			Assert.AreEqual(55, user2.Age);
		}
	}
}
   