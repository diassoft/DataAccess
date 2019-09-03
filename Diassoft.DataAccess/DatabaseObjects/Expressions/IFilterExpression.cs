using Diassoft.DataAccess.DatabaseObjects.Fields;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.DatabaseObjects.Expressions
{
    /// <summary>
    /// Defines the Interface to be implemented by Expressions
    /// </summary>
    /// <remarks>Expressions are used in the Where and Having clauses</remarks>
    public interface IFilterExpression
    {
        /// <summary>
        /// The connection between this and the next expression
        /// </summary>
        FieldAndOr AndOr { get; set; }
        /// <summary>
        /// Defines whether the expression should be enclosed in parenthesis or not
        /// </summary>
        bool Enclose { get; }
    }
}
