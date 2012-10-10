using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using Simple.Data.Access.Test.Data;

namespace Simple.Data.Access.Test
{
    public enum AccessDatabaseType
    {
        Access2002,
        Access2003,
        Access2007,
        Access2010
    };

    public class DatabaseHelper
    {
        private string _connectionString;
        private OleDbConnection _connection;
        private string _xmlData;
        private string _dbPath;
        private AccessDatabaseType _dbType;
        private string _provider;

        public DatabaseHelper(AccessDatabaseType dbType)
        {
            this._dbType = dbType;

            switch (dbType)
            {
                case AccessDatabaseType.Access2002:
                    ConfigureAccess2003();
                    break;
                case AccessDatabaseType.Access2003:
                    ConfigureAccess2003();
                    break;
                case AccessDatabaseType.Access2007:
                    ConfigureAccess2007();
                    break;
                case AccessDatabaseType.Access2010:
                    ConfigureAccess2007();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("dbType");
            }
        }

        private void ConfigureAccess2003()
        {
            this._connectionString = Properties.Settings.Default.Access2003ConnectionString;
            this._connection = new OleDbConnection(_connectionString);
            this._xmlData = @"..\..\Data\Access2003TestData.xml";
            this._dbPath = @"..\..\Data\Access2003TestDatabase.mdb";
            this._provider = "Provider=Microsoft.Jet.OLEDB.4.0";
        }

        private void ConfigureAccess2007()
        {
            this._connectionString = Properties.Settings.Default.Access2007ConnectionString;
            this._connection = new OleDbConnection(_connectionString);
            this._xmlData = @"..\..\Data\Access2007TestData.xml";
            this._dbPath = @"..\..\Data\Access2007TestDatabase.mdb";
            this._provider = "Provider=Microsoft.ACE.OLEDB.12.0";
        }

        public string ConnectionString { get { return this._connectionString;  } }

        public AccessDatabaseType DatabaseType { get { return this._dbType; } }

        public void GenerateXmlTestDataFromExistingDatabase()
        {
            var adapter = new Data.Access2003DataSetTableAdapters.UsersTableAdapter();
            adapter.Connection = new OleDbConnection(Properties.Settings.Default.Access2003ConnectionString);

            var dt = new Access2003DataSet.UsersDataTable();
            adapter.Fill(dt);

            dt.WriteXml(@"..\..\Data\Access2003TestData.xml");

            var adapter2 = new Data.Access2007DataSetTableAdapters.UsersTableAdapter();
            adapter2.Connection = new OleDbConnection(Properties.Settings.Default.Access2003ConnectionString);

            var dt2 = new Access2007DataSet.UsersDataTable();
            adapter2.Fill(dt2);

            dt2.WriteXml(@"..\..\Data\Access2007TestData.xml");
        }

        private void ResetAutoNumbers(string connectionString, string tempFileName, string dbFilename)
        {
            object[] oParams;

            //create an inctance of a Jet Replication Object
            object objJRO =
                Activator.CreateInstance(Type.GetTypeFromProgID("JRO.JetEngine"));

            //filling Parameters array
            //cnahge "Jet OLEDB:Engine Type=5" to an appropriate value
            // or leave it as is if you db is JET4X format (access 2000,2002)
            //(yes, jetengine5 is for JET4X, no misprint here)
            oParams = new object[]
                          {
                              connectionString,
                              _provider + ";Data Source=" + tempFileName + ";Jet OLEDB:Engine Type=5"
                          };

            //invoke a CompactDatabase method of a JRO object
            //pass Parameters array
            objJRO.GetType().InvokeMember("CompactDatabase",
                                          System.Reflection.BindingFlags.InvokeMethod,
                                          null,
                                          objJRO,
                                          oParams);

            //database is compacted now
            //to a new file C:\\tempdb.mdw
            //let's copy it over an old one and delete it

            System.IO.File.Delete(dbFilename);
            System.IO.File.Move(tempFileName, dbFilename);

            //clean up (just in case)
            System.Runtime.InteropServices.Marshal.ReleaseComObject(objJRO);
            objJRO = null;
        }

        public void Reset()
        {
            if (this._dbType == AccessDatabaseType.Access2003 || this._dbType == AccessDatabaseType.Access2002)
            {
                var adapter = new Data.Access2003DataSetTableAdapters.UsersTableAdapter();
                adapter.Connection = _connection;

                // Delete any existing records
                adapter.DeleteAll();

                // Auto reset numbers by compacting Access
                // MAKE SURE to first delete all data
                ResetAutoNumbers(_connectionString, "tempDb.mdb", this._dbPath);

                // Load clean data
                var dt = new Access2003DataSet.UsersDataTable();
                dt.ReadXml(this._xmlData);
                adapter.Update(dt);
            }
            else
            {
                var adapter = new Data.Access2007DataSetTableAdapters.UsersTableAdapter();
                adapter.Connection = _connection;

                // Delete any existing records
                adapter.DeleteAll();

                // Auto reset numbers by compacting Access
                // MAKE SURE to first delete all data
                ResetAutoNumbers(_connectionString, "tempDb.accdb", this._dbPath);

                // Load clean data
                var dt = new Access2007DataSet.UsersDataTable();
                dt.ReadXml(this._xmlData);
                adapter.Update(dt);
            }
        }
    }
}
