using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.Exceptions
{
    /// <summary>
    /// Represents an Exception thrown when there is an error in a database operation
    /// </summary>
    public class DatabaseOperationException: System.Exception
    {
        private static readonly string DEFAULTMESSAGE = "Error during the execution of a database operation";

        /// <summary>
        /// The Sql Statement
        /// </summary>
        public string SqlStatement { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the DatabaseOperationException
        /// </summary>
        public DatabaseOperationException(): this(null) { }

        /// <summary>
        /// Initializes a new instance of the DatabaseOperationException
        /// </summary>
        /// <param name="message">The Error Message</param>
        public DatabaseOperationException(string message): this(message, null) { }

        /// <summary>
        /// Initializes a new instance of the DatabaseOperationException
        /// </summary>
        /// <param name="message">The Error Message</param>
        /// <param name="innerException">The inner exception to complement the exception</param>
        public DatabaseOperationException(string message, Exception innerException): this(message, innerException, null) { }

        /// <summary>
        /// Initializes a new instance of the DatabaseOperationException
        /// </summary>
        /// <param name="message">The Error Message</param>
        /// <param name="innerException">The inner exception to complement the exception</param>
        /// <param name="sqlStatement">The SQL Statement generating the failure</param>
        public DatabaseOperationException(string message, Exception innerException, string sqlStatement): base(message ?? DEFAULTMESSAGE, innerException)
        {
            SqlStatement = sqlStatement;
        }

    }
}
