using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Diassoft.DataAccess.DatabaseObjects;
using Diassoft.DataAccess.DatabaseObjects.Expressions;
using Diassoft.DataAccess.DatabaseObjects.Fields;
using Diassoft.DataAccess.Operations;
using System.Collections.ObjectModel;

namespace Diassoft.DataAccess.Dialects
{
    /// <summary>
    /// Represents the Base class for a Dialect.
    /// </summary>
    /// <remarks>Dialects are used to communicate with the database using their syntax</remarks>
    public abstract class Dialect
    {
        /// <summary>
        /// Initializes a new instance of the Dialect Class
        /// </summary>
        public Dialect()
        {
            // Populate with default operators
            Operators = new Dictionary<FieldOperators, string>
            {
                { FieldOperators.None, "" },
                { FieldOperators.Equal, "=" },
                { FieldOperators.NotEqual, "<>" },
                { FieldOperators.GreaterThan, ">" },
                { FieldOperators.GreaterThanOrEqual, ">=" },
                { FieldOperators.LessThan, "<" },
                { FieldOperators.LessThanOrEqual, "<=" },
                { FieldOperators.Like, "LIKE {value}" },
                { FieldOperators.NotLike, "NOT LIKE {value}" },
                { FieldOperators.In, "IN ({value})" },
                { FieldOperators.NotIn, "NOT IN ({value})" },
                { FieldOperators.IsNull, "IS NULL" },
                { FieldOperators.NotIsNull, "IS NOT NULL" },
                { FieldOperators.Between, "BETWEEN {value1} AND {value2}" },
                { FieldOperators.NotBetween, "NOT BETWEEN {value1} AND {value2}" }
            };

            // Populate default Aggregates
            AggregateFunctions = new Dictionary<Aggregates, string>
            {
                { Aggregates.Sum, "SUM({value})" },
                { Aggregates.Max, "MAX({value})" },
                { Aggregates.Min, "MIN({value})" },
                { Aggregates.Average, "AVG({value})" },
                { Aggregates.Count, "COUNT({value})" },
                { Aggregates.CountDistinct, "COUNT(DISTINCT {value})" },
            };

            ReservedWords = new List<string>();
            BeforeQueryStatements = new List<string>();
            AfterQueryStatements = new List<string>();
        }

        #region Properties

        /// <summary>
        /// The list of reserved words for the dialect
        /// </summary>
        public List<string> ReservedWords { get; protected set; }

        /// <summary>
        /// The Character to add before and after any field. Add two characters to consider the first for the beginning and the second for the end. 
        /// Use it for databases where you add field names that follow this example: [myFieldName].
        /// </summary>
        public string FieldNameChar { get; protected set; }

        /// <summary>
        /// The Character to add before and after any text field. Use it to add " or ' before any text assignment. The default value is '.
        /// </summary>
        public string FieldValueChar { get; protected set; }

        /// <summary>
        /// The Character to add before and after any table name. Add two characters to consider the first for the beginning and the second for the end. 
        /// Use it for databases where you add table names that follow this example: [myTableName].
        /// </summary>
        public string TableNameChar { get; protected set; }

        /// <summary>
        /// The default date format to be used by the dialect
        /// </summary>
        public string DateFormat { get; protected set; }

        /// <summary>
        /// The default time format to be used by the dialect
        /// </summary>
        public string TimeFormat { get; protected set; }

        /// <summary>
        /// The default Numeric format to be used by the dialect
        /// </summary>
        public string NumericFormat { get; protected set; }

        /// <summary>
        /// The default Decimal format to be used by the dialect
        /// </summary>
        public string DecimalFormat { get; protected set; }

        /// <summary>
        /// Any statement that might be executed right before running any query
        /// </summary>
        public List<string> BeforeQueryStatements { get; protected set; }

        /// <summary>
        /// Any statement that might be executed right after a query runs
        /// </summary>
        public List<string> AfterQueryStatements { get; protected set; }

        /// <summary>
        /// The character to be appended at the end of any statement
        /// </summary>
        public string StatementEndCharacter { get; protected set; }

        /// <summary>
        /// A Dictionary Containing the Operators
        /// </summary>
        public Dictionary<FieldOperators, string> Operators { get; protected set; }

        /// <summary>
        /// A Dictionary Containing the Aggregate Functions
        /// </summary>
        public Dictionary<Aggregates, string> AggregateFunctions { get; protected set; }

