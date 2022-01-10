using Diassoft.DataAccess.DatabaseObjects;
using Diassoft.DataAccess.Operations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Diassoft.DataAccess.DatabaseObjects.Fields;
using Diassoft.DataAccess.DatabaseObjects.Expressions;

namespace Diassoft.DataAccess.Dialects
{
    /// <summary>
    /// Represents the Microsoft SQL Server T-SQL Dialect
    /// </summary>
    public class MSSQLDialect: Dialect
    {
        /// <summary>
        /// Initializes a new instance of the Microsoft SQL T-SQL Dialect
        /// </summary>
        public MSSQLDialect(): base()
        {
            FieldNameChar = "[]";
            FieldValueChar = "'";
            TableNameChar = "[]";
            NumericFormat = "";
            DecimalFormat = "";
            DateFormat = "yyyy-MM-dd";
            TimeFormat = "HH:mm:ss";

            // Include Reserved Words
            AddReservedWords("user",
                             "datetime",
                             "date",
                             "object",
                             "name",
                             "description");

            // Include Date Formatting before any statement
            AddBeforeStatements("SET DATEFORMAT ymd");        // Always consider dates to be Year-Month-Date, because we do not know what is the local language and thus the format
            AddBeforeStatements("SET DATEFIRST 7");           // Sets Sunday to be the first day of the week
        }

    }
}
