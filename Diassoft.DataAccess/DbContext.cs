using Diassoft.DataAccess.DatabaseObjects;
using Diassoft.DataAccess.Dialects;
using Diassoft.DataAccess.Exceptions;
using Diassoft.DataAccess.Operations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Diassoft.DataAccess
{
    /// <summary>
    /// Represents the Database Context
    /// </summary>
    public class DbContext<TConnectionType>: IDbContext
                                             where TConnectionType : IDbConnection
    {
        /// <summary>
        /// Initializes a new instance of a <see cref="DbContext{TConnectionType}">Database Context</see>
        /// </summary>
        /// <param name="dialect">A <see cref="Dialects.Dialect"/> to be used to communicate with the database</param>
        /// <param name="connectionString">A <see cref="string"/> representing the Connection String</param>
        public DbContext(Dialect dialect, string connectionString)
        {
            this.Dialect = dialect;
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// The Dialect to communicate with the Database
        /// </summary>
        public Dialect Dialect { get; set; }

        /// <summary>
        /// The Connection String with the Database Server
        /// </summary>
        public string ConnectionString { get; set; }
        
        /// <summary>
        /// Verify if all settings are correct before doing any database transaction
        /// </summary>
        protected virtual void ValidateConfiguration()
        {
            if (Dialect == null)
                throw new Exception("Unable to identify a dialect to communicate with the database");

            if (String.IsNullOrEmpty(ConnectionString))
                throw new Exception($"The '{nameof(ConnectionString)}' is not setup");
        }

        /// <summary>
        /// Returns a Database Connection
        /// </summary>
        /// <remarks>If requires a <see cref="Dialect"/> and a <see cref="ConnectionString"/> to be setup</remarks>
        /// <returns>A <see cref="IDbConnection"/> with the connection to a database</returns>
        public IDbConnection GetConnection()
        {
            // Make sure configuration is valid
            ValidateConfiguration();

            // The Object that will be returned
            if (!(Activator.CreateInstance(typeof(TConnectionType)) is IDbConnection connection))
                throw new Exception($"The system was unable to find a parameterless constructor for the type '{typeof(TConnectionType).FullName}'");

            // Opens the Connection (exceptions are thrown in case the connection is invalid)
            connection.ConnectionString = ConnectionString;
            connection.Open();

            // Returns the Open Connection
            return connection;
        }

        /// <summary>
        /// Returns a Data Reader based on the input sql command
        /// </summary>
        /// <param name="selectDbOperation">A <see cref="SelectDbOperation"/> with the statement to be executed</param>
        /// <param name="connection">Reference to an existing <see cref="IDbConnection"/> to be used. If nothing is informed, the system will create one.</param>
        /// <param name="transaction">Reference to an existing <see cref="IDbTransaction"/> to be used. If nothing is informed, the system will not consider transaction.</param>
        /// <remarks>The Database Connection is closed once you call the <see cref="IDataReader.Close"/> method</remarks>
        /// <returns>A <see cref="IDataReader"/> object to iterate thru the results.</returns>
        public IDataReader ExecuteReader(SelectDbOperation selectDbOperation, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            if (Dialect == null)
                throw new Exception("Unable to identify a dialect to communicate with the database");

            return ExecuteReader(Dialect.Select(selectDbOperation));
        }

        /// <summary>
        /// Returns a Data Reader based on the input sql command
        /// </summary>
        /// <param name="commandText">The SQL Syntax</param>
        /// <param name="connection">Reference to an existing <see cref="IDbConnection"/> to be used. If nothing is informed, the system will create one.</param>
        /// <param name="transaction">Reference to an existing <see cref="IDbTransaction"/> to be used. If nothing is informed, the system will not consider transaction.</param>
        /// <remarks>The Database Connection is closed once you call the <see cref="IDataReader.Close"/> method</remarks>
        /// <returns>A <see cref="IDataReader"/> object to iterate thru the results.</returns>
        public IDataReader ExecuteReader(string commandText, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            // Make sure configuration is valid
            ValidateConfiguration();

            // Retrieves Connection
            IDbConnection internalConnection = (connection ?? GetConnection());

            using (IDbCommand command = internalConnection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = commandText;
                command.CommandTimeout = 360;

                command.CommandText = Dialect.GetFinalStatement(command.CommandText);

                try
                {
                    return command.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception e)
                {
                    throw new DatabaseOperationException(null, e, command.CommandText);
                }
            }
        }

        /// <summary>
        /// Executes a Non Query statement (Insert, Update or Delete)
        /// </summary>
        /// <param name="dbOperation">A valid object that inherits from <see cref="DbOperation{Table}"/>. Some examples are <see cref="InsertDbOperation"/>, <see cref="UpdateDbOperation"/> and <see cref="DeleteDbOperation"/>.</param>
        /// <param name="connection">Reference to an existing <see cref="IDbConnection"/> to be used. If nothing is informed, the system will create one.</param>
        /// <param name="transaction">Reference to an existing <see cref="IDbTransaction"/> to be used. If nothing is informed, the system will not consider transaction.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(DbOperation<Table> dbOperation, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            // Make sure configuration is valid
            ValidateConfiguration();

            // Variable to hold the connection
            IDbConnection internalConnection = null;

            try
            {
                // Use existing connection or create new
                internalConnection = (connection ?? GetConnection());

                using (IDbCommand command = internalConnection.CreateCommand())
                {
                    command.Transaction = transaction;

                    if (dbOperation.GetType() == typeof(InsertDbOperation))
                        command.CommandText = Dialect.Insert((InsertDbOperation)dbOperation);
                    else if (dbOperation.GetType() == typeof(UpdateDbOperation))
                        command.CommandText = Dialect.Update((UpdateDbOperation)dbOperation);
                    else if (dbOperation.GetType() == typeof(DeleteDbOperation))
                        command.CommandText = Dialect.Delete((DeleteDbOperation)dbOperation);
                    else
                        throw new Exception($"Database Operation of type '{dbOperation.GetType().FullName}' is not defined");

                    command.CommandText = Dialect.GetFinalStatement(command.CommandText);

                    try
                    {
                        int result = command.ExecuteNonQuery();

                        return result;
                    }
                    catch (Exception e)
                    {
                        throw new DatabaseOperationException(null, e, command.CommandText);
                    }

                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (connection == null)
                {
                    // Make sure to close the internal connection, if it was created on demand
                    if (internalConnection != null)
                    {
                        if ((internalConnection.State != ConnectionState.Broken) &&
                            (internalConnection.State != ConnectionState.Closed))
                        {
                            internalConnection.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Executes a query that would return only one row and one column
        /// </summary>
        /// <param name="selectDbOperation">The <see cref="SelectDbOperation"/> to be executed</param>
        /// <param name="connection">Reference to an existing <see cref="IDbConnection"/> to be used. If nothing is informed, the system will create one.</param>
        /// <param name="transaction">Reference to an existing <see cref="IDbTransaction"/> to be used. If nothing is informed, the system will not consider transaction.</param>
        /// <returns>A <see cref="object"/> containing the results</returns>
        public object ExecuteScalar(SelectDbOperation selectDbOperation, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            // Make sure configuration is valid
            ValidateConfiguration();

            // Variable to hold the connection
            IDbConnection internalConnection = null;

            try
            {
                // Use existing connection or create new
                internalConnection = (connection ?? GetConnection());

                using (IDbCommand command = internalConnection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = Dialect.Select(selectDbOperation);

                    command.CommandText = Dialect.GetFinalStatement(command.CommandText);

                    try
                    {
                        return command.ExecuteScalar();
                    }
                    catch (Exception e)
                    {
                        throw new DatabaseOperationException(null, e, command.CommandText);
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (connection == null)
                {
                    // Make sure to close the internal connection, if it was created on demand
                    if (internalConnection != null)
                    {
                        if ((internalConnection.State != ConnectionState.Broken) &&
                            (internalConnection.State != ConnectionState.Closed))
                        {
                            internalConnection.Close();
                        }
                    }
                }
            }
        }
    }
}
