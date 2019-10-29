using Digger.Common.Models;
using Digger.Common.Utils;
using Digger.Search;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Digger.Common.Filters
{
    class ExactFilter
    {
        public static IEnumerable<FoundLine> Match(SearchOptions options, string filename, string filenameExt, string[] sourceLines, string line, int lineNo)
        {
            var result = new List<FoundLine>();
            var folderIndex = options.Folders.GetElementIndex(filename);
            foreach (var seekString in options.SeekStrings)
            {
                if (!string.IsNullOrEmpty(line) && line.Contains(seekString))
                {
                    if (options.And.Any() && !line.AllContains(options.And)) continue;
                    var si = Math.Max(0, lineNo - options.BeforeLines);
                    var ei = Math.Min(sourceLines.Length, lineNo + options.AfterLines);
                    var noOfLines = ei - si + 1;
                    var lines = sourceLines.SubArray(si, noOfLines);
                    result.Add(new FoundLine(filename, filenameExt, string.Join(options.Join ? "" : Environment.NewLine, lines), lineNo + 1, seekString, folderIndex));
                }
            }
            return result;
        }
    }
}