        /// <summary>
        /// A string to be added before the table name on all SQL statements
        /// </summary>
        public string TableNamePrefix { get; set; } = "";

        /// <summary>
        /// A string to be added after the table name on all SQL statements
        /// </summary>
        public string TableNameSuffix { get; set; } = "";

        #endregion Properties

        #region Quick Initialization Methods

        /// <summary>
        /// Adds many reserved words to the list
        /// </summary>
        /// <param name="list">List of reserved words</param>
        protected void AddReservedWords(params string[] list)
        {
            ReservedWords.AddRange(list);
        }

        /// <summary>
        /// Adds many sql statements to run before any query to the list
        /// </summary>
        /// <param name="list">The list of sql statements</param>
        protected void AddBeforeStatements(params string[] list)
        {
            BeforeQueryStatements.AddRange(list);
        }

        /// <summary>
        /// Adds many sql statements to run after any query to the list
        /// </summary>
        /// <param name="list">The list of sql statements</param>
        protected void AddAfterStatements(params string[] list)
        {
            AfterQueryStatements.AddRange(list);
        }

        /// <summary>
        /// Returns the final query to be executed, including the additional statements
        /// </summary>
        /// <param name="query">The current query</param>
        /// <returns>A query containing the pre and post statements</returns>
        public string GetFinalStatement(string query)
        {
            // Append the Before and After statements
            if (BeforeQueryStatements.Count > 0)
                query = String.Join($"{StatementEndCharacter}\r\n", BeforeQueryStatements.ToArray()) + "\r\n" + query;

            if (AfterQueryStatements.Count > 0)
                query = "\r\n" + query + String.Join($"{StatementEndCharacter}\r\n", AfterQueryStatements.ToArray());

            return query;
        }

        #endregion Quick Initialization Methods

        #region Standard SQL Methods

        #region Standard SQL Methods - Select

        /// <summary>
        /// Converts a Select Database Operation into a valid T-SQL Statement
        /// </summary>
        /// <param name="select">The <see cref="SelectDbOperation"/></param>
        /// <returns>A string containing the T-SQL</returns>
        public virtual string Select(SelectDbOperation select)
        {
            return SelectInto(select, null);
        }

        /// <summary>
        /// Converts a Select Into Database Operation into a valid T-SQL Statement
        /// </summary>
        /// <param name="select">The <see cref="SelectDbOperation"/></param>
        /// <param name="intoTable">The destination table</param>
        /// <returns>A string containing the T-SQL</returns>
        public virtual string SelectInto(SelectDbOperation select, Table intoTable)
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
                intoTable.Alias = "";
                sbSelect.Append("  INTO ");
                sbSelect.AppendLine(FormatTable(intoTable).PadLeft(7, ' '));
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

        #endregion Standard SQL Methods - Select

        #region Standard SQL Methods - Insert

