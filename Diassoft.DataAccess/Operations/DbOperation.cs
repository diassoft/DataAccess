using Diassoft.DataAccess.DatabaseObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Diassoft.DataAccess.Operations
{
    /// <summary>
    /// Represents the Base of Any Database Operation Class
    /// </summary>
    public abstract class DbOperation<T>
    {
        /// <summary>
        /// The Tables to be changed
        /// </summary>
        public T Table { get; set; }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Database Operation
        /// </summary>
        protected DbOperation()
        {

        }

        #endregion Constructors

    }
}
