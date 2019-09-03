using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.DatabaseObjects
{
    /// <summary>
    /// Represents a Table in the Database
    /// </summary>
    public class Table
    {
        /// <summary>
        /// The table Owner. Some databases name it as library.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// The table name without any formatting
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An alias for the table name (used in Select Statements)
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Initializes a new instance of a <see cref="Table"/> database object
        /// </summary>
        public Table()
        {
            Owner = "";
            Name = "";
            Alias = "";
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="Table"/> database object
        /// </summary>
        /// <param name="tableName">The table name without any formatting</param>
        public Table(string tableName) : this()
        {
            Name = tableName;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="Table"/> database object
        /// </summary>
        /// <param name="tableName">The table name without any formatting</param>
        /// <param name="tableOwner">The table Owner. Some databases name it as library.</param>
        public Table(string tableName, string tableOwner) : this(tableName)
        {
            Owner = tableOwner;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="Table"/> database object
        /// </summary>
        /// <param name="tableName">The table name without any formatting</param>
        /// <param name="tableOwner">The table Owner. Some databases name it as library.</param>
        /// <param name="tableAlias">An alias for the table name (used in Select Statements)</param>
        public Table(string tableName, string tableOwner, string tableAlias): this(tableName, tableOwner)
        {
            Alias = tableAlias;
        }

    }
}
