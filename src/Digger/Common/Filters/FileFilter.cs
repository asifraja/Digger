using Digger.Common.Models;
using Digger.Common.Utils;
using Digger.Search;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Digger.Common.Filters
{
    class FileFilter
    {
        public static bool Match(SearchOptions options, string filename, string filenameExt)
        {
            if (string.IsNullOrEmpty(options.For)) return true;
            var sourceLines = File.ReadAllLines(filename);
            for (var lineNo = 0; lineNo < sourceLines.Length; lineNo++)
            {
                var line = sourceLines[lineNo]+ " ";
                if (!string.IsNullOrEmpty(line) && (( options.CaseSensitive && line.Contains(options.For)) || (!options.CaseSensitive && line.Contains(options.For, StringComparison.OrdinalIgnoreCase))))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

