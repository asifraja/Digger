using Digger.Common.Interfaces;
using Digger.Common.Models;
using Digger.Search.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digger.Search.Process
{

    public class ProcessFiles : SearchProcess, IProcess
    {
        public ProcessFiles(SearchOptions options, IOrderedEnumerable<IGrouping<string, FoundLine>> foundFiles, CommandStats stats) : base(options, foundFiles, stats)
        {
        }

        public void Execute()
        {
            foreach (var fileGroup in FoundLineCollection)
            {
                var filename = fileGroup.Key;
                var nonDeletedContent = new StringBuilder();
                var content = string.Empty;
                var fileIsUpdated = false;
                if (!File.Exists(filename)) continue;
                {
                    var lines = File.ReadAllLines(filename);
                    var deletedLines = new int[lines.Length];
                    var remainingLines = new List<string>();
                    foreach (var foundLine in fileGroup.OrderBy(o => o.LineNo))
                    {
                        var lineNo = foundLine.LineNo - 1;
                        if (Options.PurgeLine)
                        {
                            var s = Math.Max(0, lineNo - Options.BeforeLines);
                            var e = Math.Min(lines.Length - 1, lineNo + Options.AfterLines);
                            for (int i = s; i <= e; i++)
                            {
                                deletedLines[i] = -1;
                            }
                        }
                        if(foundLine.LineIsUpdated)
                        {
                            fileIsUpdated = true;
                            lines[lineNo] = foundLine.Line;
                        }
                    }
                    for (var i = 0; i < lines.Length; i++)
                    {
                        // delete marked lines
                        if (deletedLines[i] == -1)
                        {
                            fileIsUpdated = true;
                        }
                        else
                        {
                            nonDeletedContent.AppendLine(lines[i]);
                        }
                    }
                    content = nonDeletedContent.ToString();
                    // update file if it need updating. 
                    if (!string.IsNullOrEmpty(content))
                    {
                        if (fileIsUpdated)
                        {
                            if (Options.Commit)
                                File.WriteAllText(filename, content);
                            else
                                File.WriteAllText(@"D:\temp\updated-file.txt", content);
                        }
                    }
                }
            }
        }
    }
}
