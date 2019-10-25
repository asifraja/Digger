using Digger.Common.Interfaces;
using Digger.Common.Models;
using Digger.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digger.Search.Output
{
    public class ConsoleOutput : SearchProcess, IProcess
    {
        public ConsoleOutput(SearchOptions options, IOrderedEnumerable<IGrouping<string, FoundLine>> foundlines, CommandStats stats): base(options, foundlines, stats)
        {
        }

        public void Process()
        {
            foreach (var seekStringGroup in FoundLineCollection)
            {
                var title = $"Searched: {seekStringGroup.Key} found ({seekStringGroup.Count()}) instances.";
                if (!Options.Extract.Any())
                {
                    Console.WriteLine(new string('-', title.Length));
                    Console.WriteLine(title);
                    Console.WriteLine(new string('-', title.Length));
                }
                Stats.TotalInstances += seekStringGroup.Count();
                if (Options.Verbose)
                {
                    if (Options.Extract.Any())
                    {
                        StringBuilder allLines = new StringBuilder(); 
                        foreach (var foundFile in seekStringGroup.OrderBy(f => f.Filename).ThenBy(f => f.LineNo))
                        {
                            allLines.Append(foundFile.Line);
                        }
                        var allText = allLines.ToString().Replace(Environment.NewLine," ").Replace("\t", " ");
                        var results = allText.ExtractUsingTokens(Options.Extract);
                        var ndx = 0;
                        foreach (var result in results)
                        {
                            Console.Write(result);
                            if (ndx%2==0) Console.Write(" = ");
                            ndx++;
                            if (ndx % 2 == 0) Console.Write(Environment.NewLine);
                        }
                    }
                    else
                    {
                        foreach (var foundFile in seekStringGroup.OrderBy(f => f.Filename).ThenBy(f => f.LineNo))
                        {
                            var line = foundFile.Line.Substring(1, Math.Min(foundFile.Line.Length, 1024) - 1).TrimStart() + (foundFile.Line.Length > 1023 ? "<b>...</b>" : "");
                            Console.WriteLine($"[{foundFile.LineNo}] [{foundFile.Filename}]");
                            Console.WriteLine($"{line}");
                        }
                    }
                }
            }
            Console.WriteLine();
        }
    }
}
