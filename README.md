#Simple.Data.Access
==================

Microsoft Access Provider for Simple.Data.

Tested and works with Microsoft Access 2000, 2003, and 2007 databases.

Supports .mdb and .accdb file types.

Supports standard Simple.Data query syntax for tables and views.

**DOES NOT** support compound statements (Access limitation).

**DOES NOT** support SQL OFFSET/Simple.Data 'Skip()' command (Access limitation)

**NOTE**: Current Simple.Data.Access.Test uses Access2003 database for unit tests.  Tests for Access2007 can be run by changing the database type in the BaseTest.cs class.  **HOWEVER**, current code to 'compact' and reset primary key for Users table **DOES NOT WORK** for Access 2007 database so tests involving checking for specific Ids (e.g. Id = 1) will fail.  Will try to use different tests or fix script to properly reset auto-numbers in Access 2007 during each test setup.

Tested support for **BASIC** stored procedures. Following stored procedures were tested:
* SELECT-based stored procs with NO INPUT parameters (INPUT parameters may be possible but right now test fails).
* DELETE-based stored procs with INPUT parameters
* UPDATE-based stored procs with INPUT parameters
* CREATE-based stored procs with INPUT parameters

**NOTE**: Access treats stored procedures that return data (e.g. SELECT-based stored procs) as if they were Database Views.  This **DEVIATES** from standard Simple.Data documentation.

For example:

Basic Stored Procedure:

CREATE PROCEDURE ProcedureWithoutParameters 
AS
SELECT * FROM Customers

Normal Way to Call it with Simple.Data:
db.ProcedureWithoutParameters();

Way to Call it with Simple.Data.Access:
db.ProcedureWithoutParameters.*All()*;

I'm fairly certain additional or more robust support for Access Stored Procedures could be developed I just have not had the time or need for my personal projects.

==================
##Required Libraries
==================

Simple.Data.Access requires:
* Simple.Data
* Simple.Data.Ado

These libraries are available on:
- Nuget
- [Github-Simple.Data](https://github.com/markrendle/Simple.Data)
- Included with Simple.Data.Access as a submodule under dependencies folder.

**NOTE: The current version of Simple.Data (0.18.1 has an issue with the Simple.Data.Access != operator).  It has already been fixed in the source on Github and should be ok on later releases of Simple.Data/Simple.Data.Ado.

==================
##How To Use
==================

Open Connection to Database using standard OleDb connection string for Access or a filename or named or default connections in application configuration (same as Simple.Data):

1) Use default database defined in app.config 
app.config:
<br>
`<add name="Simple.Data.Properties.Settings.DefaultConnectionString"`<br>
`connectionString="Provider=Microsoft.Jet.OLEDB.4.0;Data Source=..\..\Data\Access2003TestDatabase.mdb" providerName="System.Data.OleDb" />`

Code:<br>
    var db = Database.Open();

2) Use named connection defined in app.config
app.config:
<br>
`<add name="Access2003"connectionString="Provider=Microsoft.Jet.OLEDB.4.0;`<br>
`Data Source=..\..\Data\Access2003TestDatabase.mdb" />`

Code:<br>
    var db = Database.OpenNamedConnection("Access2003");
            Assert.IsNotNull(db);

3) Use standard OleDb connection string<br>
Access 2000-2003: Provider=Microsoft.Jet.OLEDB.4.0;Data Source=..\..\Data\Access2003TestDatabase.mdb<br>
Access 2007+: Provider=Microsoft.ACE.OLEDB.12.0;Data Source=..\..\Data\Access2007TestDatabase.accdb<br>

Code:<br>
	var db = Database.OpenConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=..\..\Data\Access2003TestDatabase.mdb");

4) Open Access File directly<br>
var db = Database.OpenFile(@"..\..\Data\Access2003TestDatabase.mdb");<br>
var db = Database.OpenFile(@"..\..\Data\Access2007TestDatabase.accdb");

==================
##How To Use
==================

For issues, questions, suggestions, or help, please post something on the [Simple.Data Google Group](https://groups.google.com/forum/?fromgroups#!forum/simpledata)