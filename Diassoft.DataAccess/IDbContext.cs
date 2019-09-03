using Diassoft.DataAccess.DatabaseObjects;
using Diassoft.DataAccess.Dialects;
using Diassoft.DataAccess.Operations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Diassoft.DataAccess
{
    /// <summary>
    /// Represents an Database Context Interface
    /// </summary>
    public interface IDbContext
    {
        /// <summary>
        /// The Dialect to communicate with the Database
        /// </summary>
        Dialect Dialect { get; set; }
        /// <summary>
        /// The Connection String with the Database Server
        /// </summary>
        string ConnectionString { get; set; }
        /// <summary>
        /// Returns a Database Connection
        /// </summary>
        /// <remarks>If requires a <see cref="Dialect"/> and a <see cref="ConnectionString"/> to be setup</remarks>
        /// <returns>A <see cref="IDbConnection"/> with the connection to a database</returns>
        IDbConnection GetConnection();
        /// <summary>
        /// Returns a Data Reader based on the input sql command
        /// </summary>
        /// <param name="selectDbOperation">A <see cref="SelectDbOperation"/> with the statement to be executed</param>
        /// <param name="connection">Reference to an existing <see cref="IDbConnection"/> to be used. If nothing is informed, the system will create one.</param>
        /// <param name="transaction">Reference to an existing <see cref="IDbTransaction"/> to be used. If nothing is informed, the system will not consider transaction.</param>
        /// <remarks>The Database Connection is closed once you call the <see cref="IDataReader.Close"/> method</remarks>
        /// <returns>A <see cref="IDataReader"/> object to iterate thru the results.</returns>
        IDataReader ExecuteReader(SelectDbOperation selectDbOperation, IDbConnection connection = null, IDbTransaction transaction = null);
        /// <summary>
        /// Returns a Data Reader based on the input sql command
        /// </summary>
        /// <param name="commandText">The SQL Syntax</param>
        /// <param name="connection">Reference to an existing <see cref="IDbConnection"/> to be used. If nothing is informed, the system will create one.</param>
        /// <param name="transaction">Reference to an existing <see cref="IDbTransaction"/> to be used. If nothing is informed, the system will not consider transaction.</param>
        /// <remarks>The Database Connection is closed once you call the <see cref="IDataReader.Close"/> method</remarks>
        /// <returns>A <see cref="IDataReader"/> object to iterate thru the results.</returns>
        IDataReader ExecuteReader(string commandText, IDbConnection connection = null, IDbTransaction transaction = null);
        /// <summary>
        /// Executes a Non Query statement (Insert, Update or Delete)
        /// </summary>
        /// <param name="dbOperation">A valid object that inherits from <see cref="DbOperation{Table}"/>. Some examples are <see cref="InsertDbOperation"/>, <see cref="UpdateDbOperation"/> and <see cref="DeleteDbOperation"/>.</param>
        /// <param name="connection">Reference to an existing <see cref="IDbConnection"/> to be used. If nothing is informed, the system will create one.</param>
        /// <param name="transaction">Reference to an existing <see cref="IDbTransaction"/> to be used. If nothing is informed, the system will not consider transaction.</param>
        /// <returns></returns>
        int ExecuteNonQuery(DbOperation<Table> dbOperation, IDbConnection connection = null, IDbTransaction transaction = null);
        /// <summary>
        /// Executes a query that would return only one row and one column
        /// </summary>
        /// <param name="selectDbOperation">The <see cref="SelectDbOperation"/> to be executed</param>
        /// <param name="connection">Reference to an existing <see cref="IDbConnection"/> to be used. If nothing is informed, the system will create one.</param>
        /// <param name="transaction">Reference to an existing <see cref="IDbTransaction"/> to be used. If nothing is informed, the system will not consider transaction.</param>
        /// <returns>A <see cref="object"/> containing the results</returns>
        object ExecuteScalar(SelectDbOperation selectDbOperation, IDbConnection connection = null, IDbTransaction transaction = null);
    }
}
