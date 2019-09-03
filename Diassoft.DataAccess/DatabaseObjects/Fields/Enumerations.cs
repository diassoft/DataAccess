using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.DatabaseObjects.Fields
{
    /// <summary>
    /// A list of Database Field Types 
    /// </summary>
    public enum FieldTypes : int
    {
        /// <summary>
        /// Undefined field type
        /// </summary>
        None = 0,
        /// <summary>
        /// A Text Field (string, varchar, char)
        /// </summary>
        String = 1,
        /// <summary>
        /// A Numeric Field
        /// </summary>
        Numeric = 2,
        /// <summary>
        /// A Numeric Field with Decimal Places
        /// </summary>
        Decimal = 3,
        /// <summary>
        /// A DateTime field
        /// </summary>
        DateTime = 4,
        /// <summary>
        /// A Date Field
        /// </summary>
        Date = 5,
        /// <summary>
        /// A Time Field
        /// </summary>
        Time = 6,
        /// <summary>
        /// A <see cref="System.Boolean">Boolean</see> Field (stores 0 for false and 1 for true)"/>
        /// </summary>
        Boolean = 7,
        /// <summary>
        /// Represents a field to be displayed in a select statement.
        /// </summary>
        Display = 90,
        /// <summary>
        /// Represents an aggregate field on an Sql Statement.
        /// </summary>
        Aggregate = 91,
        /// <summary>
        /// Represents a field to be filtered according to another field.
        /// </summary>
        FieldFilter = 92
    }

    /// <summary>
    /// A list of Operators
    /// </summary>
    public enum FieldOperators : int
    {
        /// <summary>
        /// Undefined Operator
        /// </summary>
        None = 0,
        /// <summary>
        /// Equal Operator (when a value is equal another value)
        /// </summary>
        Equal = 1,
        /// <summary>
        /// NotEqual Operator (when a value is different than another value)
        /// </summary>
        NotEqual = 2,
        /// <summary>
        /// GreaterThan Operator (when a value is greater than another value)
        /// </summary>
        GreaterThan = 3,
        /// <summary>
        /// LessThan Operator (when a value is less than another value)
        /// </summary>
        LessThan = 4,
        /// <summary>
        /// GreaterThanOrEqual Operator (when a value is greater than or equal to another value)
        /// </summary>
        GreaterThanOrEqual = 5,
        /// <summary>
        /// LessThanOrEqual Operator (when a value is less than or equal to another value)
        /// </summary>
        LessThanOrEqual = 6,
        /// <summary>
        /// Like Operator (when a value is similar to another value).
        /// </summary>
        /// <remarks>
        /// When using this operator, you need to add the symbol '%' to the text. If there isn't one, it will be added to the end by the engine.
        /// The engine will use the proper symbol when necessary.
        /// </remarks>
        Like = 7,
        /// <summary>
        /// Not Like Operator (when a value is not similar to another value).
        /// </summary>
        /// <remarks>
        /// When using this operator, you need to add the symbol '%' to the text. If there isn't one, it will be added to the end by the engine.
        /// The engine will use the proper symbol when necessary.
        /// </remarks>
        NotLike = 8,
        /// <summary>
        /// In Operator (when a value is in the list of values)
        /// </summary>
        In = 9,
        /// <summary>
        /// NotIn Operator (when a value is not in the list of values)
        /// </summary>
        NotIn = 10,
        /// <summary>
        /// IsNull Operator (when the value is null)
        /// </summary>
        IsNull = 11,
        /// <summary>
        /// NotIsNull Operator (when the value is not null)
        /// </summary>
        NotIsNull = 12,
        /// <summary>
        /// Between Operator (when the value is between two values)
        /// </summary>
        Between = 13,
        /// <summary>
        /// NotBetween Operator (when the value is not between two values)
        /// </summary>
        NotBetween = 14
    }

    /// <summary>
    /// Syntax Connector
    /// </summary>
    public enum FieldAndOr : int
    {
        /// <summary>
        /// Undefined Connector
        /// </summary>
        None = 0,
        /// <summary>
        /// Combine two syntaxes with the AND operator
        /// </summary>
        And = 1,
        /// <summary>
        /// Combine two syntaxes with the OR operator
        /// </summary>
        Or = 2
    }

    /// <summary>
    /// List of Valid Aggregate Functions
    /// </summary>
    public enum Aggregates: int
    {
        /// <summary>
        /// Count of Records
        /// </summary>
        Count = 0,
        /// <summary>
        /// Maximum Value of a Field
        /// </summary>
        Max = 1,
        /// <summary>
        /// Minimum Value of a Field
        /// </summary>
        Min = 2,
        /// <summary>
        /// Average Value of a Field
        /// </summary>
        Average = 3,
        /// <summary>
        /// Count of Distinct Records (to avoid duplication)
        /// </summary>
        CountDistinct = 4,
        /// <summary>
        /// Sum of the Value of a Field
        /// </summary>
        Sum = 5
    }

}
