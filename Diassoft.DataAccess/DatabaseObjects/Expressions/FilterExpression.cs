using Diassoft.DataAccess.DatabaseObjects.Fields;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.DatabaseObjects.Expressions
{
    /// <summary>
    /// Represents an Expression to be used in a T-SQL Statement
    /// </summary>
    public class FilterExpression: IFilterExpression
    {
        /// <summary>
        /// The <see cref="FieldOperators">FieldOperator</see>
        /// </summary>
        public FieldOperators Operator { get; set; }

        /// <summary>
        /// The left side of the expression
        /// </summary>
        public object Field1 { get; set; }

        /// <summary>
        /// The right side of the expression
        /// </summary>
        public object Field2 { get; set; }

        /// <summary>
        /// The connection between this and the next expression
        /// </summary>
        public FieldAndOr AndOr { get; set; }

        /// <summary>
        /// Defines whether the expression should be enclosed between parenthesis
        /// </summary>
        public bool Enclose { get; } = false;

        /// <summary>
        /// Initializes a new <see cref="FilterExpression"/>
        /// </summary>
        public FilterExpression(): this(null, FieldOperators.None, null, FieldAndOr.None) { }

        /// <summary>
        /// Initializes a new <see cref="FilterExpression"/>
        /// </summary>
        /// <param name="field1">The left side of the expression</param>
        /// <param name="operator">The operator</param>
        /// <param name="field2">The right side of the expression</param>
        /// <param name="andOr">The connection between this and the next expression</param>
        public FilterExpression(object field1, FieldOperators @operator, object field2, FieldAndOr andOr)
        {
            AndOr = andOr;
            Field1 = field1;
            Operator = @operator;
            Field2 = field2;
        }

    }
}
