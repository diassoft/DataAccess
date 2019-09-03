using Diassoft.DataAccess.DatabaseObjects;
using Diassoft.DataAccess.DatabaseObjects.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.Operations
{
    /// <summary>
    /// A class that represents a Delete database operation
    /// </summary>
    public class DeleteDbOperation: DbOperation<Table>
    {

        /// <summary>
        /// An <see cref="WhereCollection"/> containing <see cref="FilterExpression"/> or <see cref="GroupedFilterExpression"/> representing the filter criterias for the query
        /// </summary>
        /// <remarks>Accept objects that implement the <see cref="IFilterExpression"/> interface.</remarks>
        public WhereCollection Where { get; set; }

        /// <summary>
        /// Initializes a new <see cref="InsertDbOperation"/>
        /// </summary>
        public DeleteDbOperation() : this((Table)null) { }

        /// <summary>
        /// Initializes a new <see cref="DeleteDbOperation"/>
        /// </summary>
        /// <param name="tableName">A <see cref="string"/> representing the object to be modified</param>
        public DeleteDbOperation(string tableName) : this(new Table(tableName)) { }

        /// <summary>
        /// Initializes a new <see cref="DeleteDbOperation"/>
        /// </summary>
        /// <param name="table">A <see cref="Table"/> representing the object to be modified</param>
        public DeleteDbOperation(Table table)
        {
            base.Table = table;

            // Always remove the alias on an insert operation
            base.Table.Alias = "";

            Where = new WhereCollection();
        }
    }
}
