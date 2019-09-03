using Diassoft.DataAccess.FilterParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diassoft.DataAccess.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a record could be found in a database table
    /// </summary>
    public class RecordNotFoundException: System.Exception
    {
        private static string DEFAULTMESSAGE = "Record not found";

        /// <summary>
        /// The Keys used for the search on the object
        /// </summary>
        public List<FilterField> Keys { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordNotFoundException"/>
        /// </summary>
        public RecordNotFoundException(): this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordNotFoundException"/>
        /// </summary>
        /// <param name="message">The exception message</param>
        public RecordNotFoundException(string message) : this(message, (Exception)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordNotFoundException"/>
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The exception that caused this exception to be thrown</param>
        public RecordNotFoundException(string message, Exception innerException): this(message, null, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordNotFoundException"/>
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="keys">Keys used for the search</param>
        public RecordNotFoundException(string message, List<FilterField> keys): this(message, keys, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordNotFoundException"/>
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="keys">Keys used for the search</param>
        /// <param name="innerException">The exception that caused this exception to be thrown</param>
        public RecordNotFoundException(string message, List<FilterField> keys, Exception innerException): base(message ?? DEFAULTMESSAGE, innerException)
        {
            Keys = keys;
        }

        /// <summary>
        /// Creates and returns a string representation of the exception
        /// </summary>
        /// <returns>A string containing the representation of the exception</returns>
        public override string ToString()
        {
            if (Keys.Count == 0)
                return base.Message;
            else
            {
                return string.Format("{0}. Keys used: {1}",
                                     base.Message,
                                     String.Join(",", from k 
                                                      in Keys
                                                      select $"{k.Field}={k.Value.ToString()}"));
            }
        }

    }
}
