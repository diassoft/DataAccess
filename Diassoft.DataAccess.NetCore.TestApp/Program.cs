using Diassoft.DataAccess.DatabaseObjects;
using Diassoft.DataAccess.DatabaseObjects.Expressions;
using Diassoft.DataAccess.DatabaseObjects.Fields;
using Diassoft.DataAccess.Operations;
using System;

namespace Diassoft.DataAccess.NetCore.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string userChoice = "";

            while (userChoice != "X")
            {
                Console.WriteLine("================================================================================");
                Console.WriteLine("Diassoft Data Access Examples");
                Console.WriteLine("================================================================================");
                Console.WriteLine("1 = Run Example 01 (method {0}())", nameof(Example_01));
                Console.WriteLine("X = Exit");
                Console.WriteLine();
                Console.Write("Enter your option and press ENTER => ");

                userChoice = Console.ReadLine().ToUpper();

                if (userChoice == "1")
                {
                    Example_01();
                }

                Console.WriteLine();
                Console.WriteLine("Press enter to restart...");
                Console.ReadLine();
                Console.Clear();
            }


        }

        // This function opens a table and return its data
        static void Example_01()
        {
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("Example 01");
            Console.WriteLine("--------------------------------------------------");

            // Create Database Context
            var connectionString = "Server=localhost\\SQLExpress; Database=Northwind; User Id=guest; Password=Guest1234;";
            var dbContext = new DbContext<System.Data.SqlClient.SqlConnection>(new Diassoft.DataAccess.Dialects.MSSQLDialect(), connectionString);

            // Create a Connection
            using (var connection = dbContext.GetConnection())
            {
                // Create the Select Statement
                var select = new SelectDbOperation(new Table("Employees", "dbo"))
                {
                    SelectFields = new FieldCollection()
                    {
                        new DisplayField("FirstName"),
                        new DisplayField("LastName"),
                    },
                    Where = new WhereCollection()
                    {
                        new FilterExpression(new Field("Country"), FieldOperators.Equal, "USA", FieldAndOr.And),
                        new FilterExpression(new Field("Region"), FieldOperators.Equal, "WA", FieldAndOr.None),
                    }
                };

                // Execute the Select Statement
                using (var reader = dbContext.ExecuteReader(select))
                {
                    // Read each record
                    while (reader.Read())
                    {
                        Console.WriteLine("Employee: {0} {1}", reader.GetValue(0).ToString(), reader.GetValue(1).ToString());
                    }
                }
            }

        }
    }
}
