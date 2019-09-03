using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.Exceptions
{
    /// <summary>
    /// Represents an <see cref="Exception"/> that is thrown when the Query Syntax is invalid
    /// </summary>
    public class QuerySyntaxErrorException: System.Exception
    {
        /// <summary>
        /// The <see cref="DbOperation"/>where the error happened
        /// </summary>
        public object DbOperation { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuerySyntaxErrorException"/>
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="dbOperation">The <see cref="DbOperation"/></param>
        public QuerySyntaxErrorException(string message, object dbOperation): this(message, dbOperation, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuerySyntaxErrorException"/>
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="dbOperation">The <see cref="DbOperation"/></param>
        /// <param name="exception">The reason to cause the exception</param>
        public QuerySyntaxErrorException(string message, object dbOperation, Exception exception): base(message, exception)
        {
            DbOperation = dbOperation;
        }
    }

}
