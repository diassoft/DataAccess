using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.Attributes
{
    /// <summary>
    /// Represents an attribute to be used to decorate Database Fields
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DatabaseFieldAttribute: System.Attribute
    {
        /// <summary>
        /// The Database Field Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The Database Field Type
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// The Database Field Size
        /// </summary>
        /// <remarks>Zero menas the system will not apply any character limit to the field</remarks>
        public int Size { get; set; }
        /// <summary>
        /// The Database Field Number of Decimal Places
        /// </summary>
        public int Decimals { get; set; }
        /// <summary>
        /// Defines whether the field is part of the Primary Key.
        /// </summary>
        /// <remarks>Primary Key fields are used to define the Database Operations of Update and Delete. They are also used on the Creation of a Record to validate if it doesn't exists already.</remarks>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseFieldAttribute"/>
        /// </summary>
        public DatabaseFieldAttribute(): this((string)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseFieldAttribute"/>
        /// </summary>
        /// <param name="type">The database field type</param>
        public DatabaseFieldAttribute(Type type) : this(type, 0) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseFieldAttribute"/>
        /// </summary>
        /// <param name="type">The database field type</param>
        /// <param name="size">The database field size</param>
        public DatabaseFieldAttribute(Type type, int size) : this(null, type, size, 0) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseFieldAttribute"/>
        /// </summary>
        /// <param name="type">The database field type</param>
        /// <param name="size">The database field size</param>
        /// <param name="decimals">The database field decimals</param>
        public DatabaseFieldAttribute(Type type, int size, int decimals) : this(null, type, size, decimals) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseFieldAttribute"/>
        /// </summary>
        /// <param name="name">The database field name</param>
        public DatabaseFieldAttribute(string name): this(name, typeof(string), 0) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseFieldAttribute"/>
        /// </summary>
        /// <param name="name">The database field name</param>
        /// <param name="type">The database field type</param>
        /// <param name="size">The database field size</param>
        public DatabaseFieldAttribute(string name, Type type, int size): this(name, type, size, 0) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseFieldAttribute"/>
        /// </summary>
        /// <param name="name">The database field name</param>
        /// <param name="type">The database field type</param>
        /// <param name="size">The database field size</param>
        /// <param name="decimals">The database field decimals</param>
        public DatabaseFieldAttribute(string name, Type type, int size, int decimals)
        {
            this.Name = name;
            this.Type = type;
            this.Size = size;
            this.Decimals = decimals;
        }


    }
}
