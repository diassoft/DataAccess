using System;
using System.Collections.Generic;
using System.Text;
using Diassoft.DataAccess.DatabaseObjects.Fields;

namespace Diassoft.DataAccess.DatabaseObjects.Expressions
{
    /// <summary>
    /// Represents a Collection of <see cref="IFilterExpression"/>
    /// </summary>
    public class GroupedFilterExpression : System.Collections.ObjectModel.Collection<IFilterExpression>, IFilterExpression
    {
        /// <summary>
        /// The connection between this and the next expression
        /// </summary>
        public FieldAndOr AndOr { get; set; }
        /// <summary>
        /// Defines whether the expression should be enclosed in parenthesis or not
        /// </summary>
        public bool Enclose { get; } = true;
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedFilterExpression"/>
        /// </summary>
        public GroupedFilterExpression(): this(FieldAndOr.None) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedFilterExpression"/>
        /// </summary>
        /// <param name="andOr">The connection between this and the next expression</param>
        public GroupedFilterExpression(FieldAndOr andOr): base()
        {
            AndOr = andOr;
        }

        /// <summary>
        /// Inserts a new item into the collection
        /// </summary>
        /// <param name="index"></param>
        /// <param name="newItem"></param>
        protected override void InsertItem(int index, IFilterExpression newItem)
        {
            base.InsertItem(index, newItem);
        }

        /// <summary>
        /// Sets the item on the collection
        /// </summary>
        /// <param name="index"></param>
        /// <param name="newItem"></param>
        protected override void SetItem(int index, IFilterExpression newItem)
        {
            IFilterExpression replaced = Items[index];
            base.SetItem(index, newItem);
        }

        /// <summary>
        /// Removes an item from the collection
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            IFilterExpression removedItem = Items[index];
            base.RemoveItem(index);
        }

        /// <summary>
        /// Clear all items from the collection
        /// </summary>
        protected override void ClearItems()
        {
            base.ClearItems();
        }

    }
}
