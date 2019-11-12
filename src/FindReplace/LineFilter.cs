using Common.Models;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FindReplace
{
    class LineFilter
    {
        public static IEnumerable<FoundLine> Match(FolderInfo folderInfo, Options options, int folderIndex, string filename, string filenameExt, string[] sourceLines, string line, int lineNo)
        {
            var result = new List<FoundLine>();
            var lineUpdated = false;
            foreach (var seekString in options.SeekStrings)
            {
                if (!string.IsNullOrEmpty(line) && ((options.CaseSensitive && line.Contains(seekString)) || (!options.CaseSensitive && line.Contains(seekString, StringComparison.OrdinalIgnoreCase))))
                {
                    folderInfo.SeekStrings[seekString] = folderInfo.SeekStrings[seekString] + 1;
                    var si = Math.Max(0, lineNo - options.BeforeLines);
                    var ei = Math.Min(sourceLines.Length, lineNo + options.AfterLines);
                    var noOfLines = ei - si + 1;
                    var lines = sourceLines.SubArray(si, noOfLines);
                    // apply find and replace
                    var previousLine = string.Join(options.Join ? "" : Environment.NewLine, lines);
                    if (options.Find.Any())
                    {
                        for (var lineNdx = 0; lineNdx < lines.Length; lineNdx++)
                        {
                            foreach (var find in options.Find)
                            {
                                if (find.Contains('|'))
                                {
                                    var findParts = find.Split('|');
                                    if (lines[lineNdx].Contains(findParts[0]))
                                    {
                                        lineUpdated = true;
                                        lines[lineNdx] = lines[lineNdx].Replace(findParts[0], findParts[1]);
                                    }
                                }
                            }
                        }
                    }
                    if (options.And.Any() && ((options.CaseSensitive && !previousLine.AllContains(options.And)) || (!options.CaseSensitive && !previousLine.AllContains(options.And, StringComparison.OrdinalIgnoreCase)))) continue;
                    result.Add(new FoundLine(filename, filenameExt, string.Join(options.Join ? "" : Environment.NewLine, lines), lineUpdated ? previousLine : string.Empty, lineNo + 1, seekString, folderIndex, true));
                }
            }
            return result;
        }
    }
}