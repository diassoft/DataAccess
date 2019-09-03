using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.Models.DTO
{
    /// <summary>
    /// Represents the Base Class of a Data Transfer Object
    /// </summary>
    /// <typeparam name="TModelType">Type of the Base Model</typeparam>
    public abstract class DTOModelBase<TModelType>: IDTOModel<TModelType>
    {
        /// <summary>
        /// Maps the Current DTO to a Model
        /// </summary>
        /// <returns>An instance of the model defined by <typeparamref name="TModelType"/></returns>
        public virtual TModelType MapToModel()
        {
            return PropertyMapper.Map<TModelType>(this);
        }

        /// <summary>
        /// Updates the DTO with the information from the model
        /// </summary>
        /// <param name="model">Reference to the Model</param>
        public virtual void MapFromModel(TModelType model)
        {
            PropertyMapper.Map(model, this);
        }
    }
}
