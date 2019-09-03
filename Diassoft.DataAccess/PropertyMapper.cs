using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Diassoft.DataAccess
{
    /// <summary>
    /// An object that maps properties from an object to another
    /// </summary>
    public static class PropertyMapper
    {
        /// <summary>
        /// A dictionary containing the type specs
        /// </summary>
        /// <remarks>It uses a <see cref="ConcurrentDictionary{TKey, TValue}"/> because multiple instances can be sharing the same data, especially when running services.</remarks>
        private static ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> TypeSpecs = new ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>>();

        /// <summary>
        /// Retrieves the Type Specification
        /// </summary>
        /// <remarks>This function caches the information in a <see cref="ConcurrentDictionary{TKey, TValue}"/> to speed performance</remarks>
        /// <param name="type">Type to retrieve specifications</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> containing the properties by name</returns>
        private static Dictionary<string, PropertyInfo> GetTypeSpec(Type type)
        {
            // Check on existing repository
            if (TypeSpecs.TryGetValue(type, out Dictionary<string, PropertyInfo> currentSpecs))
                return currentSpecs;

            // Read type specification and populates the collection
            var newTypeSpec = new Dictionary<string, PropertyInfo>();

            var properties = type.GetProperties();
            foreach (var property in properties)
                newTypeSpec.Add(property.Name, property);

            // Try to add it to the collection
            TypeSpecs.TryAdd(type, newTypeSpec);

            // Returns the new type specification
            return newTypeSpec;
        }

        /// <summary>
        /// Maps the contents of one object to another based on the property names
        /// </summary>
        /// <typeparam name="TDestinationType">The destination object type (it needs to have a parameterless contructor)</typeparam>
        /// <param name="source">The source object</param>
        /// <returns></returns>
        public static TDestinationType Map<TDestinationType>(object source)
        {
            // Initializes Destination Object
            var destinationObject = Activator.CreateInstance<TDestinationType>();

            Map(source, destinationObject);

            // Returns the newly created object
            return destinationObject;
        }

        /// <summary>
        /// Maps the contents of one object to another based on the property names
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        public static void Map(object source, object destination)
        {
            // Retrieve Source and Destination Types Spec
            var sourceTypeSpec = GetTypeSpec(source.GetType());
            var destinationTypeSpec = GetTypeSpec(destination.GetType());

            // Apply each property to the destination
            foreach (var destinationProperty in destinationTypeSpec.Values)
            {
                // Make sure property can be written
                if (destinationProperty.CanWrite)
                {
                    // Look for a corresponding property on the spec of the source object
                    if (sourceTypeSpec.TryGetValue(destinationProperty.Name, out PropertyInfo sourceProperty))
                    {
                        // Set the value
                        destinationProperty.SetValue(destination, sourceProperty.GetValue(source));
                    }
                }
            }
        }
    }
}
