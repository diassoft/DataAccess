using System;
using System.Collections.Generic;
using System.Text;
using Diassoft.DataAccess.DatabaseObjects;
using Diassoft.DataAccess.Operations;

namespace Diassoft.DataAccess.Dialects
{
    /// <summary>
    /// Represents the Oracle MySql T-SQL Dialect
    /// </summary>
    public class MySqlDialect: Dialect
    {
        /// <summary>
        /// Initializes a new instance of the Oracle MySql T-SQL Dialect
        /// </summary>
        public MySqlDialect(): base()
        {
            FieldNameChar = "`";
            FieldValueChar = "'";
            TableNameChar = "`";
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
        }

        /// <summary>
        /// Converts a Select Into Database Operation into a valid T-SQL Statement
        /// </summary>
        /// <param name="select">The <see cref="SelectDbOperation"/></param>
        /// <param name="intoTable">The destination table</param>
        /// <returns>A string containing the T-SQL</returns>
        public override string SelectInto(SelectDbOperation select, Table intoTable)
        {
            // The syntax for Select Into in MySql uses a Create Table
            if (intoTable == null)
                return base.SelectInto(select, intoTable);
            else
            {
                // Setup the "Create Table" syntax
                StringBuilder sbCreate = new StringBuilder();

                if (String.IsNullOrEmpty(intoTable.Name))
                    throw new Exception("A valid destination table name must be informed");

                intoTable.Alias = "";

                sbCreate.Append("CREATE TABLE ");
                sbCreate.Append(FormatTable(intoTable));
                sbCreate.AppendLine(" AS");

                sbCreate.Append(base.SelectInto(select, null));

                return sbCreate.ToString();
            }
        }

    }
}
