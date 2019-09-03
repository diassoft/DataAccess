using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.Exceptions
{
    /// <summary>
    /// Exception thrown when a Primary Key is violated
    /// </summary>
    public class PrimaryKeyViolationException : DatabaseOperationException
    {
        private static readonly string DEFAULTMESSAGE = "A primary key violation prevented the operation from happening";

        /// <summary>
        /// Initializes a new instance of the Primary Key Violation Exception
        /// </summary>
        public PrimaryKeyViolationException(): this(null) { }
        /// <summary>
        /// Initializes a new instance of the Primary Key Violation Exception
        /// </summary>
        /// <param name="message">The Error Message</param>
        public PrimaryKeyViolationException(string message) : this(message, null) { }
        /// <summary>
        /// Initializes a new instance of the Primary Key Violation Exception
        /// </summary>
        /// <param name="message">The Error Message</param>
        /// <param name="innerException">The inner exception to complement the exception</param>
        public PrimaryKeyViolationException(string message, Exception innerException): this(message, innerException, null) { }
        /// <summary>
        /// Initializes a new instance of the Primary Key Violation Exception
        /// </summary>
        /// <param name="message">The Error Message</param>
        /// <param name="innerException">The inner exception to complement the exception</param>
        /// <param name="sqlStatement">The SQL Statement generating the failure</param>
        public PrimaryKeyViolationException(string message, Exception innerException, string sqlStatement) : base(message ?? DEFAULTMESSAGE, innerException, sqlStatement) { }
    }
}