        /// <summary>
        /// Converts an Insert Into Database Operation into a valid T-SQL Statement
        /// </summary>
        /// <param name="insert">The <see cref="InsertDbOperation"/></param>
        /// <returns>A string containing the T-SQL</returns>
        public virtual string Insert(InsertDbOperation insert)
        {
            // String Builders
            StringBuilder sbInsert = new StringBuilder();

            // Validate Table
            if (insert.Table == null)
                throw new Exception("Insert requires a table. Table is null.");

            // Make sure to remove alias from table
            insert.Table.Alias = "";

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
        /// Converts an Insert Into Database Operation with a Select Statement into a valid T-SQL Statement
        /// </summary>
        /// <param name="insert">The <see cref="InsertDbOperation"/></param>
        /// <param name="select">The <see cref="SelectDbOperation"/></param>
        /// <returns></returns>
        public virtual string InsertFromSelect(InsertDbOperation insert, SelectDbOperation select)
        {
            // String Builders
            StringBuilder sbInsert = new StringBuilder();

            // Validate Table
            if (insert.Table == null)
                throw new Exception("Insert requires a table. Table is null.");

            // Make sure to remove the alias
            insert.Table.Alias = "";

            if (String.IsNullOrEmpty(insert.Table.Name))
                throw new Exception("Insert requires a table. Table is blank or empty.");

            // Validate Select
            if (select == null)
                throw new Exception("A Select Statement must be provided");

            // Format for output
            sbInsert.Append("INSERT INTO ");
            sbInsert.AppendLine(FormatTable(insert.Table));

            // Create arrays for insert data
            if (insert.Assignments.Length > 0)
            {
                string[] insertFields = new string[insert.Assignments.Length];
                for (int f = 0; f < insert.Assignments.Length; f++)
                    insertFields[f] = FormatExpressionField(insert.Assignments[f].Field1);

                sbInsert.AppendLine("            (");
                sbInsert.Append("                    ");
                sbInsert.AppendLine(String.Join(",\r\n                    ", insertFields));
                sbInsert.AppendLine("            )");
            }

            sbInsert.AppendLine(Select(select));

            return sbInsert.ToString();

        }

        #endregion Standard SQL Methods - Insert

        #region Standard SQL Methods - Update

        /// <summary>
        /// Converts an Update Database Operation into a valid T-SQL Statement
        /// </summary>
        /// <param name="update">The <see cref="UpdateDbOperation"/></param>
        /// <returns>A string contaiing the T-SQL</returns>
        public virtual string Update(UpdateDbOperation update)
        {
            // String Builders
            StringBuilder sbUpdate = new StringBuilder();

            // Validate Table
            if (update.Table == null)
                throw new Exception("Update requires a table. Table is null.");

            // Make sure to remove the alias
            update.Table.Alias = "";

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

        #endregion Standard SQL Methods - Update

        #region Standard SQL Methods - Delete

        /// <summary>
        /// Converts a Delete Database Operation into a valid T-SQL Statement
        /// </summary>
        /// <param name="delete">The <see cref="DeleteDbOperation"/></param>
        /// <returns>A string contaiing the T-SQL</returns>
        public virtual string Delete(DeleteDbOperation delete)
        {
            // String Builders
            StringBuilder sbDelete = new StringBuilder();

            // Validate Table
            if (delete.Table == null)
                throw new Exception("Delete requires a table. Table is null.");

            // Make sure to remove alias from table
            delete.Table.Alias = "";

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


        #endregion Standard SQL Methods - Delete

        #endregion Standard SQL Methods

        #region Formatting Methods

        /// <summary>
        /// Formats a <see cref="DateTime"/> accordingly to the Database Dialect
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> to format</param>
        /// <returns>A <see cref="string"/> containing the formatted date</returns>
        protected virtual string FormatDate(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue) return "NULL";

            return $"{FieldValueChar}{dateTime.ToString($"{DateFormat} {TimeFormat}")}{FieldValueChar}";
        }

        /// <summary>
        /// Formats a <see cref="string"/> accordingly to the Database Dialect
        /// </summary>
        /// <param name="value">The string input</param>
        /// <returns>A <see cref="string"/> containing the formatted results</returns>
        protected virtual string FormatString(string value)
        {
            // Escape characters
            value = value.Replace(FieldValueChar, FieldValueChar + FieldValueChar);

            return $"{FieldValueChar}{value}{FieldValueChar}";
        }
        
        /// <summary>
        /// Formats the Operator accordingly to the database dialect
        /// </summary>
        /// <param name="operator">The opeperator</param>
        /// <returns>A string containing the formatted operator</returns>
        protected virtual string FormatOperator(FieldOperators @operator)
        {
            if (Operators == null)
                throw new Exception($"The '{nameof(Operators)}' Dictionary is null");

            if (Operators.Count == 0)
                throw new Exception($"The '{nameof(Operators)}' Dictionary is initialized but empty");

            if (!Operators.ContainsKey(@operator))
                throw new Exception($"The operator '{Enum.GetName(typeof(FieldOperators), @operator)}' is not defined");

            return Operators[@operator];
        }

        /// <summary>
        /// Formats a <see cref="bool"/> accordingly to the Database Dialect
        /// </summary>
        /// <param name="value">The bool input</param>
        /// <returns>A <see cref="string"/> containing the formatted results</returns>
        protected virtual string FormatBoolean(bool value)
        {
            return (value ? "1" : "0");
        }

        /// <summary>
        /// Formats a <see cref="List{T}"/> of expressions into a sql statement 
        /// </summary>
        /// <param name="expressions">A <see cref="List{T}"/> of <see cref="object"/> containing expressions. Only objects of type <see cref="FilterExpression"/> or an <see cref="Array"/> of <see cref="FilterExpression"/> are accepted.</param>
        /// <param name="identLevel">The identation level</param>
        /// <returns>A <see cref="string"/> containing the formatted expressions</returns>
        protected virtual string FormatExpressions(IFilterExpression[] expressions, int identLevel)
        {
            if (expressions == null) return "";
            if (expressions.Length == 0) return "";

            StringBuilder sbExpressions = new StringBuilder();

            // Loop thru expressions
            for (int iExp = 0; iExp < expressions.Length; iExp++)
            {
                IFilterExpression exp = expressions[iExp];

                // Check type of expression
                if (exp is GroupedFilterExpression gExp)
                {
                    // Grouped Filter Expression

                    // Make sure the expression isn't empty
                    if (gExp.Count == 0)
                        continue;

                    gExp[gExp.Count - 1].AndOr = FieldAndOr.None;

                    // Format Expressions
                    sbExpressions.Append(String.Empty.PadRight(7 * identLevel, ' '));
                    sbExpressions.AppendLine("(");

                    foreach (var eToFormat in gExp)
                    {
                        if (eToFormat is FilterExpression fExp_2)
                            sbExpressions.AppendLine(FormatExpression(fExp_2, identLevel + 1));
                        else if (eToFormat is GroupedFilterExpression fGExp_2)
                            sbExpressions.AppendLine(FormatExpressions(fGExp_2.ToArray<IFilterExpression>(), identLevel + 1));
                    }
                        
                    sbExpressions.Append(String.Empty.PadRight(7 * identLevel, ' '));
                    sbExpressions.Append(")");

                    if (gExp.AndOr == FieldAndOr.And)
                        sbExpressions.AppendLine(" AND");
                    else if (gExp.AndOr == FieldAndOr.Or)
                        sbExpressions.AppendLine(" OR");
                }
                else if (exp is FilterExpression fExp)
                {
                    // Simple Expression

                    // Automatically set last expression to "None"
                    if (iExp == expressions.Length - 1)
                        fExp.AndOr = FieldAndOr.None;

                    sbExpressions.AppendLine(FormatExpression(fExp, identLevel));
                }
                else
                {
                    // Invalid Type
                    throw new Exception($"Object of type '{exp.GetType().FullName}' cannot be considered an expression");
                }

                //if (exp.GetType() == typeof(GroupedFilterExpression))
                //{
                //    // Collection of Expressions
                //    System.Collections.IEnumerator e = null;

                //    if (exp.GetType() == typeof(Array))
                //        e = ((Array)exp).GetEnumerator();
                //    else if (exp.GetType() == typeof(List<FilterExpression>))
                //        e = ((List<FilterExpression>)exp).GetEnumerator();
                //    else if (exp.GetType() == typeof(FilterExpression[]))
                //        e = ((FilterExpression[])exp).GetEnumerator();

                //    if (e == null)
                //        throw new Exception("Unable to find enumerator for expression");

                //    // Move it to a manageable list
                //    List<FilterExpression> expressionsToFormat = new List<FilterExpression>();
                //    FieldAndOr lastAndOr = FieldAndOr.None;

                //    while ((e.MoveNext()) && (e.Current != null))
                //    {
                //        if (e.Current.GetType() == typeof(FilterExpression))
                //        {
                //            var currentExp = (FilterExpression)e.Current;
                //            expressionsToFormat.Add(currentExp);
                //            lastAndOr = currentExp.AndOr;
                //        }
                //    }

                //    // Update the last item with None for formatting
                //    if (expressionsToFormat.Count == 0)
                //        throw new Exception("Unable to identify Expressions inside the given array");

                //    expressionsToFormat[expressionsToFormat.Count - 1].AndOr = FieldAndOr.None;

                //    // Format Expressions
                //    sbExpressions.Append(String.Empty.PadRight(7 * identLevel, ' '));
                //    sbExpressions.AppendLine("(");

                //    foreach (var eToFormat in expressionsToFormat)
                //        sbExpressions.AppendLine(FormatExpression(eToFormat, identLevel+1));

                //    sbExpressions.Append(String.Empty.PadRight(7 * identLevel, ' '));
                //    sbExpressions.Append(")");

                //    if (lastAndOr == FieldAndOr.And)
                //        sbExpressions.AppendLine(" AND");
                //    else if (lastAndOr == FieldAndOr.Or)
                //        sbExpressions.AppendLine(" OR");

                //}
                //else if (exp.GetType() == typeof(FilterExpression))
                //{
                //    // Automatically set last expression to "None"
                //    if (iExp == expressions.Length -1)
                //        ((FilterExpression)exp).AndOr = FieldAndOr.None;

                //    sbExpressions.AppendLine(FormatExpression((FilterExpression)exp, identLevel));
                //}
                //else
                //{
                //    throw new Exception($"Object of type '{exp.GetType().FullName}' cannot be considered an expression");
                //}
            }

            return sbExpressions.ToString();
        }

        /// <summary>
        /// Format a <see cref="FilterExpression"/> into a SQL Statement
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="identLevel"></param>
        /// <returns></returns>
        protected virtual string FormatExpression(FilterExpression expression, int identLevel)
        {
            // Make sure expression is not null
            if (expression == null)
                throw new Exception("Null Expressions cannot be formatted");

            // Ensure left side is not null
            if (expression.Field1 == null)
                throw new Exception("The left side of the expression cannot be null");

            // ======================================
            // Create String Holder
            // ======================================
            StringBuilder sbExpression = new StringBuilder();
            string andOrConnector = "";

            // ======================================
            // Append Identation
            // ======================================
            sbExpression.Append(String.Empty.PadRight(7 * identLevel, ' '));

            // ======================================
            // Format AndOr
            // ======================================
            if (expression.AndOr == FieldAndOr.And)
                andOrConnector = " AND";
            else if (expression.AndOr == FieldAndOr.Or)
                andOrConnector = " OR";
            else
                andOrConnector = "";

            // ======================================
            // Format Left Side
            // ======================================
            sbExpression.Append(FormatExpressionField(expression.Field1));

            // ======================================
            // Format Operator and Right Side
            // ======================================
            string formattedOperator = FormatOperator(expression.Operator);

            // Some operators have placeholders (IN, NOT IN, LIKE, NOT LIKE, BETWEEN, NOT BETWEEN)
            if ((expression.Operator == FieldOperators.In) ||
                (expression.Operator == FieldOperators.NotIn) ||
                (expression.Operator == FieldOperators.Like) ||
                (expression.Operator == FieldOperators.NotIn))
            {
                sbExpression.Append(" ");
                sbExpression.Append(formattedOperator.Replace("{value}", FormatExpressionField(expression.Field2)));
            }
            else if ((expression.Operator == FieldOperators.IsNull) ||
                     (expression.Operator == FieldOperators.NotIsNull))
            {
                sbExpression.Append(" ");
                sbExpression.Append(formattedOperator);
            }
            else if ((expression.Operator == FieldOperators.Between) ||
                     (expression.Operator == FieldOperators.NotBetween))
            {
                // Make sure the Field2 is an array of two
                if (expression.Field2.GetType().IsArray)
                {
                    var a = (Array)expression.Field2;

                    if ((a.GetLength(0) == 0) || (a.GetLength(0) == 1))
                        throw new Exception("Array size is not valid for between operator. It must be at least two.");

                    string formattedBetween = formattedOperator.Replace("{value1}", FormatExpressionField(a.GetValue(0)));
                    formattedBetween = formattedBetween.Replace("{value2}", FormatExpressionField(a.GetValue(1)));

                    sbExpression.Append(" ");
                    sbExpression.Append(formattedBetween);
                }
            }
            else
            {
                sbExpression.Append(formattedOperator);
                sbExpression.Append(FormatExpressionField(expression.Field2));
            }

            sbExpression.Append(andOrConnector);

            return sbExpression.ToString();
        }

        /// <summary>
        /// Formats an Expression Field into a String
        /// </summary>
        /// <param name="field">An object representing a field</param>
        /// <returns>A <see cref="string"/> containingt he formatted object</returns>
        protected string FormatExpressionField(object field)
        {
            if (field == null) return "NULL";

            if (field.GetType() == typeof(AggregateField)) return FormatField((AggregateField)field);
            else if (field.GetType() == typeof(DisplayField)) return FormatField((DisplayField)field);
            else if (field.GetType() == typeof(Field)) return FormatField((Field)field);
            else if (field.GetType() == typeof(string)) return FormatString(field.ToString());
            else if (field.GetType() == typeof(DateTime)) return FormatDate((DateTime)field);
            else if (field.GetType() == typeof(bool)) return FormatBoolean((bool)field);
            else if (field.GetType() == typeof(SelectDbOperation)) return $"({Select((SelectDbOperation)field)})";
            else if (field.GetType() == typeof(List<object>))
            {
                return string.Join(",", from l in ((List<object>)field) select FormatExpressionField(l));
            }
            else return field.ToString();
        }

        /// <summary>
        /// Validates the table information
        /// </summary>
        /// <param name="table">The <see cref="Table"/> to be formatted</param>
        protected virtual void ValidateTable(Table table)
        {
            // Remove any blanks from the string
            TableNameChar = TableNameChar.Trim();

            // Validate Table Name Char (might not have more than two characters)
            if (TableNameChar.Length > 2) throw new Exception($"'{nameof(TableNameChar)}' cannot have more than two characters. Current value is '{TableNameChar}'");

            // Look for the separator inside it
            if (TableNameChar != "")
            {
                /* There is a table name char */
                /* On this case, it's acceptable to have inner blank spaces */

                if (TableNameChar.Length == 2)
                {
                    // There is a begin / end character
                    if ((table.Name.Contains(TableNameChar[0].ToString())) &&
                        (table.Name.Contains(TableNameChar[1].ToString())))
                    {
                        /* There is a problem , the table name should not have the separator character inside it */
                        throw new Exception($"Table name '{table.Name}' is invalid. A table cannot have the separator '{TableNameChar}' on it");
                    }
                }
                else
                {
                    // There is only one character
                    if (table.Name.Contains(TableNameChar[0].ToString()))
                    {
                        /* There is a problem , the table name should not have the separator character inside it */
                        throw new Exception($"Table name '{table.Name}' is invalid. A table cannot have the separator '{TableNameChar}' on it");
                    }
                }
            }
            else
            {
                /* There is no table name char */
                /* On this case, it's not acceptable to have inner blank spaces */

                // Look for blank spaces
                if (table.Name.Contains(" "))
                {
                    /* There are blank spaces inside it, consider invalid */
                    throw new Exception($"Table name '{table.Name}' is invalid. It contains blank spaces but the dialect for this database does not have a valid '{nameof(TableNameChar)}'");
                }
            }
        }

        /// <summary>
        /// Returns the Table Formatted
        /// </summary>
        /// <param name="tableObject">The <see cref="Table"/> object</param>
        /// <returns>A <see cref="string"/> containing the formatted table</returns>
        protected virtual string FormatTable(object tableObject)
        {
            // The table object to be parsed
            Table table;

            if (tableObject.GetType() == typeof(Table))
                table = (Table)tableObject;
            else if (tableObject.GetType() == typeof(string))
                table = new Table(tableObject.ToString());
            else
                throw new Exception($"Table object of type '{tableObject.GetType().FullName}' is not acceptable as a table");

            // Variable to hold the Table Name
            string FormattedTableName = "";
            string CharacterBegin = "";
            string CharacterEnd = "";

            // Check if the table name is valid
            ValidateTable(table);

            if (TableNameChar.Length == 2)
            {
                // Set Character Begin / End
                CharacterBegin = TableNameChar[0].ToString();
                CharacterEnd = TableNameChar[1].ToString();
            }
            else if (TableNameChar.Length == 1)
            {
                // Set Character Begin / End (the same)
                CharacterBegin = TableNameChar[0].ToString();
                CharacterEnd = TableNameChar[0].ToString();
            }

            // Verify Owner
            if (table?.Owner?.ToString() != "")
            {
                // There is an owner, add it to the string
                FormattedTableName += $"{CharacterBegin}{table.Owner}{CharacterEnd}.";
            }

            // Add the table name with Prefix and Suffix
            FormattedTableName += $"{CharacterBegin}{TableNamePrefix}{table.Name}{TableNameSuffix}{CharacterEnd}";

            // Verify Alias
            if (table?.Alias?.ToString() != "")
            {
                // For security reasons (avoid users passing additional commands on the table alias), add the "T_" before the table alias.
                FormattedTableName += $" T_{table.Alias}";
            }

            return FormattedTableName;
        }
        
        /// <summary>
        /// Validate the field
        /// </summary>
        /// <param name="field">The <see cref="Field"/> object to be validated</param>
        protected virtual void ValidateField(Field field)
        {
            // Remove any blanks from the string
            FieldNameChar = FieldNameChar.Trim();

            // Validate Field Name Char (might not have more than two characters)
            if (FieldNameChar.Length > 2) throw new Exception($"The {nameof(FieldNameChar)} has an invalid value. It may not be over 2 characters.");

            // Look for the separator inside it
            if (FieldNameChar != "")
            {
                /* There is a Field Name Char */
                /* On this case, it's acceptable to have inner blank spaces */
                if (FieldNameChar.Length == 2)
                {
                    // There is a begin / end character
                    if ((field.Name.Contains(FieldNameChar[0].ToString())) &&
                        (field.Name.Contains(FieldNameChar[1].ToString())))
                    {
                        /* There is a problem , the table name should not have the separator character inside it */
                        throw new Exception($"The {nameof(FieldNameChar)} was found inside the {nameof(field)} ({field.Name}).");
                    }
                }
                else
                {
                    // There is only one character
                    if (field.Name.Contains(FieldNameChar[0].ToString()))
                    {
                        /* There is a problem , the table name should not have the separator character inside it */
                        throw new Exception($"The {nameof(FieldNameChar)} was found inside the {nameof(field)} ({field.Name}).");
                    }
                }
            }
            else
            {
                /* There is no field name char */
                /* On this case, it's not acceptable to have inner blank spaces */

                // Look for blank spaces
                if (field.Name.Contains(" "))
                {
                    /* There are blank spaces inside it, consider invalid */
                    throw new Exception($"Field name '{field.Name}' is invalid. It's not possible to have blank characters inside the field name unless you have a {nameof(FieldNameChar)} configured.");
                }
            }
        }

        /// <summary>
        /// Formats the field to a valid string appendable to a query
        /// </summary>
        /// <param name="field">The <see cref="Field"/> to be formatted</param>
        /// <returns>A <see cref="string"/> with the formatted field.</returns>
        public virtual string FormatField(Field field)
        {
            // Checks if the field is valid
            ValidateField(field);

            // Begin/End Strings
            string CharacterBegin = "";
            string CharacterEnd = "";

            if (FieldNameChar.Length == 2)
            {
                // Set Character Begin / End
                CharacterBegin = FieldNameChar[0].ToString();
                CharacterEnd = FieldNameChar[1].ToString();
            }
            else if (FieldNameChar.Length == 1)
            {
                // Set Character Begin / End (the same)
                CharacterBegin = FieldNameChar[0].ToString();
                CharacterEnd = FieldNameChar[0].ToString();
            }

            // Return the formatted field
            if (field is AggregateField af)
            {
                if (AggregateFunctions.ContainsKey(af.Function))
                {
                    string tempFunctionName = AggregateFunctions[af.Function];

                    if ((field.Name == "0") || (field.Name == "*"))
                    {
                        tempFunctionName = tempFunctionName.Replace("{value}", field.Name);
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(af.TableAlias))
                            tempFunctionName = tempFunctionName.Replace("{value}", $"T_{af.TableAlias}.{CharacterBegin}{field.Name}{CharacterEnd}");
                        else
                            tempFunctionName = tempFunctionName.Replace("{value}", $"{CharacterBegin}{field.Name}{CharacterEnd}");
                    }

                    return tempFunctionName;
                }
                else
                {
                    throw new Exception($"Aggregate function '{Enum.GetName(typeof(Aggregates), af.Function)}' is not configured for this database dialect");
                }
            }
            else if (field is DisplayField dpf)
            {
                StringBuilder sbDisplayField = new StringBuilder();

                if (!String.IsNullOrEmpty(dpf.TableAlias))
                    sbDisplayField.Append($"T_{dpf.TableAlias}.{CharacterBegin}{field.Name}{CharacterEnd}");
                else
                    sbDisplayField.Append($"{CharacterBegin}{field.Name}{CharacterEnd}");

                if (!String.IsNullOrEmpty(dpf.AlternateName))
                    sbDisplayField.Append(" ").Append(dpf.AlternateName);

                return sbDisplayField.ToString();
            }
            else if (field is OrderByField obf)
            {
                StringBuilder sbOrderByField = new StringBuilder();

                if (!String.IsNullOrEmpty(obf.TableAlias))
                    sbOrderByField.Append($"T_{obf.TableAlias}.{CharacterBegin}{field.Name}{CharacterEnd}");
                else
                    sbOrderByField.Append($"{CharacterBegin}{field.Name}{CharacterEnd}");

                return sbOrderByField.ToString();
            }
            else
            {
                // Return the formatted field
                return $"{CharacterBegin}{field.Name}{CharacterEnd}";
            }

        }

        #endregion Formatting Methods

    }
}
