# Diassoft Data Access v3.0.0
[![nuget](https://img.shields.io/nuget/v/Diassoft.DataAccess.svg)](https://www.nuget.org/packages/Diassoft.DataAccess/) 
![GitHub release](https://img.shields.io/github/release/diassoft/Diassoft.DataAccess.svg)
![NuGet](https://img.shields.io/nuget/dt/Diassoft.DataAccess.svg)
![license](https://img.shields.io/github/license/diassoft/Diassoft.DataAccess.svg)

The Diassoft Data Access is a component to assist the generation of database queries for multiple databases.
Instead of writing queires that are specific to a certain database, this component provides an abstract method to create the database operations and generates the queries according to the database.

As of this version, the component has been tested for the following databases:

| Database | Last Version Tested | Package Release |
| :-- | :-- | :-- |
| Microsoft SQL Server | 2019 | v.1.0.0 |
| MySql| 8.0.29 Community Edition | v.3.0.7 |

>If you wish to contact the creator of this component, you can make it thru [Nuget.org](https://www.nuget.org/packages/Diassoft.DataAccess/) page or by email [olavodias@gmail.com](mailto:olavodias@gmail.com)

## In this repository

* [Data Access Documentation](https://diassoft.github.io/Diassoft.DataAccess_v300)
* [Getting Started](#getting-started)
  * [Selecting Records](#selecting-records)
* [How To Use It](#how-to-use-it)

## Getting Started

Use the examples on this section to get started with the component.

>You will need to use a demo database to run some of the examples. [Refer to this repository to download the Northwind database](https://github.com/Microsoft/sql-server-samples/tree/master/samples/databases/northwind-pubs).

### Selecting Records

The code below demonstrates how to connect to a database and execute a select statement.

```cs
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
````

## Undestanding The Objects

The Data Access component needs to understand how each database expects the queries to be generated.
This information is given to the component thru a Dialect.

The base of each SQL query is, most of the times, very similar. However, there are details that have to be taken into consideration when generating queries.

By default, the component comes with the following dialects:

| Dialect | Database |
| :-- | :-- |
| `MSSQLDialect` | Microsoft SQL Server |
| `MySqlDialect` | MySql Database |

New dialects can be created by inheriting the `Dialect` base class.

## How To Use It

