using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.DatabaseObjects.Fields
{
    /// <summary>
    /// Defines an Aggregate Field, which is an operation that is applied on a database field.
    /// </summary>
    public sealed class AggregateField : DisplayField
    {
        #region Properties

        /// <summary>
        /// An <see cref="Aggregates">Aggregate Function</see> to define which calculation will be applied on the database field.
        /// </summary>
        public Aggregates Function { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of a <see cref="AggregateField"/>.
        /// </summary>
        /// <param name="function">The <see cref="Aggregates">Aggregate Function</see></param>
        /// <param name="name">The Field Name</param>
        public AggregateField(Aggregates function, string name) : base(name)
        {
            Function = function;
            base.Type = FieldTypes.Aggregate;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="AggregateField"/>.
        /// </summary>
        /// <param name="function">The <see cref="Aggregates">Aggregate Function</see></param>
        /// <param name="name">The Field Name</param>
        /// <param name="tableAlias">The Table Alias</param>
        public AggregateField(Aggregates function, string name, string tableAlias) : this(function, name)
        {
            TableAlias = tableAlias;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="AggregateField"/>.
        /// </summary>
        /// <param name="function">The <see cref="Aggregates">Aggregate Function</see></param>
        /// <param name="name">The Field Name</param>
        /// <param name="tableAlias">The Table Alias</param>
        /// <param name="alternateName">An alternative name to give to the field</param>
        public AggregateField(Aggregates function, string name, string tableAlias, string alternateName) : this(function, name, tableAlias)
        {
            AlternateName = alternateName;
        }

        #endregion Constructors

    }
}
