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

        /// <summary>
        /// Formats a <see cref="DateTime"/> accordingly to the Database Dialect
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> to format</param>
        /// <returns>A <see cref="string"/> containing the formatted date</returns>
        protected override string FormatDate(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue) return "NULL";

            return $"{FieldValueChar}{dateTime.ToString($"{DateFormat} {TimeFormat}")}{FieldValueChar}";
        }

        /// <summary>
        /// Selects data from the database using the Microsoft SQL Server T-SQL Dialect
        /// </summary>
        /// <param name="select">An <see cref="SelectDbOperation"/> representing the Select to Perform</param>
        /// <returns>A <see cref="string"/> containing the select statement to be executed</returns>
        public override string Select(SelectDbOperation select)
        {
            return SelectInto(select, null);
        }

        /// <summary>
        /// Selects data from the database using the Microsoft SQL Server T-SQL Dialect. Appends the result into a table
        /// </summary>
        /// <param name="select">An <see cref="SelectDbOperation"/> representing the Select to Perform</param>
        /// <param name="intoTable">The <see cref="Table"/> where the data should be inserted</param>
        /// <returns>A <see cref="string"/> containing the select statement to be executed</returns>
        public override string SelectInto(SelectDbOperation select, Table intoTable)
        {
            // Container for Query Parameters
            StringBuilder sbSelect = new StringBuilder();

            // Ensure there are tables on the operation
            if (select.Table == null) throw new Exception("There are no tables on the Select Statement");
            if (select.Table.Length == 0) throw new Exception("There are no tables on the Select Statement");

            // Make sure Distinct and Group by are not activated at the same time
            if ((select.Distinct) && (select.GroupBy))
                throw new Exception($"Unable to make a Select Statement with both DISTINCT and GROUP BY at the same time");

            // Check Group By Setup
            if (select.GroupBy)
            {
                if ((select.SelectFields == null) || (select.SelectFields?.Count == 0))
                    throw new Exception($"When using Group By you need to inform the columns you want to group on the {nameof(select.SelectFields)} collection.");
            }

            // ====================================================================================
            // SELECT FIELDS AREA
            // ====================================================================================

            // Display Fields
            sbSelect.Append("SELECT ");

            if (select.Distinct) sbSelect.Append("DISTINCT ");

            // Check all Non-Aggregate Fields
            if (select.SelectFields?.Count > 0)
            {
                // For formatting purposes, add a line right after the select
                sbSelect.AppendLine();

                // Select Specific Fields
                sbSelect.AppendLine(String.Join(",\r\n", from field in @select.SelectFields
                                                         select String.Concat(String.Empty.PadLeft(7, ' '),
                                                                              this.FormatExpressionField(field))));
            }
            else
            {
                // Select All Fields
                sbSelect.Append('*');
                sbSelect.AppendLine();
            }

            // ====================================================================================
            // INTO AREA
            // ====================================================================================

            // If a table was passed, use it for a Select Into
            if (intoTable != null)
            {
                sbSelect.AppendLine("  INTO");
                sbSelect.AppendLine(base.FormatTable(intoTable).PadLeft(7, ' '));
            }

            // ====================================================================================
            // FROM AREA
            // ====================================================================================

            // From Statement
            sbSelect.Append("  FROM ");

            if (select.Table.Length == 1)
            {
                // Append Single Table
                sbSelect.AppendLine(FormatTable(select.Table[0]));
            }
            else
            {
                // Append Multiple Tables Separated by a Comma
                sbSelect.AppendLine();
                sbSelect.AppendLine(String.Join(",\r\n", from tbl in @select.Table
                                                         select String.Concat(String.Empty.PadLeft(7, ' '), FormatTable(tbl))));
            }

            // ====================================================================================
            // JOINS AREA
            // ====================================================================================

            //TODO: Implement 

            // ====================================================================================
            // WHERE AREA
            // ====================================================================================

            if (select.Where?.Count > 0)
            {
                sbSelect.AppendLine(" WHERE");
                sbSelect.Append(FormatExpressions(select.Where.ToArray(), 1));
            }

            // ====================================================================================
            // GROUP BY AREA
            // ====================================================================================

            // Group By Fields
            if (select.GroupBy)
            {
                sbSelect.AppendLine("GROUP BY");

                if (select.SelectFields?.Count(field => field.GetType() != typeof(AggregateField)) > 0)
                {
                    // Add all display fields but make sure to not have the alternate name on it
                    sbSelect.AppendLine(String.Join(",\r\n", from field in @select.SelectFields
                                                             where field.GetType() != typeof(AggregateField)
                                                             select String.Concat(String.Empty.PadLeft(9, ' '), FormatField(new DisplayField(((DisplayField)field).Name, ((DisplayField)field).TableAlias)))));
                }
            }

            // ====================================================================================
            // HAVING AREA
            // ====================================================================================

            //TODO: Implement

            // ====================================================================================
            // ORDER BY AREA
            // ====================================================================================

            if (select.OrderBy?.Count > 0)
            {
                sbSelect.AppendLine("ORDER BY");
                sbSelect.AppendLine(String.Join(",\r\n", from orderByField in @select.OrderBy
                                                         select String.Concat(String.Empty.PadLeft(9, ' '),
                                                                              FormatField(orderByField),
                                                                              orderByField.SortMode == SortModes.Descending ? " DESC" : "")));
            }

            // Returns the Result of the Select Statement
            return sbSelect.ToString();

        }

        /// <summary>
        /// Inserts data into the database using the Microsoft SQL Server T-SQL Dialect. 
        /// </summary>
        /// <param name="insert">The <see cref="InsertDbOperation"/></param>
        /// <returns>A string containing the T-SQL</returns>
        public override string Insert(InsertDbOperation insert)
        {
            // String Builders
            StringBuilder sbInsert = new StringBuilder();

            // Validate Table
            if (insert.Table == null)
                throw new Exception("Insert requires a table. Table is null.");

            if (String.IsNullOrEmpty(insert.Table.Name))
                throw new Exception("Insert requires a table. Table is blank or empty.");

            // Validate Assignments
            if (insert.Assignments == null)
                throw new Exception("No assignments defined for the Insert Operation");

            if (insert.Assignments.Length == 0)
                throw new Exception("Zero assignments defined for the Insert Operation");

            // Create arrays for insert data
            string[] insertFields = new string[insert.Assignments.Length];
            string[] insertValues = new string[insert.Assignments.Length];

            // Now format the sintax
            for (int f = 0; f < insert.Assignments.Length; f++)
            {
                insertFields[f] = FormatExpressionField(insert.Assignments[f].Field1);
                insertValues[f] = FormatExpressionField(insert.Assignments[f].Field2);
            }

            // Format for output
            sbInsert.Append("INSERT INTO ");
            sbInsert.AppendLine(FormatTable(insert.Table));

            sbInsert.AppendLine("            (");
            sbInsert.Append("                    ");
            sbInsert.AppendLine(String.Join(",\r\n                    ", insertFields));
            sbInsert.AppendLine("            )");
            sbInsert.AppendLine("     VALUES (");
            sbInsert.Append("                    ");
            sbInsert.AppendLine(String.Join(",\r\n                    ", insertValues));
            sbInsert.AppendLine("            )");

            return sbInsert.ToString();
        }

        /// <summary>
        /// Converts an Update Database Operation into a valid T-SQL Statement
        /// </summary>
        /// <param name="update">The <see cref="UpdateDbOperation"/></param>
        /// <returns>A string contaiing the T-SQL</returns>
        public override string Update(UpdateDbOperation update)
        {
            // String Builders
            StringBuilder sbUpdate = new StringBuilder();

            // Validate Table
            if (update.Table == null)
                throw new Exception("Update requires a table. Table is null.");

            if (String.IsNullOrEmpty(update.Table.Name))
                throw new Exception("Update requires a table. Table is blank or empty.");

            // Validate Assignments
            if (update.Assignments == null)
                throw new Exception("No assignments defined for the Update Operation");

            if (update.Assignments.Length == 0)
                throw new Exception("Zero assignments defined for the Update Operation");

            // ===================================================================
            // UPDATE
            // ===================================================================

            sbUpdate.Append("UPDATE ");
            sbUpdate.AppendLine(FormatTable(update.Table));

            // ===================================================================
            // SET
            // ===================================================================
            sbUpdate.AppendLine("   SET");

            string[] updateAssignments = new string[update.Assignments.Length];

            for (int i = 0; i < update.Assignments.Length; i++)
            {
                var a = update.Assignments[i];
                updateAssignments[i] = $"       {FormatExpressionField(a.Field1)}={FormatExpressionField(a.Field2)}";
            }

            sbUpdate.AppendLine(String.Join(",\r\n", updateAssignments));

            // ===================================================================
            // WHERE
            // ===================================================================

            if (update.Where?.Count > 0)
            {
                sbUpdate.AppendLine(" WHERE");
                sbUpdate.Append(FormatExpressions(update.Where.ToArray(), 1));
            }

            return sbUpdate.ToString();
        }

        /// <summary>
        /// Converts a Delete Database Operation into a valid T-SQL Statement
        /// </summary>
        /// <param name="delete">The <see cref="DeleteDbOperation"/></param>
        /// <returns>A string contaiing the T-SQL</returns>
        public override string Delete(DeleteDbOperation delete)
        {
            // String Builders
            StringBuilder sbDelete = new StringBuilder();

            // Validate Table
            if (delete.Table == null)
                throw new Exception("Delete requires a table. Table is null.");

            if (String.IsNullOrEmpty(delete.Table.Name))
                throw new Exception("Delete requires a table. Table is blank or empty.");

            // ===================================================================
            // DELETE FROM
            // ===================================================================

            sbDelete.Append("DELETE FROM ");
            sbDelete.AppendLine(FormatTable(delete.Table));

            // ===================================================================
            // WHERE
            // ===================================================================

            if (delete.Where?.Count > 0)
            {
                sbDelete.AppendLine(" WHERE");
                sbDelete.Append(FormatExpressions(delete.Where.ToArray(), 1));
            }

            return sbDelete.ToString();
        }

    }
}
