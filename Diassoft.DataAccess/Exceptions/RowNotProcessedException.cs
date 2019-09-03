using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Diassoft.DataAccess.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a row is not processed from a file
    /// </summary>
    public sealed class RowNotProcessedException: System.Exception
    {
        private static string DEFAULTMESSAGE = "Row could not be processed";

        /// <summary>
        /// The file name
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Number of the row that could not be processed
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RowNotProcessedException"/>
        /// </summary>
        /// <param name="fileName">The file that was being processed</param>
        /// <param name="rowNumber">Number of the row that could not be processed</param>
        public RowNotProcessedException(string fileName, int rowNumber) : this(fileName, rowNumber, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RowNotProcessedException"/>
        /// </summary>
        /// <param name="fileName">The file that was being processed</param>
        /// <param name="rowNumber">Number of the row that could not be processed</param>
        /// <param name="innerException">The exception that caused the error to happen</param>
        public RowNotProcessedException(string fileName, int rowNumber, Exception innerException): base(DEFAULTMESSAGE, innerException)
        {
            FileName = fileName;
            RowNumber = rowNumber;
        }

        /// <summary>
        /// Creates and returns a string representation of the exception
        /// </summary>
        /// <returns>A string containing the representation of the exception</returns>
        public override string ToString()
        {
            if (String.IsNullOrEmpty(FileName))
            {
                var fileInfo = new FileInfo(FileName);
                return $"Row {RowNumber} on file '{fileInfo.Name}' could not be processed";
            }
            else
            {
                return $"Row {RowNumber} could not be processed";
            }
        }
    }
}
