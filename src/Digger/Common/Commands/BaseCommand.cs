using Digger.Common.Models;
using Digger.Common.Options;
using Digger.Common.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Digger.Common.Commands
{
    public abstract class BaseCommand
    {
        public CommandStats Stats { get; }
        private BaseCommand()
        {
            Stats = new CommandStats();
        }

        public BaseCommand(GenericOptions genericOptions) : this()
        {
            Files = GetFiles(genericOptions.Folders, genericOptions.ExcludeFolders, genericOptions.Exts, genericOptions.Recursive).ToArray();
            Stats.TotalFiles = Files.Length;
            Options = genericOptions;
        }
        protected BlockingCollection<FoundLine> foundLines = new BlockingCollection<FoundLine>();

        public string[] Files { get; }
        public GenericOptions Options { get; }
        public IEnumerable<string> GetFiles(IEnumerable<string> folders, IEnumerable<string> excludeFolders, IEnumerable<string> exts, bool recursive)
        {
            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = folders.AsParallel().SelectMany(path => exts.AsParallel().SelectMany(searchPattern => Directory.EnumerateFiles(path, searchPattern, searchOption)));
            return files.Where(f => excludeFolders.All(e => !@f.Contains(@e, StringComparison.OrdinalIgnoreCase))).ToArray();
        }
    }
}
