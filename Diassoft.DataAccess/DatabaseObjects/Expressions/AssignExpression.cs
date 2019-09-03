using Diassoft.DataAccess.DatabaseObjects.Fields;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.DatabaseObjects.Expressions
{
    /// <summary>
    /// Represents an Assign Expression to be used by Inserts and Updates
    /// </summary>
    public class AssignExpression
    {
        /// <summary>
        /// The left side of the expression
        /// </summary>
        public object Field1 { get; set; }

        /// <summary>
        /// The right side of the expression
        /// </summary>
        public object Field2 { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignExpression"/>
        /// </summary>
        public AssignExpression(): this(null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignExpression"/>
        /// </summary>
        /// <param name="field1">The left side of the assignment</param>
        /// <param name="field2">The right side of the assignment</param>
        public AssignExpression(object field1, object field2)
        {
            // Force String Contents to be considered as a field
            if (field1.GetType() == typeof(string))
                Field1 = new Field(field1.ToString());
            else
                Field1 = field1;

            Field2 = field2;
        }
    }
}
