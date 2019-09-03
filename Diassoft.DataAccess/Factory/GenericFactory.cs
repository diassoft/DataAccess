using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using Diassoft.DataAccess;
using Diassoft.DataAccess.Operations;
using Diassoft.DataAccess.Exceptions;
using Diassoft.DataAccess.DatabaseObjects.Expressions;
using Diassoft.DataAccess.DatabaseObjects.Fields;
using System.Reflection;
using System.ComponentModel;
using System.Threading.Tasks;
using Diassoft.DataAccess.Models.DTO;
using Diassoft.DataAccess.FilterParsers;
using Diassoft.DataAccess.Attributes;
using Diassoft.DataAccess.Extensions;

namespace Diassoft.DataAccess.Factory
{
    /// <summary>
    /// Represents a Generic Factory to Maintain Objects
    /// </summary>
    /// <typeparam name="TModel">The type of the Model</typeparam>
    /// <typeparam name="TCreateDTO">The type of the DTO when creating a new record</typeparam>
    /// <typeparam name="TUpdateDTO">The type of the DTO when updating an existing record</typeparam>
    /// <typeparam name="TDeleteDTO">The type of the DTO when deleting an existing record (most likely will only contain the key fields)</typeparam>
    public class GenericFactory<TModel, TCreateDTO, TUpdateDTO, TDeleteDTO>
        where TCreateDTO : IDTOModel<TModel>
        where TUpdateDTO : IDTOModel<TModel>
        where TDeleteDTO : IDTOModel<TModel>
    {
        /// <summary>
        /// The Database Context
        /// </summary>
        protected IDbContext DbContext { get; }

        /// <summary>
        /// Defines whether the Model is Valid
        /// </summary>
        /// <remarks>The model has to be decorated with attributes to represent the table name and field names</remarks>
        public bool IsModelValid { get; protected set; }

        /// <summary>
        /// Represents the Database Table Attribute assigned to the Model
        /// </summary>
        protected DatabaseTableAttribute DatabaseTableAttribute { get; set; }

        /// <summary>
        /// Array of Primary Keys (PropertyInfo and DatabaseFieldAttribute)
        /// </summary>
        public FieldInfo[] PrimaryKeys { get; protected set; }

        /// <summary>
        /// Array of Primary Keys (PropertyInfo and DatabaseFieldAttribute) for the <typeparamref name="TUpdateDTO"/>
        /// </summary>
        public FieldInfo[] PrimaryKeysForUpdateDTO { get; protected set; }

        /// <summary>
        /// Array of Primary Keys (PropertyInfo and DatabaseFieldAttribute) for the <typeparamref name="TDeleteDTO"/>
        /// </summary>
        public FieldInfo[] PrimaryKeysForDeleteDTO { get; protected set; }

        /// <summary>
        /// Array of Fields (PropertyInfo and DatabaseFieldAttribute)
        /// </summary>
        public FieldInfo[] Fields { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericFactory{TModel, TCreateDTO, TUpdateDTO, TDeleteDTO}"/>
        /// </summary>
        /// <param name="dbContext">Reference to the Database Context</param>
        public GenericFactory(IDbContext dbContext)
        {
            DbContext = dbContext;
            IsModelValid = false;

            ValidateModel();
        }

        /// <summary>
        /// Validates the Model
        /// </summary>
        /// <remarks>A model must have at least one property and one primary key.
        /// Use <see cref="DatabaseTableAttribute"/> and <see cref="DatabaseFieldAttribute"/> to configure the model to match the database.</remarks>
        protected virtual void ValidateModel()
        {
            // A model to be used in a Factory must contain a Primary Key.
            // Table Name will come from the DatabaseTableAttribute, however, if this is not provided, the Class Name will be used instead.
            // Field Name will come from the DatabaseFieldAttribute, however, if this is not provided, the property will be ignored.

            // Reset Model Valid Variable
            IsModelValid = false;

            // Internal Objects
            var primaryKeysList = new List<FieldInfo>();
            var primaryKeysUpdateDTOList = new List<FieldInfo>();
            var primaryKeysDeleteDTOList = new List<FieldInfo>();
            var fieldsList = new List<FieldInfo>();

            // Retrieve Database Table Attribute
            this.DatabaseTableAttribute = Attribute.GetCustomAttribute(typeof(TModel), typeof(DatabaseTableAttribute)) as DatabaseTableAttribute;
            if (this.DatabaseTableAttribute == null)
            {
                // Initializes the attribute
                this.DatabaseTableAttribute = new DatabaseTableAttribute(typeof(TModel).Name);
            }
            else
            {
                if (this.DatabaseTableAttribute.Name == null)
                    this.DatabaseTableAttribute.Name = typeof(TModel).Name;
            }

            // Retrieve Properties of the Class
            var properties = typeof(TModel).GetProperties();
            if (properties == null)
                throw new Exception($"No properties were found on the type '{typeof(TModel).Name}'. Unable to use it in a FactoryBase.");

            if (properties?.Length == 0)
                throw new Exception($"No properties were found on the type '{typeof(TModel).Name}'. Unable to use it in a FactoryBase.");

            foreach (var property in properties)
            {
                var propertyAttributes = property.GetCustomAttributes(typeof(DatabaseFieldAttribute), true);
                if (propertyAttributes?.Length > 0)
                {
                    foreach (var propertyAttribute in from pa in propertyAttributes
                                                      where pa is DatabaseFieldAttribute
                                                      select pa as DatabaseFieldAttribute)
                    {
                        // Verify Field Name
                        if (propertyAttribute.Name == null)
                            propertyAttribute.Name = property.Name;

                        // Verify Primary Keys
                        if (propertyAttribute.IsPrimaryKey)
                            primaryKeysList.Add(new FieldInfo(property, propertyAttribute));

                        // Add to the collection
                        fieldsList.Add(new FieldInfo(property, propertyAttribute));
                    }
                }
            }

            // Ensure there are primary keys
            if (primaryKeysList.Count == 0)
                throw new Exception($"No primary keys could be found for type '{typeof(TModel).Name}'. Unable to use the object in a FactoryBase.");

            // Convert Lists to Arrays
            PrimaryKeys = primaryKeysList.ToArray();
            Fields = fieldsList.ToArray();

            // Retrieve DTO Primary Keys
            var updateDTOType = typeof(TUpdateDTO);
            var deleteDTOType = typeof(TDeleteDTO);
            foreach (var primaryKey in PrimaryKeys)
            {
                // Check if propery name exists on Update DTO
                var updateProperty = updateDTOType.GetProperty(primaryKey.Property.Name);
                if (updateProperty != null) primaryKeysUpdateDTOList.Add(new FieldInfo(updateProperty, primaryKey.Attribute));

                // Check if propery name exists on Update DTO
                var deleteProperty = deleteDTOType.GetProperty(primaryKey.Property.Name);
                if (deleteProperty != null) primaryKeysDeleteDTOList.Add(new FieldInfo(deleteProperty, primaryKey.Attribute));
            }

            PrimaryKeysForUpdateDTO = primaryKeysUpdateDTOList.ToArray();
            PrimaryKeysForDeleteDTO = primaryKeysDeleteDTOList.ToArray();

            // Everything is valid, set model to valid
            IsModelValid = true;
        }

        /// <summary>
        /// Apply any additional formatting before <see cref="Create(TCreateDTO, IDbConnection, IDbTransaction)"/> or <see cref="Update(TUpdateDTO, IDbConnection, IDbTransaction)"/> methods are called.
        /// </summary>
        /// <param name="model">Reference to the model</param>
        protected virtual void FormatModel(TModel model)
        {

        }

        #region Methods Before/After I/O Operations

        /// <summary>
        /// Method called before the object creation is performed
        /// </summary>
        /// <param name="newModel">Reference to the new model, as it will be on the database</param>
        /// <param name="entity">Reference to the Data Transfer Object</param>
        /// <param name="dbConnection">Reference to an existing database connection</param>
        /// <param name="dbTransaction">Reference to an existing database transaction</param>
        /// <returns>A <see cref="bool"/> to define whether the object creation should proceed</returns>
        protected virtual bool OnBeforeCreate(TModel newModel, TCreateDTO entity, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            return true;
        }

        /// <summary>
        /// Method called after the object creation is performed
        /// </summary>
        /// <param name="newModel">Reference to the new model, as it is on the database</param>
        /// <param name="entity">Reference to the Data Transfer Object</param>
        /// <param name="dbConnection">Reference to an existing database connection</param>
        /// <param name="dbTransaction">Reference to an existing database transaction</param>
        protected virtual void OnAfterCreate(TModel newModel, TCreateDTO entity, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {

        }

        /// <summary>
        /// Method called before the Update is performed
        /// </summary>
        /// <remarks>Use this method to provide additional information before the data is updated</remarks>
        /// <param name="originalModel">Reference to the current model as it is on the database</param>
        /// <param name="newModel">Reference to the model to be updated</param>
        /// <param name="modelDTO">Referente to the Data Transfer object</param>
        /// <param name="dbConnection">Reference to an existing database connection</param>
        /// <param name="dbTransaction">Reference to an existing database transaction</param>
        /// <returns>A <see cref="bool"/> to define whether the record should be updated or not</returns>
        protected virtual bool OnBeforeUpdate(TModel originalModel, TModel newModel, TUpdateDTO modelDTO, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            return true;
        }

        /// <summary>
        /// Method called after the Update is performed
        /// </summary>
        /// <param name="model">Reference to the model to be updated</param>
        /// <param name="modelDTO">Referente to the Data Transfer object</param>
        /// <param name="dbConnection">Reference to an existing database connection</param>
        /// <param name="dbTransaction">Reference to an existing database transaction</param>
        protected virtual void OnAfterUpdate(TModel model, TUpdateDTO modelDTO, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {

        }

        /// <summary>
        /// Method called before the object is deleted
        /// </summary>
        /// <param name="deleteModel">Model to be deleted</param>
        /// <param name="modelDTO">Data Transfer object for the object to be deleted</param>
        /// <param name="dbConnection">Reference to an existing database connection</param>
        /// <param name="dbTransaction">Reference to an existing database transaction</param>
        /// <returns></returns>
        protected virtual bool OnBeforeDelete(TModel deleteModel, TDeleteDTO modelDTO, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            return true;
        }

        /// <summary>
        /// Method called after the object deleted
        /// </summary>
        /// <param name="deleteModel">Model that has been deleted</param>
        /// <param name="modelDTO">Data Transfer object for the object deleteed</param>
        /// <param name="dbConnection">Reference to an existing database connection</param>
        /// <param name="dbTransaction">Reference to an existing database transaction</param>
        protected virtual void OnAfterDelete(TModel deleteModel, TDeleteDTO modelDTO, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {

        }

        /// <summary>
        /// Method called before loading a record into the resultset
        /// </summary>
        /// <param name="model">Model to be inserted</param>
        /// <param name="dbConnection">Reference to an existing database connection</param>
        /// <param name="dbTransaction">Reference to an existing database transaction</param>
        /// <returns>A <see cref="bool"/> to define whether the record should or not be loaded into the resultset</returns>
        protected virtual bool OnBeforeRecordLoad(TModel model, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            return true;
        }

        /// <summary>
        /// Method called after loading the record into the resultset
        /// </summary>
        /// <param name="model">Model that has just been inserted into the return collection</param>
        /// <param name="dbConnection">Reference to an existing database connection</param>
        /// <param name="dbTransaction">Reference to an existing database transaction</param>
        protected virtual void OnAfterRecordLoad(TModel model, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {

        }

        #endregion Methods Before/After I/O Operations

        #region CRUD Operations

        /// <summary>
        /// Creates a new instance of the <typeparamref name="TModel"/> into the database
        /// </summary>
        /// <param name="entity">The data to be created</param>
        /// <param name="dbConnection">Reference to an existing database connection</param>
        /// <param name="dbTransaction">Reference to an existing database transaction</param>
        /// <returns>An instance of the recently created object</returns>
        /// <exception cref="PrimaryKeyViolationException">Thrown if the record already exists</exception>
        public virtual TModel Create(TCreateDTO entity, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            // Ensure Model is Valid
            if (!IsModelValid)
                throw new Exception("Model is invalid");

            // Initializes a transaction, if applicable
            IDbConnection connection = dbConnection;
            IDbTransaction transaction = dbTransaction;

            try
            {
                // Creates a connection if no connection has been passed
                if (connection == null)
                    connection = DbContext.GetConnection();

                // Initializes a transaction if no transaction has been passed
                if (transaction == null)
                    transaction = connection.BeginTransaction();

                // Map Dto to Model
                var model = entity.MapToModel();

                // Calls the "OnBeforeCreate" method
                if (!OnBeforeCreate(model, entity, connection, transaction))
                    throw new Exception("Create operation cancelled");

                // Verify Primary Key Violation
                var models = Get(GetPrimaryKeyFilters(model), connection, transaction);
                if (models.Count > 0)
                    throw new PrimaryKeyViolationException($"Duplicated Key for '{typeof(TModel).Name}'");

                // Calls Model Format function
                FormatModel(model);

                // Define AssignExpressions 
                var assignExpressions = new List<AssignExpression>();
                foreach (var fieldInfo in Fields)
                {
                    // Define the Assign Expression Object
                    AssignExpression assignExpression;

                    // For String type, shrink character count (when size is specified)
                    if ((fieldInfo.Attribute.Type == typeof(string)) && (fieldInfo.Attribute.Size > 0))
                    {
                        // Shrink Field Contents, if applicable
                        var fieldContents = fieldInfo.Property.GetValue(model)?.ToString().PadRight(fieldInfo.Attribute.Size, ' ').Substring(0, fieldInfo.Attribute.Size).Trim();

                        // Apply New Contents to the Model
                        if (fieldContents == null) fieldContents = ""; //TODO: fix framework to accept nulls

                        fieldInfo.Property.SetValue(model, fieldContents);

                        // Apply to the AssignExpression
                        assignExpression = new AssignExpression(new Field(fieldInfo.Attribute.Name), fieldContents);
                    }
                    else
                    {
                        // Check for Enum types
                        if (fieldInfo.Property.PropertyType.IsEnum)
                        {
                            // Enumerations have a special treatment as its value need to be parsed from the string that represents it
                            var enumValue = fieldInfo.Property.GetValue(model);
                            object finalValue = null;

                            // If value is null, force it to zero
                            if (enumValue == null)
                                finalValue = 0;
                            else
                                finalValue = Convert.ChangeType(enumValue, fieldInfo.Property.PropertyType.GetEnumUnderlyingType());

                            assignExpression = new AssignExpression(new Field(fieldInfo.Attribute.Name), finalValue);
                        }
                        else
                        {
                            assignExpression = new AssignExpression(new Field(fieldInfo.Attribute.Name), fieldInfo.Property.GetValue(model));
                        }
                    }

                    assignExpressions.Add(assignExpression);
                }

                // Create the Insert Operation with the previously setup Assign Expressions
                var insert = new InsertDbOperation(DatabaseTableAttribute.Name)
                {
                    Assignments = assignExpressions.ToArray()
                };

                // Runs the Insert Statement
                DbContext.ExecuteNonQuery(insert, connection, transaction);

                // Calls the "OnAfterCreate" method
                OnAfterCreate(model, entity, connection, transaction);

                if (dbTransaction == null)
                    transaction.Commit();

                // Returns the Entity Created
                return model;
            }
            catch
            {
                // Check if this action is joining a transaction
                if (dbTransaction == null)
                    transaction.Rollback();

                throw;
            }
            finally
            {
                // Closes the connection created on this method
                if (dbConnection == null)
                    if (connection?.State == ConnectionState.Open)
                        connection.Close();
            }
        }

        /// <summary>
        /// Creates a new instance of the TModel into the database
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>A <see cref="Task"/> of <typeparamref name="TModel"/> containing the creation results</returns>
        public async Task<TModel> CreateAsync(TCreateDTO entity)
        {
            var task = new Task<TModel>(() => Create(entity, null, null));
            task.Start();

            return await task.ConfigureAwait(false);
        }

        /// <summary>
        /// Bulk create records on the database
        /// </summary>
        /// <param name="entities">A <see cref="List{T}"/> of <typeparamref name="TCreateDTO"/> containing the objects to be created</param>
        /// <param name="dbConnection">Reference to an existing database connection</param>
        /// <param name="dbTransaction">Reference to an existing database transaction</param>
        /// <returns>A <see cref="List{T}"/> containing all the models created</returns>
        public virtual List<TModel> Create(List<TCreateDTO> entities, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            // Initializes a transaction, if applicable
            IDbConnection connection = dbConnection;
            IDbTransaction transaction = dbTransaction;

            try
            {
                // Creates a connection if no connection has been passed
                if (connection == null)
                    connection = DbContext.GetConnection();

                // Initializes a transaction if no transaction has been passed
                if (transaction == null)
                    transaction = connection.BeginTransaction();

                // Loop thru elements and call the Create method
                var returnList = new List<TModel>();

                foreach (var entity in entities)
                    returnList.Add(Create(entity, connection, transaction));

                if (dbTransaction == null)
                    transaction.Commit();

                // Returns the Entity Created
                return returnList;
            }
            catch
            {
                // Check if this action is joining a transaction
                if (dbTransaction == null)
                    transaction.Rollback();

                throw;
            }
            finally
            {
                // Closes the connection created on this method
                if (dbConnection == null)
                    if (connection?.State == ConnectionState.Open)
                        connection.Close();
            }
        }

        /// <summary>
        /// Bulk create records on the database asynchronously
        /// </summary>
        /// <param name="entities">A <see cref="List{T}"/> of <typeparamref name="TCreateDTO"/> containing the objects to be created</param>
        /// <returns>A <see cref="List{T}"/> containing all the models created</returns>
        public async Task<List<TModel>> CreateAsync(List<TCreateDTO> entities)
        {
            var task = new Task<List<TModel>>(() => Create(entities, null, null));
            task.Start();

            return await task.ConfigureAwait(false);
        }

        /// <summary>
        /// Returns instances of the <typeparamref name="TModel"/> based on pre-defined filters
        /// </summary>
        /// <param name="filters">A <see cref="List{T}"/> of <see cref="FilterField"/> containing the filters</param>
        /// <param name="dbConnection">Reference to an existing database connection</param>
        /// <param name="dbTransaction">Reference to an existing database transaction</param>
        /// <returns>A <see cref="List{T}"/> of <typeparamref name="TModel"/> with the objects returned from the database</returns>
        public virtual List<TModel> Get(List<FilterField> filters, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            // Ensure Model is Valid
            if (!IsModelValid)
                throw new Exception("Model is invalid");

            // List of TModel instance
            var models = new List<TModel>();

            // Retrieve List of TModel
            var select = new SelectDbOperation(DatabaseTableAttribute.Name)
            {
                Where = new WhereCollection()
            };
            select.Where.AppendQueryFilters(filters, false);

            using (var reader = DbContext.ExecuteReader(select))
            {
                while (reader.Read())
                {
                    // Initializes the instance of the TModel
                    var model = Activator.CreateInstance(typeof(TModel));

                    // Load Data
                    foreach (var fieldInfo in Fields)
                    {
                        fieldInfo.Property.SetValue(model, reader.GetValueOrDefault(fieldInfo.Attribute.Name, fieldInfo.Attribute.Type));
                    }

                    // Make sure record can be added
                    if (OnBeforeRecordLoad((TModel)model, dbConnection, dbTransaction))
                    {
                        // Add to the resultset
                        models.Add((TModel)model);

                        // Calls the method to process a record that has already been inserted
                        OnAfterRecordLoad((TModel)model, dbConnection, dbTransaction);
                    }
                }
            }

            return models;
        }

        /// <summary>
        /// Return instances of the <typeparamref name="TModel"/> 
        /// </summary>
        /// <param name="filters">A <see cref="List{T}"/> of <see cref="FilterField"/> containing the filters</param>
        /// <returns>A <see cref="List{T}"/> of <typeparamref name="TModel"/> with the objects returned from the database</returns>
        public async Task<List<TModel>> GetAsync(List<FilterField> filters)
        {
            var task = new Task<List<TModel>>(() => Get(filters, null, null));
            task.Start();

            return await task.ConfigureAwait(false);
        }

        /// <summary>
        /// Returns instances of the <typeparamref name="TModel"/> converted to the <typeparamref name="TResultModel"/>
        /// </summary>
        /// <remarks>If the <typeparamref name="TResultModel"/> implements the <see cref="IDTOModel{TModel}"/> interface, the conversion will use the method <see cref="IDTOModel{TModel}.MapFromModel(TModel)"/> to perform the conversion.</remarks>
        /// <typeparam name="TResultModel">Type of the object to return. Prefer to use objects implementing the <see cref="IDTOModel{TModel}"/> interface.</typeparam>
        /// <param name="filters">A <see cref="List{T}"/> of <see cref="FilterField"/> containing the filters</param>
        /// <param name="dbConnection">Reference to an existing database connection</param>
        /// <param name="dbTransaction">Reference to an existing database transaction</param>
        /// <returns>A <see cref="List{T}"/> of <typeparamref name="TResultModel"/></returns>
        public virtual List<TResultModel> Get<TResultModel>(List<FilterField> filters, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            // Retrieve the data on the original model type
            var models = Get(filters, dbConnection, dbTransaction);

            // Container for converted data
            var convertedModels = new List<TResultModel>();

            // Converts each record
            foreach (var model in models)
            {
                var convertedModel = Activator.CreateInstance<TResultModel>();

                if (typeof(TResultModel) is IDTOModel<TModel>)
                {
                    // There is a direct conversion
                    ((IDTOModel<TModel>)convertedModel).MapFromModel(model);
                }
                else
                {
                    // There is no conversion, try using the property mapper
                    PropertyMapper.Map(model, convertedModel);
                }

                convertedModels.Add(convertedModel);
            }

            return convertedModels;
        }

        /// <summary>
        /// Return instances of the <typeparamref name="TModel"/> converted to <typeparamref name="TResultModel"/>
        /// </summary>
        /// <typeparam name="TResultModel">Type of the object to return. Prefer to use objects implementing the <see cref="IDTOModel{TModel}"/> interface.</typeparam>
        /// <param name="filters">A <see cref="List{T}"/> of <see cref="FilterField"/> containing the filters</param>
        /// <returns>A <see cref="List{T}"/> of <typeparamref name="TResultModel"/></returns>
        public async Task<List<TResultModel>> GetAsync<TResultModel>(List<FilterField> filters)
        {
            var task = new Task<List<TResultModel>>(() => Get<TResultModel>(filters, null, null));
            task.Start();

            return await task.ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the Record to the Database
        /// </summary>
        /// <remarks>This action updates the entire records and all fields on it, not partial updates</remarks>
        /// <param name="entity">The entity to be updated</param>
        /// <param name="dbConnection">Reference to an existing database connection</param>
        /// <param name="dbTransaction">Reference to an existing database transaction</param>
        public virtual void Update(TUpdateDTO entity, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            // Ensure Model is Valid
            if (!IsModelValid)
                throw new Exception("Model is invalid");

            // Map Dto to Model
            var model = entity.MapToModel();

            // Initializes a transaction, if applicable
            IDbConnection connection = dbConnection;
            IDbTransaction transaction = dbTransaction;

            try
            {
                // Creates a connection if no connection has been passed
                if (connection == null)
                    connection = DbContext.GetConnection();

                // Initializes a transaction if no transaction has been passed
                if (transaction == null)
                    transaction = connection.BeginTransaction();

                // Verify if the record exists
                var primarykeys = GetPrimaryKeyFilters(model);
                var models = Get(primarykeys, connection, transaction);
                if (models.Count == 0)
                    throw new RecordNotFoundException($"Unable to find '{model.GetType().Name}' to update based on the given primary keys", primarykeys);

                // Calls Model Format function
                FormatModel(model);

                // Calls the "OnBeforeUpdate" method
                if (!OnBeforeUpdate(models[0], model, entity, connection, transaction))
                    throw new Exception("Update operation cancelled");

                // Define AssignExpressions 
                var assignExpressions = new List<AssignExpression>();
                foreach (var fieldInfo in Fields)
                {
                    // Define the Assign Expression Object
                    AssignExpression assignExpression;

                    // For String type, shrink character count (when size is specified)
                    if ((fieldInfo.Attribute.Type == typeof(string)) && (fieldInfo.Attribute.Size > 0))
                    {
                        if (fieldInfo.Property.GetValue(model) != null)
                        {
                            // Shrink Field Contents, if applicable
                            var fieldContents = fieldInfo.Property.GetValue(model)?.ToString().PadRight(fieldInfo.Attribute.Size, ' ').Substring(0, fieldInfo.Attribute.Size).Trim();

                            // Apply New Contents to the Model
                            if (fieldContents == null) fieldContents = ""; //TODO: fix it on the framework
                            fieldInfo.Property.SetValue(model, fieldContents);

                            // Apply to the AssignExpression
                            assignExpression = new AssignExpression(new Field(fieldInfo.Attribute.Name), fieldContents);
                        }
                        else
                        {
                            //TODO: accept null. Need to fix the Diassoft DataAccess
                            // Apply New Contents to the Model
                            fieldInfo.Property.SetValue(model, "");

                            // Apply to the AssignExpression
                            assignExpression = new AssignExpression(new Field(fieldInfo.Attribute.Name), "");
                        }
                    }
                    else
                    {
                        // Check for Enum types
                        if (fieldInfo.Property.PropertyType.IsEnum)
                        {
                            // Enumerations have a special treatment as its value need to be parsed from the string that represents it
                            var enumValue = fieldInfo.Property.GetValue(model);
                            object finalValue = null;

                            // If value is null, force it to zero
                            if (enumValue == null)
                                finalValue = 0;
                            else
                                finalValue = Convert.ChangeType(enumValue, fieldInfo.Property.PropertyType.GetEnumUnderlyingType());

                            assignExpression = new AssignExpression(new Field(fieldInfo.Attribute.Name), finalValue);
                        }
                        else
                        {
                            assignExpression = new AssignExpression(new Field(fieldInfo.Attribute.Name), fieldInfo.Property.GetValue(model));
                        }
                    }

                    assignExpressions.Add(assignExpression);
                }

                var update = new UpdateDbOperation(DatabaseTableAttribute.Name)
                {
                    Assignments = assignExpressions.ToArray()
                };
                update.Where.AppendQueryFilters(GetPrimaryKeyFilters(model), false);

                DbContext.ExecuteNonQuery(update, connection, transaction);

                // Calls the OnAfterUpdate method
                OnAfterUpdate(model, entity, connection, transaction);

                // Commits the transaction
                if (dbTransaction == null)
                    transaction.Commit();
            }
            catch
            {
                // Check if this action is joining a transaction
                if (dbTransaction == null)
                    transaction.Rollback();

                throw;
            }
            finally
            {
                // Closes the connection created on this method
                if (dbConnection == null)
                    if (connection?.State == ConnectionState.Open)
                        connection.Close();
            }
            
        }

        /// <summary>
        /// Updates the record in the database
        /// </summary>
        /// <remarks>This action updates the entire records and all fields on it, not partial updates</remarks>
        /// <param name="entity">The entity to be updated</param>
        /// <returns>The task created to run the process asynchronously</returns>
        public async Task UpdateAsync(TUpdateDTO entity)
        {
            var task = new Task(() => Update(entity, null, null));
            task.Start();

            await task.ConfigureAwait(false);
        }

        /// <summary>
        /// Delete objects from the database based on the primary key of <typeparamref name="TModel"/>
        /// </summary>
        /// <param name="entity">The entity containing the data to be deleted</param>
        /// <param name="dbConnection">Reference to an existing database connection</param>
        /// <param name="dbTransaction">Reference to an existing database transaction</param>
        public virtual void Delete(TDeleteDTO entity, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            // Ensure Model is Valid
            if (!IsModelValid)
                throw new Exception("Model is invalid");

            try
            {
                // Converts to the Model
                TModel model = entity.MapToModel();

                // Initializes a transaction, if applicable
                IDbConnection connection = dbConnection;
                IDbTransaction transaction = dbTransaction;

                try
                {
                    // Creates a connection if no connection has been passed
                    if (connection == null)
                        connection = DbContext.GetConnection();

                    // Initializes a transaction if no transaction has been passed
                    if (transaction == null)
                        transaction = connection.BeginTransaction();

                    // Check OnBeforeDelete
                    if (!OnBeforeDelete(model, entity, connection, transaction))
                        throw new Exception("Delete operation cancelled");

                    // Delete based on Primary Key
                    Delete(GetPrimaryKeyFilters(model), dbConnection, dbTransaction);

                    // Call OnAfterDelete
                    OnAfterDelete(model, entity, connection, transaction);

                    if (dbTransaction == null)
                        transaction.Commit();
                }
                catch
                {
                    // Check if this action is joining a transaction
                    if (dbTransaction == null)
                        transaction.Rollback();

                    throw;
                }
                finally
                {
                    // Closes the connection created on this method
                    if (dbConnection == null)
                        if (connection?.State == ConnectionState.Open)
                            connection.Close();
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Delete the object from the database based on the primary key
        /// </summary>
        /// <param name="entity">The entity containing the data to be deleted</param>
        /// <returns>The task created to run the process asynchronously</returns>
        public async Task DeleteAsync(TDeleteDTO entity)
        {
            var task = new Task(() => Delete(entity, null, null));
            task.Start();

            await task.ConfigureAwait(false);
        }

        /// <summary>
        /// Delete objects from the database based on a <see cref="List{T}"/> of <see cref="FilterField"/>
        /// </summary>
        /// <param name="filters">A <see cref="List{T}"/> of <see cref="FilterField"/> containing the filters</param>
        /// <param name="dbConnection">Reference to an existing database connection</param>
        /// <param name="dbTransaction">Reference to an existing database transaction</param>
        public void Delete(List<FilterField> filters, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            // Ensure Model is Valid
            if (!IsModelValid)
                throw new Exception("Model is invalid");

            try
            {
                // Remove the Object from the Database
                var delete = new DeleteDbOperation(DatabaseTableAttribute.Name);
                delete.Where.AppendQueryFilters(filters, false);

                // Initializes a transaction, if applicable
                IDbConnection connection = dbConnection;
                IDbTransaction transaction = dbTransaction;

                try
                {
                    // Creates a connection if no connection has been passed
                    if (connection == null)
                        connection = DbContext.GetConnection();

                    // Initializes a transaction if no transaction has been passed
                    if (transaction == null)
                        transaction = connection.BeginTransaction();

                    DbContext.ExecuteNonQuery(delete, dbConnection, dbTransaction);

                    if (dbTransaction == null)
                        transaction.Commit();
                }
                catch
                {
                    // Check if this action is joining a transaction
                    if (dbTransaction == null)
                        transaction.Rollback();

                    throw;
                }
                finally
                {
                    // Closes the connection created on this method
                    if (dbConnection == null)
                        if (connection?.State == ConnectionState.Open)
                            connection.Close();
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion CRUD Operations

        /// <summary>
        /// Get the Filters based on the Object Primary Key
        /// </summary>
        /// <param name="entity">The entity to process</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="FilterField"/> containing the filters by Primary Key</returns>
        protected List<FilterField> GetPrimaryKeyFilters(TModel entity)
        {
            // Append Filters to the Query
            var filters = new List<FilterField>();

            foreach (var primaryKey in PrimaryKeys)
            {
                if (primaryKey.Property.PropertyType.IsEnum)
                {
                    // Enumerations have a special treatment as its value need to be parsed from the string that represents it
                    var enumValue = primaryKey.Property.GetValue(entity);
                    object finalValue = null;

                    // If value is null, force it to zero
                    if (enumValue == null)
                        finalValue = 0;
                    else
                        finalValue = Convert.ChangeType(enumValue, primaryKey.Property.PropertyType.GetEnumUnderlyingType());

                    filters.Add(new FilterField(primaryKey.Attribute.Name,
                                                FilterFieldOperations.Equal,
                                                finalValue,
                                                primaryKey.Attribute.Type));
                }
                else
                {
                    filters.Add(new FilterField(primaryKey.Attribute.Name,
                                                FilterFieldOperations.Equal,
                                                primaryKey.Property.GetValue(entity),
                                                primaryKey.Attribute.Type));

                }
            }

            return filters;
        }
    }

    /// <summary>
    /// Defines an object containing the relationship between Property and Field
    /// </summary>
    public class FieldInfo
    {
        /// <summary>
        /// The <see cref="PropertyInfo"/> containing the class property object
        /// </summary>
        public PropertyInfo Property { get; }
        /// <summary>
        /// The <see cref="DatabaseFieldAttribute"/> containing Database Related information
        /// </summary>
        public DatabaseFieldAttribute Attribute { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldInfo"/>
        /// </summary>
        /// <param name="property">The property info</param>
        /// <param name="attribute">The database field attribute</param>
        public FieldInfo(PropertyInfo property, DatabaseFieldAttribute attribute)
        {
            Property = property;
            Attribute = attribute;
        }
    }

}
