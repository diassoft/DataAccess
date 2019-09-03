using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.FilterParsers
{
    /// <summary>
    /// Represents a field to be filtered
    /// </summary>
    public class FilterField
    {
        /// <summary>
        /// The field name
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// The filter operation
        /// </summary>
        public FilterFieldOperations Operator { get; set; }
        /// <summary>
        /// The original value in string
        /// </summary>
        public string OriginalValue { get; set; }
        /// <summary>
        /// An object representing the field value
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// The Type of the filter field
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterField"/>
        /// </summary>
        public FilterField(): this("", FilterFieldOperations.Equal, null, null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterField"/>
        /// </summary>
        /// <param name="field">The field name</param>
        /// <param name="operator">the filter operation</param>
        /// <param name="value">An object representing the field value</param>
        /// <param name="type">The type of the filter field</param>
        public FilterField(string field, FilterFieldOperations @operator, object value, Type type) : this(field, @operator, value, type, "") { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterField"/>
        /// </summary>
        /// <param name="field">The field name</param>
        /// <param name="operator">the filter operation</param>
        /// <param name="value">An object representing the field value</param>
        /// <param name="type">The type of the filter field</param>
        /// <param name="originalValue">A string containing the original value</param>
        public FilterField(string field, FilterFieldOperations @operator, object value, Type type, string originalValue)
        {
            Field = field;
            Operator = @operator;
            Value = value;
            Type = type;
            OriginalValue = originalValue;
        }

        /// <summary>
        /// Convert the filter into a list
        /// </summary>
        /// <returns></returns>
        public List<FilterField> ToList()
        {
            return new List<FilterField>
            {
                this
            };
        }
    }

    /// <summary>
    /// Valid Values for Filter Operations
    /// </summary>
    public enum FilterFieldOperations : int
    {
        /// <summary>
        /// Equal
        /// </summary>
        Equal = 0,
        /// <summary>
        /// Not Equal
        /// </summary>
        NotEqual = 1,
        /// <summary>
        /// Belong to the List of Values
        /// </summary>
        In = 2,
        /// <summary>
        /// Do not belong to the List of Values
        /// </summary>
        NotIn = 3,
        /// <summary>
        /// Greater Than
        /// </summary>
        GreaterThan = 4,
        /// <summary>
        /// Greater than or equal to
        /// </summary>
        GreaterThanOrEqualTo = 5,
        /// <summary>
        /// Less Than
        /// </summary>
        LessThan = 6,
        /// <summary>
        /// Less Than or Equal To
        /// </summary>
        LessThanOrEqualTo = 7
    }
}
