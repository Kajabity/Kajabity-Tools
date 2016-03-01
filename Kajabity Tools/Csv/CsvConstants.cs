using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kajabity.Tools.Csv
{
    /// <summary>
    /// A class holding constants shared by both CSV reader and writer.
    /// </summary>
    public class CsvConstants
    {
        /// <summary>
        /// The default separator character for use by the CSV reader and writer.
        /// </summary>
        public const char DEFAULT_SEPARATOR_CHAR = ',';

        /// <summary>
        /// The default quote character for use by the CSV reader and writer.
        /// </summary>
        public const char DEFAULT_QUOTE_CHAR = '"';
    }
}
