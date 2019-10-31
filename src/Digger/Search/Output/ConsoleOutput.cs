using Digger.Common.Interfaces;
using Digger.Common.Models;
using Digger.Common.Utils;
using System;
using System.Linq;
using System.Text;

namespace Digger.Search.Output
{
    public class ConsoleOutput : SearchProcess, IProcess
    {
        public ConsoleOutput(SearchOptions options, IOrderedEnumerable<IGrouping<string, FoundLine>> foundlines, CommandStats stats): base(options, foundlines, stats)
        {
        }

        public void Execute()
        {
            foreach (var seekStringGroup in FoundLineCollection)
            {
                var title = $"Searched: {seekStringGroup.Key} found ({seekStringGroup.Count()}) instances.";
                if (Options.Verbose)
                {
                    Console.WriteLine(new string('-', title.Length));
                    Console.WriteLine(title);
                    Console.WriteLine(new string('-', title.Length));
                }
                if (Options.Extract.Any())
                {
                    StringBuilder allLines = new StringBuilder();
                    foreach (var foundFile in seekStringGroup.OrderBy(f => f.FolderIndex).ThenBy(f => f.Filename).ThenBy(f => f.LineNo))
                    {
                        allLines.Append(foundFile.Line);
                    }
                    var allText = allLines.ToString().Replace(Environment.NewLine, "<<EOL>>").Replace("\t", "<<TAB>>");
                    var results = allText.ExtractUsingTokens(Options.Extract);
                    //if (Options.Verbose) Console.WriteLine(allText.Replace("<<EOL>>", Environment.NewLine).Replace("<<TAB>>", "\t"));
                    Stats.TotalInstances += results.Count();
                    foreach (var result in results)
                    {
                        Console.WriteLine(result);
                    }
                }
                else
                {
                    Stats.TotalInstances += seekStringGroup.Count();
                    foreach (var foundFile in seekStringGroup.OrderBy(f => f.FolderIndex).ThenBy(f => f.Filename).ThenBy(f => f.LineNo))
                    {
                        var line = foundFile.Line.Substring(1, Math.Min(foundFile.Line.Length, 4048) - 1).TrimStart() + (foundFile.Line.Length > 4047 ? "<b>...</b>" : "");
                        if (Options.Verbose)
                        {
                            Console.WriteLine($"[{foundFile.LineNo}] [{foundFile.Filename}]");
                        }
                        Console.WriteLine($"{line}");
                    }
                }
            }
            Console.WriteLine();
        }
    }
}
