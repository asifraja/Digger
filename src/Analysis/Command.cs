using CommandLine;
using Common.Models;
using Common.Utils;
using Plugins;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Analysis
{
    public class Command : IPluginAssembly
    {
        public string Name { get => "Analyse"; }
        public string Description { get => "Provide blazingly fast analysis within files"; }
        public string Verb { get => "analysis"; }
        public BlockingCollection<FolderInfo> FolderInfos { get; private set; }
        public long CompletedInMilliseconds { get; private set; }
        protected Options Options;

        public IEnumerable<FolderInfo> Execute(string[] args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(options =>
            {
                Options = options;
                FolderInfos = FileUtils.GetFolderInfos(options.SeekStrings, options.Folders, options.Exts, options.ExcludeFolders, options.Recursive);
            });

            if (args.Length <= 1) return null;

            FolderInfos.AsParallel().ForAll(Process);
            watch.Stop();
            CompletedInMilliseconds = watch.ElapsedMilliseconds;
            return FolderInfos.ToList();
        }

        private void Process(FolderInfo folderInfo)
        {
            folderInfo.Files.AsParallel().ForAll(f=> { ProcessFile(folderInfo, f); });
        }

        private void ProcessFile(FolderInfo folderInfo, FileDetails details)
        {
            if (File.Exists(details.Path))
            {
                var lines = File.ReadAllLines(details.Path);
                var lineIndex = 0;
                foreach (var line in lines)
                { 
                    var analyser = new CsFileAnalyser(lines, details.Path);
                    LineAnalyser.Analyse(folderInfo, Options, folderInfo.Order, details.Path, Path.GetExtension(details.Path), lines, line, lineIndex);                    
                    //foundLines.AsParallel().ForAll(f=>details.FoundLines.Add(f));
                    lineIndex = lineIndex + lines.Length;
                }
            }
        }
    }
}