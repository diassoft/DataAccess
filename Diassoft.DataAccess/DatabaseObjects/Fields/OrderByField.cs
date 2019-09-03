using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.DatabaseObjects.Fields
{
    /// <summary>
    /// Represents a field to be used in an Order By
    /// </summary>
    public class OrderByField: Field
    {
        /// <summary>
        /// The Table Alias
        /// </summary>
        public string TableAlias { get; set; }
        /// <summary>
        /// Represents the Sorting Mode
        /// </summary>
        public SortModes SortMode { get; set; }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of a <see cref="OrderByField"/>.
        /// </summary>
        /// <param name="name">The Field Name</param>
        public OrderByField(string name) : this(name, "", SortModes.Ascending) { }

        /// <summary>
        /// Initializes a new instance of a <see cref="OrderByField"/>.
        /// </summary>
        /// <param name="name">The Field Name</param>
        /// <param name="sortMode">The Sorting Mode (Ascending or Descending)</param>
        public OrderByField(string name, SortModes sortMode) : this(name, "", sortMode) { }

        /// <summary>
        /// Initializes a new instance of a <see cref="OrderByField"/>.
        /// </summary>
        /// <param name="name">The Field Name</param>
        /// <param name="tableAlias">The Table Alias</param>
        /// <param name="sortMode">The Sorting Mode (Ascending or Descending)</param>
        public OrderByField(string name, string tableAlias, SortModes sortMode) : base(name)
        {
            TableAlias = tableAlias;
            SortMode = sortMode;
        }

        #endregion Constructors
    }

    /// <summary>
    /// Representing the Sorting Modes
    /// </summary>
    public enum SortModes: int
    {
        /// <summary>
        /// Sorts the data ascendingly
        /// </summary>
        Ascending = 0,
        /// <summary>
        /// Sorts the data descendingly
        /// </summary>
        Descending = 1
    }
}
