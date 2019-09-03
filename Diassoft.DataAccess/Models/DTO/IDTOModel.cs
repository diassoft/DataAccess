using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.Models.DTO
{
    /// <summary>
    /// An interface to be implemented by Data Transfer Objects (DTO) models
    /// </summary>
    /// <remarks>Dto Models are representations of a model but with specific parameters to accept user input. 
    /// Example: a model may contain an internal Unique ID auto generated, the Dto Model do not need to have this field.</remarks>
    /// <typeparam name="TModel">Base Model of the DTO</typeparam>
    public interface IDTOModel<TModel>
    {
        /// <summary>
        /// Map the Dto Model to a Model
        /// </summary>
        /// <returns>A instance of <typeparamref name="TModel"/> containing the conversion of the Dto Model</returns>
        TModel MapToModel();
        /// <summary>
        /// Apply model information to the current DTO model
        /// </summary>
        /// <param name="model">The model to copy the data from</param>
        void MapFromModel(TModel model);
    }
}
