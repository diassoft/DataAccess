using System;
using System.Data;
using Diassoft.DataAccess;
using Diassoft.DataAccess.DatabaseObjects;
using Diassoft.DataAccess.DatabaseObjects.Expressions;
using Diassoft.DataAccess.DatabaseObjects.Fields;
using Diassoft.DataAccess.Dialects;
using Diassoft.DataAccess.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Diassoft.DataAccess.Test
{
    [TestClass]
    public class DbContextTest
    {
        // Connection String
        private string connectionString = "Server=(local);Database=TestDb;User Id=test;Password=test;";

        [TestMethod]
        public void TestDbContext_001_Connectivity()
        {
            // Creates the DbContext
            DbContext<System.Data.SqlClient.SqlConnection> dbContext = new DbContext<System.Data.SqlClient.SqlConnection>(new MSSQLDialect(), connectionString);

            try
            {
                IDbConnection connection = dbContext.GetConnection();
                connection.Close();
            }
            catch
            {
                throw;
            }
        }

        [TestMethod]
        public void TestDbContext_002_ExecuteNonQuery()
        {
            // Creates the DbContext
            DbContext<System.Data.SqlClient.SqlConnection> dbContext = new DbContext<System.Data.SqlClient.SqlConnection>(new MSSQLDialect(), connectionString);

            try
            {
                // Current DateTime
                DateTime currentDateTime = DateTime.Now;

                // Create an insert
                dbContext.ExecuteNonQuery(new InsertDbOperation(new Table("Person"))
                {
                    Assignments = new AssignExpression[]
                    {
                        new AssignExpression("FirstName", $"Name {currentDateTime.ToString("yyyy-MM-dd HH:mm:ss")}"),
                        new AssignExpression("DateOfBirth", new DateTime(1984, 7, 27)),
                    }
                });
            }
            catch
            {
                throw;
            }
        }

        [TestMethod]
        public void TestDbContext_003_ExecuteReader()
        {
            // Creates the DbContext
            DbContext<System.Data.SqlClient.SqlConnection> dbContext = new DbContext<System.Data.SqlClient.SqlConnection>(new MSSQLDialect(), connectionString);

            try
            {
                // Current DateTime
                DateTime currentDateTime = DateTime.Now;
                int rowCount = 0;

                // Create an insert
                using (IDataReader reader = dbContext.ExecuteReader(new SelectDbOperation("Person")))
                {
                    while (reader.Read())
                    {
                        rowCount++;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        [TestMethod]
        public void TestDbContext_004_ExecuteScalar()
        {
            // Creates the DbContext
            DbContext<System.Data.SqlClient.SqlConnection> dbContext = new DbContext<System.Data.SqlClient.SqlConnection>(new MSSQLDialect(), connectionString);

            try
            {
                // Current DateTime
                DateTime currentDateTime = DateTime.Now;

                int rowCount = (int)dbContext.ExecuteScalar(new SelectDbOperation("Person")
                {
                    SelectFields = new FieldCollection()
                    {
                        new AggregateField(Aggregates.Count, "0")
                    }
                });


            }
            catch
            {
                throw;
            }
        }
    }
}
