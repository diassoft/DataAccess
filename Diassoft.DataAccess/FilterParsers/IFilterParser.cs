using System;
using System.Collections.Generic;
using System.Text;

namespace Diassoft.DataAccess.FilterParsers
{
    /// <summary>
    /// Interface to be implemented by Query String Filter parsers
    /// </summary>
    public interface IFilterParser
    {
        /// <summary>
        /// Parse the query string input to a <see cref="List{T}"/> of <see cref="FilterField"/>
        /// </summary>
        /// <param name="input">A string containing the raw query string filters</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="FilterField"/> containing the filters extracted from the query string</returns>
        List<FilterField> Parse(string input);
    }
}
