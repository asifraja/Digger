using Digger.Common.Models;
using Digger.Common.Utils;
using Digger.Search;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Digger.Common.Filters
{
    class LineFilter
    {
        public static IEnumerable<FoundLine> Match(SearchOptions options, string filename, string filenameExt, string[] sourceLines, string line, int lineNo)
        {
            var result = new List<FoundLine>();
            var folderIndex = options.Folders.ToArray().GetElementIndex(filename);
            var lineUpdated = false;
            foreach (var seekString in options.SeekStrings)
            {
                if (!string.IsNullOrEmpty(line) && ((options.CaseSensitive && line.Contains(seekString)) || (!options.CaseSensitive && line.Contains(seekString, StringComparison.OrdinalIgnoreCase))))
                {
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
                                var mode = 0;
                                var toFind = find;
                                var filter = string.Empty;
                                var extractedText = string.Empty;

                                if (toFind.Contains("***"))
                                {
                                    mode = 1;
                                    filter = toFind.Trim().Substring(1, toFind.IndexOf(']') - 1);
                                    toFind = toFind.Substring(toFind.IndexOf(']') + 1);
                                    extractedText = lines[lineNdx].ExtractUsingTokens(new[] { filter }).FirstOrDefault();
                                }
                                else if (toFind.Contains('[') && toFind.Contains(']'))
                                {
                                    mode = 2;
                                    filter = toFind.Trim().Substring(1, toFind.IndexOf(']')-1);
                                    toFind = toFind.Substring(toFind.IndexOf(']')+1);
                                }
                                if (toFind.Contains('|'))
                                {
                                    var findParts = toFind.Split('|');
                                    if (lines[lineNdx].Contains(findParts[0], StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (mode == 1 && !string.IsNullOrEmpty(extractedText))
                                        {
                                            lineUpdated = true;
                                            lines[lineNdx] = lines[lineNdx].Replace(findParts[0], findParts[1].Replace("$$", extractedText));
                                        }
                                        else if ((mode == 2 && lines[lineNdx].Contains(filter, StringComparison.OrdinalIgnoreCase)) || (mode == 0))
                                        {
                                            lineUpdated = true;
                                            lines[lineNdx] = lines[lineNdx].Replace(findParts[0], findParts[1]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (options.And.Any() && ((options.CaseSensitive && !previousLine.AllContains(options.And)) || (!options.CaseSensitive && !previousLine.AllContains(options.And, StringComparison.OrdinalIgnoreCase)))) continue;
                    if (options.Not.Any() && ((options.CaseSensitive && previousLine.AtleastOneContains(options.Not)) || (!options.CaseSensitive && previousLine.AtleastOneContains(options.Not, StringComparison.OrdinalIgnoreCase)))) continue;
                    if (options.Or.Any() && ((options.CaseSensitive && !previousLine.AtleastOneContains(options.Or)) || (!options.CaseSensitive && !previousLine.AtleastOneContains(options.Or, StringComparison.OrdinalIgnoreCase)))) continue;
                    result.Add(new FoundLine(filename, filenameExt, string.Join(options.Join ? "" : Environment.NewLine, lines), lineUpdated && !options.Mute ? previousLine : string.Empty, lineNo + 1, seekString, folderIndex,true));
                }
            }
            return result;
        }
    }
}

