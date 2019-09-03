using Diassoft.DataAccess;
using Diassoft.DataAccess.DatabaseObjects.Fields;
using Diassoft.DataAccess.FilterParsers;
using Diassoft.DataAccess.Operations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Diassoft.DataAccess.Extensions
{
    /// <summary>
    /// Represents Extension Methods for the Diassoft DataAccess DbContext
    /// </summary>
    public static class DiassoftDataAccessDbContextExtensions
    {

        /// <summary>
        /// Perform a Select Count into a table using specific filters
        /// </summary>
        /// <param name="dbContext">Reference to the Database context</param>
        /// <param name="tableName">The table to search for data</param>
        /// <param name="dbConnection">Reference to an existing database connection</param>
        /// <param name="dbTransaction">Reference to an existing database transaction</param>
        /// <param name="filterFields">Array of filter fields</param>
        /// <returns>A <see cref="int"/> containing the Count of rows for a given select</returns>
        public static int SelectCount(this IDbContext dbContext, string tableName, IDbConnection dbConnection, IDbTransaction dbTransaction, params FilterField[] filterFields)
        {
            var select = new SelectDbOperation(tableName)
            {
                SelectFields = new FieldCollection()
                {
                    new AggregateField(Aggregates.Count, "0")
                }
            };

            select.Where.AppendQueryFilters(filterFields, false);

            return (int)dbContext.ExecuteScalar(select, dbConnection, dbTransaction);
        }

    }
}
