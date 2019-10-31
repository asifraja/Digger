using Digger.Common.Interfaces;
using Digger.Common.Models;
using Digger.Common.Options;
using Digger.Common.Utils;
using Digger.Search.Output;
using Digger.Common.Commands;
using Digger.Common.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Digger.Search.Process;

namespace Digger.Search
{
    public class SearchCommand : BaseCommand, ICommand
    {
        public SearchCommand(GenericOptions genericOptions) : base(genericOptions)
        {
        }

        public void Execute()
        {
            var options = Options as SearchOptions;
            try
            {
                Parallel.For(0, Stats.TotalFiles, fileIndex =>
                {
                    {
                        var sourceLines = File.ReadAllLines(Files[fileIndex]);
                        string filenameExt = Path.GetExtension(Files[fileIndex]).ToLower();
                        Stats.FoundFiles++;
                        for (var lineNo = 0; lineNo < sourceLines.Length; lineNo++)
                        {
                            var line = sourceLines[lineNo];
                            var lines = options.CaseSensitive ? ExactFilter.Match(options, Files[fileIndex], filenameExt, sourceLines, line, lineNo) : ContainsFilter.Match(options, Files[fileIndex], filenameExt, sourceLines, line, lineNo);
                            if (lines.Count() > 0)
                            {
                                foreach (var processedLine in lines)
                                {
                                    foundLines.Add(processedLine);
                                }
                            }
                        }
                    }
                });
            }
            finally
            {
                foundLines.CompleteAdding();
            }

            var result = foundLines.GetConsumingEnumerable().ToList();

            if (result.Any())
            {
                ProcessHtmlOutput(Options as SearchOptions, result);
                ProcessConsoleOutput(Options as SearchOptions, result);
                ProcessFileUpdateRelatedDirectives(Options as SearchOptions, result);
            }
        }

        private void ProcessHtmlOutput(SearchOptions options, IEnumerable<FoundLine> foundLines)
        {
            if (string.IsNullOrEmpty(options.Output) || !options.Output.Contains(".htm", StringComparison.OrdinalIgnoreCase)) return;
            // Following creates groups by seek string / text
            var seekStringCollection =
                from line in foundLines
                group line by line.SeekString into seekStringGroup
                orderby seekStringGroup.Key
                select seekStringGroup;

            new HtmlOutput(options, seekStringCollection, Stats).Execute();
        }

        private void ProcessConsoleOutput(SearchOptions options, IEnumerable<FoundLine> foundLines)
        {
            if (string.IsNullOrEmpty(options.Output) || !options.Output.Equals("console", StringComparison.OrdinalIgnoreCase)) return;
            // Following creates groups by seek string / text
            var seekStringCollection =
                from line in foundLines
                group line by line.SeekString into seekStringGroup
                orderby seekStringGroup.Key
                select seekStringGroup;

            new ConsoleOutput(options, seekStringCollection, Stats).Execute();
        }

        private void ProcessFileUpdateRelatedDirectives(SearchOptions options, IEnumerable<FoundLine> foundLines)
        {
            if (!options.PurgeLine && !options.Find.Any()) return;
            
            // Following creates groups by filename
            var fileCollection =
                from line in foundLines
                group line by line.Filename into fileGroup
                orderby fileGroup.Key
                select fileGroup;

            new ProcessFiles(options, fileCollection, Stats).Execute();
        }
    }
}
