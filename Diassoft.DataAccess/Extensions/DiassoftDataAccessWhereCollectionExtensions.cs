using System;
using System.Collections.Generic;
using System.Text;
using Diassoft.DataAccess.DatabaseObjects.Expressions;
using Diassoft.DataAccess.DatabaseObjects.Fields;
using Diassoft.DataAccess.FilterParsers;
using Diassoft.DataAccess.Operations;

namespace Diassoft.DataAccess.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="WhereCollection"/> object
    /// </summary>
    public static class DiassoftDataAccessWhereCollectionExtensions
    {
        /// <summary>
        /// Append an array of <see cref="FilterField"/> to a Where Collection
        /// </summary>
        /// <param name="whereCollection">Reference to the WhereCollection</param>
        /// <param name="filterFields">An array of <see cref="FilterField"/> containing the query string filters</param>
        /// <param name="wrapIntoParenthesis">A flag to define whether to wrap the filters into a parenthesis</param>
        public static void AppendQueryFilters(this WhereCollection whereCollection, FilterField[] filterFields, bool wrapIntoParenthesis)
        {
            // Make sure filters are not null
            if (filterFields == null) return;
            if (filterFields.Length == 0) return;

            // Makre sure where collection is initialized
            if (whereCollection == null) throw new NullReferenceException("The 'WhereCollection' is empty. System is unable to Append Query Filters to a null collection");

            // The Grouped Where Collection
            var groupedWhereCollection = new GroupedFilterExpression(FieldAndOr.And);

            // Loop thru all filters on the QueryFilters
            foreach (var filter in filterFields)
            {
                // Creates the Expression
                var filterExpression = new FilterExpression()
                {
                    Field1 = new Field(filter.Field),
                    Field2 = filter.Value,
                    AndOr = FieldAndOr.And,
                };

                // Chooses the Operator
                switch (filter.Operator)
                {
                    case FilterFieldOperations.NotEqual: filterExpression.Operator = FieldOperators.NotEqual; break;
                    case FilterFieldOperations.In: filterExpression.Operator = FieldOperators.In; break;
                    case FilterFieldOperations.NotIn: filterExpression.Operator = FieldOperators.NotIn; break;
                    case FilterFieldOperations.LessThan: filterExpression.Operator = FieldOperators.LessThan; break;
                    case FilterFieldOperations.LessThanOrEqualTo: filterExpression.Operator = FieldOperators.LessThanOrEqual; break;
                    case FilterFieldOperations.GreaterThan: filterExpression.Operator = FieldOperators.GreaterThan; break;
                    case FilterFieldOperations.GreaterThanOrEqualTo: filterExpression.Operator = FieldOperators.GreaterThanOrEqual; break;
                    default:
                        filterExpression.Operator = FieldOperators.Equal;
                        break;
                }

                // Adds the Other Side of the Filter
                //if (filter.Value.GetType().IsArray)
                //{
                //    filterExpression.Field2 = filter.Value;
                //}
                //else
                //{
                //    filterExpression.Field2 = filter.Value;
                //}

                // Includes it on the collection
                if (wrapIntoParenthesis)
                    groupedWhereCollection.Add(filterExpression);
                else
                    whereCollection.Add(filterExpression);
            }

            // When wrapping, adds the GroupedWhereCollection later
            if (wrapIntoParenthesis)
                whereCollection.Add(groupedWhereCollection);
        }

        /// <summary>
        /// Append a <see cref="List{T}"/> of <see cref="FilterField"/> to a Where Collection
        /// </summary>
        /// <param name="whereCollection">Reference to the WhereCollection</param>
        /// <param name="queryFilters">The <see cref="List{T}"/> of <see cref="FilterField"/> containing the Query String filters</param>
        /// <param name="wrapIntoParenthesis">A flag to define whether to wrap the filters into a parenthesis</param>
        public static void AppendQueryFilters(this WhereCollection whereCollection, List<FilterField> queryFilters, bool wrapIntoParenthesis)
        {
            if (queryFilters == null) return;
            if (queryFilters.Count == 0) return;

            AppendQueryFilters(whereCollection, queryFilters.ToArray(), wrapIntoParenthesis);
        }
    }
}
