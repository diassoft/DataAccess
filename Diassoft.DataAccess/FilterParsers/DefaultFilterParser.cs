using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Diassoft.DataAccess.FilterParsers
{
    /// <summary>
    /// Represents the Default Parser for Query String parameters
    /// </summary>
    public class DefaultFilterParser: IFilterParser
    {
        private readonly NumberStyles defaultNumberStyle = NumberStyles.Number | NumberStyles.AllowDecimalPoint;
        private readonly CultureInfo defaultCulture = CultureInfo.InvariantCulture;

        /// <summary>
        /// Parse Filters into a List of <see cref="FilterField"/>
        /// </summary>
        /// <param name="input">The string input</param>
        /// <returns></returns>
        public List<FilterField> Parse(string input)
        {
            // Variable to store the results
            var results = new List<FilterField>();

            // Ensure there is some input, if not, return empty list
            if (String.IsNullOrEmpty(input)) return results;

            // =================================================================================================
            // Examples
            //
            // field1=filterData;field2+neq=filterData2
            // field1+in=(filterData1,filterData2,filterData3);field2+notin=(filterData1,filterData2)
            //
            // Fields are separated by ";".
            // Each field will have the "=" operator.
            //
            // =================================================================================================
            // Formats
            //
            // -----------------------------------------------------------------
            // Date/Time
            // -----------------------------------------------------------------
            // Date Fields.......: 2012-04-23
            // Date Time Fields..: 2012-04-23T18:25:43.511Z
            //
            // -----------------------------------------------------------------
            // Number
            // -----------------------------------------------------------------
            // Integer...........: 10000
            // Decimal...........: 3.1415 (always use . as the separator)
            // Decimal...........: 105200.25 (never use thousands separator)
            //
            // =================================================================================================

            // Check fields coming from the input
            var filtersArray = input.Split(';');

            // Loop thru each filter to parse it
            foreach (var f in filtersArray)
            {
                string fieldName = "";
                string fieldValue = "";

                int positionFieldEnd = f.IndexOf('=');

                // If there is no '=', ignore it
                if (positionFieldEnd > 0)
                {
                    // Field Information
                    var filterField = new FilterField();

                    // Retrieve Field Name and Value. (do not use Split because we can have '=' on the filter contents)
                    fieldName = f.Substring(0, positionFieldEnd);
                    fieldValue = f.Substring(positionFieldEnd+1, f.Length - positionFieldEnd -1);
                    int positionBlankSpace = fieldName.IndexOf(' ');

                    if (positionBlankSpace > 0)
                    {
                        // There is an operator that has to be parsed too
                        filterField.Field = fieldName.Substring(0, positionBlankSpace);

                        var operatorString = fieldName.Substring(positionBlankSpace+1, fieldName.Length - positionBlankSpace - 1).Trim().ToUpper();

                        switch (operatorString)
                        {
                            case "NEQ": filterField.Operator = FilterFieldOperations.NotEqual; break;
                            case "IN": filterField.Operator = FilterFieldOperations.In; break;
                            case "NOTIN": filterField.Operator = FilterFieldOperations.NotIn; break;
                            case "GT": filterField.Operator = FilterFieldOperations.GreaterThan; break;
                            case "GE": filterField.Operator = FilterFieldOperations.GreaterThanOrEqualTo; break;
                            case "LT": filterField.Operator = FilterFieldOperations.LessThan; break;
                            case "LE": filterField.Operator = FilterFieldOperations.LessThanOrEqualTo; break;
                            default:
                                filterField.Operator = FilterFieldOperations.Equal;
                                break;
                        }
                    }
                    else
                    {
                        // The entire contents is the field name
                        filterField.Field = fieldName;
                    }

                    // Check field value
                    if ((filterField.Operator == FilterFieldOperations.In) ||
                        (filterField.Operator == FilterFieldOperations.NotIn))
                    {
                        // Remove Paranthesis from begining and ending of the string
                        if (fieldValue[0] == '(')
                            fieldValue = fieldValue.Remove(0, 1);

                        if (fieldValue[fieldValue.Length - 1] == ')')
                            fieldValue = fieldValue.Substring(0, fieldValue.Length - 1);

                        // Get the data array
                        var fieldDataArray = fieldValue.Split(',');

                        // Loop thru all values
                        var fieldValuesList = new List<object>();
                        foreach (var fd in fieldDataArray)
                        {
                            var inferredDataType = InferDataType(fd);
                            if (filterField.Type == null)
                                filterField.Type = inferredDataType.Type;

                            fieldValuesList.Add(inferredDataType.Value);
                        }

                        filterField.OriginalValue = fieldValue;
                        filterField.Value = fieldValuesList.ToArray();
                    }
                    else
                    {
                        // Operator has a single value
                        var inferredDataType = InferDataType(fieldValue);

                        filterField.OriginalValue = fieldValue;
                        filterField.Value = inferredDataType.Value;
                        filterField.Type = inferredDataType.Type;
                    }

                    // Add to the Results
                    results.Add(filterField);
                }
            }

            return results;
        }

        /// <summary>
        /// Function to try to infer the data type and value based on the input string
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>A tuple containing the type and the value</returns>
        private InferredDataType InferDataType(string input)
        {
            // Check for Numeric Value
            if (Double.TryParse(input, defaultNumberStyle, defaultCulture, out Double doubleValue))
            {
                return new InferredDataType(typeof(double), doubleValue);
            }

            // Check for DateTime Value
            var dateFormats = new string[] { "yyyy-MM-dd", "yyyy-MM-dd'T'HH':'mm':'ss'.'fff'Z'" };

            if (DateTime.TryParseExact(input, dateFormats, defaultCulture, DateTimeStyles.None, out DateTime dateTimeValue))
            {
                return new InferredDataType(typeof(System.DateTime), dateTimeValue);
            }

            // Assume it is a string
            return new InferredDataType(typeof(System.String), input);
        }

    }
    
    /// <summary>
    /// Represents an internal class to be used for the <see cref="DefaultFilterParser.InferDataType(string)"/> method
    /// </summary>
    internal class InferredDataType
    {
        /// <summary>
        /// The type inferred
        /// </summary>
        public Type Type { get; }
        /// <summary>
        /// The value inferred
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InferredDataType"/>
        /// </summary>
        /// <param name="type">The type inferred</param>
        /// <param name="value">The value inferred</param>
        public InferredDataType(Type type, object value)
        {
            this.Type = type;
            this.Value = value;
        }
    }

}
