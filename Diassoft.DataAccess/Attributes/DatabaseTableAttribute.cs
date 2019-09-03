using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.Attributes
{
    /// <summary>
    /// Represents an Attribute to be used to decorate Database Tables
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DatabaseTableAttribute: System.Attribute
    {
        /// <summary>
        /// The Database Table Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTableAttribute"/>
        /// </summary>
        public DatabaseTableAttribute(): this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTableAttribute"/>
        /// </summary>
        /// <param name="name">The database table name</param>
        public DatabaseTableAttribute(string name)
        { 
            Name = name;
        }

    }
}
