using Digger.Common.Interfaces;
using Digger.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digger.Search.Output
{

    public class UpdateFiles : SearchProcess, IProcess
    {
        public UpdateFiles(SearchOptions options, IOrderedEnumerable<IGrouping<string, FoundLine>> foundFiles, CommandStats stats): base(options, foundFiles, stats)
        {
        }

        public void Process()
        {
            foreach (var fileGroup in FoundLineCollection)
            {
                var filename = fileGroup.Key;
                var nonDeletedContent = new StringBuilder();
                var content = string.Empty;
                var fileIsUpdated = false;
                if (!File.Exists(filename)) continue;
                // delete lines
                if (Options.PurgeLine)
                {
                    var lines = File.ReadAllLines(filename);
                    var deletedLines = new int[lines.Length];
                    var remainingLines = new List<string>();
                    foreach (var foundLine in fileGroup.OrderBy(o=>o.LineNo))
                    {
                        var s = Math.Max(0,foundLine.LineNo - 1 - Options.BeforeLines);
                        var e = Math.Min(lines.Length-1, foundLine.LineNo - 1 + Options.AfterLines);
                        for(int i=s; i<=e; i++)
                        {
                            deletedLines[i] = -1;
                        }
                    }
                    for(var i=0;i< lines.Length;i++)
                    {
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
                }
                else
                {
                    content = File.ReadAllText(filename);
                }
                // update file if it need updating. 
                if (!string.IsNullOrEmpty(content))
                {
                    var foundFind = string.IsNullOrEmpty(Options.Find) ? false:content.Contains(Options.Find);
                    if ((foundFind || fileIsUpdated))
                    {
                        if (Options.Commit)
                            File.WriteAllText(filename, foundFind?content.Replace(Options.Find, Options.Replace):content);
                        else
                            File.WriteAllText(@"D:\temp\updated-file.txt", foundFind ? content.Replace(Options.Find, Options.Replace) : content);
                    }
                }
            }
        }
    }
}
