using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace Diassoft.DataAccess.Extensions
{
    /// <summary>
    /// Represent extension methods for the <see cref="IDataReader"/> interface
    /// </summary>
    public static class IDataReaderExtensions
    {
        /// <summary>
        /// Retrieves the value of a field or its default value
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="dataReader">Reference to the Data Reader</param>
        /// <param name="fieldName">Field to Retrieve</param>
        /// <returns>The contents of the field or the default value</returns>
        public static T GetValueOrDefault<T>(this IDataReader dataReader, string fieldName)
        {
            // Retrieve Field Position (if not found it will throw a IndexOutOfRangeException)
            var fieldOrdinal = dataReader.GetOrdinal(fieldName);

            // If the value is null, return the default value for the field type
            if (dataReader.IsDBNull(fieldOrdinal)) return default(T);
            
            // Get the value itsels
            var fieldValue = dataReader.GetValue(fieldOrdinal);

            if (fieldValue is T res)
                return res;

            var converterOfFieldValue = TypeDescriptor.GetConverter(fieldValue.GetType());

            if (converterOfFieldValue == null)
                throw new Exception($"Unable to find a converter for type '{fieldValue.GetType()}'");

            if (converterOfFieldValue.CanConvertTo(typeof(T)))
                return (T)(converterOfFieldValue.ConvertTo(fieldValue, typeof(T)));
            else
                throw new InvalidCastException();
        }

        /// <summary>
        /// Retrieves the value of a field or its default value
        /// </summary>
        /// <param name="dataReader">Reference to the Data Reader</param>
        /// <param name="fieldName">Field to Retrieve</param>
        /// <param name="fieldType">Type of the Field to Retrieve</param>
        /// <returns>The contents of the field or the default value</returns>
        public static object GetValueOrDefault(this IDataReader dataReader, string fieldName, Type fieldType)
        {
            // Retrieve Field Position (if not found it will throw a IndexOutOfRangeException)
            var fieldOrdinal = dataReader.GetOrdinal(fieldName);

            // If the value is null, return the default value for the field type
            if (dataReader.IsDBNull(fieldOrdinal))
            {
                // Returns the Default Value for the Field
                if (fieldType.GetType().IsValueType)
                    return Activator.CreateInstance(fieldType);
                else
                    return null;
            }

            // Get the value itsels
            var fieldValue = dataReader.GetValue(fieldOrdinal);

            if (fieldValue.GetType() == fieldType)
                return fieldValue;

            var converterOfFieldValue = TypeDescriptor.GetConverter(fieldValue.GetType());

            if (converterOfFieldValue == null)
                throw new Exception($"Unable to find a converter for type '{fieldValue.GetType()}'");

            if (converterOfFieldValue.CanConvertTo(fieldType))
                return (converterOfFieldValue.ConvertTo(fieldValue, fieldType));
            else
                throw new InvalidCastException();
        }
    }
}
