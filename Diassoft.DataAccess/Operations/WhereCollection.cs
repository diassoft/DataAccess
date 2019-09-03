using Diassoft.DataAccess.DatabaseObjects.Expressions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Diassoft.DataAccess.Operations
{
    /// <summary>
    /// Represents the Where Clause Collection of <see cref="IFilterExpression"/>
    /// </summary>
    public class WhereCollection: Collection<IFilterExpression>
    {
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
