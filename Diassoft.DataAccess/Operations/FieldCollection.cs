using Diassoft.DataAccess.DatabaseObjects.Fields;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Diassoft.DataAccess.Operations
{
    /// <summary>
    /// Represents a Collection of <see cref="Field">Fields</see>
    /// </summary>
    public class FieldCollection: Collection<Field>
    {

        /// <summary>
        /// Inserts a new item into the collection
        /// </summary>
        /// <param name="index"></param>
        /// <param name="newItem"></param>
        protected override void InsertItem(int index, Field newItem)
        {
            base.InsertItem(index, newItem);
        }

        /// <summary>
        /// Sets the item on the collection
        /// </summary>
        /// <param name="index"></param>
        /// <param name="newItem"></param>
        protected override void SetItem(int index, Field newItem)
        {
            Field replaced = Items[index];
            base.SetItem(index, newItem);
        }

        /// <summary>
        /// Removes an item from the collection
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            Field removedItem = Items[index];
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
