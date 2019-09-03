using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.DatabaseObjects.Fields
{
    /// <summary>
    /// Represents the base for any Field
    /// </summary>
    public class Field
    {
        /// <summary>
        /// The Field Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Field Type according to the <see cref="FieldTypes"/>.
        /// </summary>
        public FieldTypes Type { get; set; }

        /// <summary>
        /// Initializes a new instance of a <see cref="Field"/>.
        /// </summary>
        public Field(): this("") { }

        /// <summary>
        /// Initializes a new instance of a <see cref="Field"/>.
        /// </summary>
        public Field(string name): this(name, FieldTypes.None) { }

        /// <summary>
        /// Initializes a new instance of a <see cref="Field"/>.
        /// </summary>
        /// <param name="name">The Field Name</param>
        /// <param name="type">The Field Type according to the <see cref="FieldTypes"/> enumeration.</param>
        public Field(string name, FieldTypes type)
        {
            Name = name;
            Type = type;
        }
    }

}
