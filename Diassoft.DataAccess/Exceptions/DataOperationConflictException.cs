using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when there is a conflict during the database operation
    /// </summary>
    /// <remarks>A conflict could be caused by multiple reasons:
    /// <list type="bullet">
    /// <item>Data cannot be deleted because it has connections with other tables</item>
    /// <item>Data cannot be changed because it will violate database contraints</item>
    /// </list></remarks>
    public class DataOperationConflictException: System.Exception
    {
        private static string DEFAULTMESSAGE = "A conflict happened while performing a database operation";

        /// <summary>
        /// Initialize a new instance of the <see cref="DataOperationConflictException"/>
        /// </summary>
        public DataOperationConflictException(): this(null) { }

        /// <summary>
        /// Initialize a new instance of the <see cref="DataOperationConflictException"/>
        /// </summary>
        /// <param name="message">The error message</param>
        public DataOperationConflictException(string message) : this(message, null) { }

        /// <summary>
        /// Initialize a new instance of the <see cref="DataOperationConflictException"/>
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="innerException">The exception that caused this exception to be thrown</param>
        public DataOperationConflictException(string message, Exception innerException): base(message ?? DEFAULTMESSAGE, innerException)
        {

        }

    }
}
