using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.DatabaseObjects.Fields
{
    /// <summary>
    /// Represents a field to be displayed on a Select Statement
    /// </summary>
    public class DisplayField: Field
    {

        #region Properties

        /// <summary>
        /// The Table Alias
        /// </summary>
        public string TableAlias { get; set; }

        /// <summary>
        /// The alternative name to assign to this field.
        /// </summary>
        /// <remarks>By default, all fields return their own names on the result set. This property allows you to change that behavior.</remarks>
        public string AlternateName { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of a <see cref="DisplayField"/>.
        /// </summary>
        /// <param name="name">The Field Name</param>
        public DisplayField(string name): base(name, FieldTypes.Display) { }

        /// <summary>
        /// Initializes a new instance of a <see cref="DisplayField"/>.
        /// </summary>
        /// <param name="name">The Field Name</param>
        /// <param name="tableAlias">The Table Alias</param>
        public DisplayField(string name, string tableAlias) : this(name)
        {
            TableAlias = tableAlias;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="DisplayField"/>.
        /// </summary>
        /// <param name="name">The Field Name</param>
        /// <param name="tableAlias">The Table Alias</param>
        /// <param name="alternateName">An alternative name to give to the field</param>
        public DisplayField(string name, string tableAlias, string alternateName) : this(name, tableAlias)
        {
            AlternateName = alternateName;
        }

        #endregion Constructors

    }
}
