using CommandLine;
using Common.Models;
using Common.Utils;
using Plugins;
using System.Collections.Concurrent;

namespace FindReplace
{
    public class Command : IPluginAssembly
    {
        public string Name { get => "Find and Replace"; }
        public string Description { get => "Provide blazingly fast search/replace within files"; }
        public string Verb { get => "search"; }
        public BlockingCollection<FolderInfo> FolderInfos { get; private set; }
        public long CompletedInMilliseconds { get; private set; }
        protected Options Options;

        public long Execute(string[] args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(options =>
            {
                Options = options;
                FolderInfos = FileUtils.GetFolderInfos(options.Folders, options.Exts, options.ExcludeFolders, options.Recursive);
            });
            watch.Stop();
            CompletedInMilliseconds = watch.ElapsedMilliseconds;
            return CompletedInMilliseconds;
        }
    }
}