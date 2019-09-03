using System;
using System.Collections.Generic;
using System.Text;
using Diassoft.DataAccess.DatabaseObjects;
using Diassoft.DataAccess.DatabaseObjects.Fields;
using Diassoft.DataAccess.DatabaseObjects.Expressions;
using System.Data;
using System.Linq;
using System.Collections;

namespace Diassoft.DataAccess.Operations
{
    /// <summary>
    /// A class that represents a Select database operation
    /// </summary>
    public class SelectDbOperation: DbOperation<object[]>
    {

        #region Constructors

        /* Since this is using a inheritance structure, it's necessary to redefine the methods on this base class.
         * It's going to call the Base Methods always, there is no need to implement any logic here */

        /// <summary>
        /// Initializes a new instance of the Select Database Operation
        /// </summary>
        public SelectDbOperation() : this("") { }

        /// <summary>
        /// Initializes a new instance of the Select Database Operation
        /// </summary>
        /// <param name="tableName">The table name for the select statement</param>
        public SelectDbOperation(string tableName) : this(String.IsNullOrEmpty(tableName) ? null : new Table(tableName)) { }

        /// <summary>
        /// Initializes a new instance of the Select Database Operation
        /// </summary>
        /// <param name="table">Table to Sql against</param>
        public SelectDbOperation(Table table) : base()
        {
            Distinct = false;
            TopNRows = 0;
            GroupBy = false;

            if (table != null)
                Table = new Table[] { table };

            SelectFields = new FieldCollection();
            Where = new WhereCollection();
            OrderBy = new OrderByCollection();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Defines whether the select will be of the distinct fields.
        /// By default the value is false.
        /// </summary>
        public bool Distinct { get; set; } = false;

        /// <summary>
        /// Defines the Top N Rows to be displayed.
        /// By default the value is zero.
        /// </summary>
        public int TopNRows { get; set; } = 0;

        /// <summary>
        /// Defines whether there will be a Group By at the end of the query
        /// </summary>
        public bool GroupBy { get; set; } = false;

        /// <summary>
        /// A list with the fields to be selected, including aggregates
        /// </summary>
        public FieldCollection SelectFields { get; set; }

        /// <summary>
        /// An <see cref="WhereCollection"/> containing <see cref="FilterExpression"/> or <see cref="GroupedFilterExpression"/> representing the filter criterias for the query
        /// </summary>
        /// <remarks>Accept objects that implement the <see cref="IFilterExpression"/> interface.</remarks>
        public WhereCollection Where { get; set; }

        /// <summary>
        /// A <see cref="OrderByCollection"/> containing the <see cref="OrderByField">OrderByFields</see> to be applied for sorting the data
        /// </summary>
        public OrderByCollection OrderBy { get; set; }

        #endregion Properties
        
    }
}
