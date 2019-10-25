using CommandLine;
using Digger.Common;
using Digger.Compare;
using Digger.Search;
using System;
using System.IO;
using System.Linq;

namespace Digger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine($" Digger cli - Copyright 2019-{DateTime.UtcNow.Year + 1}.");
            Console.WriteLine($" Author: Asif Raja.");
            Console.WriteLine("---------------------------------------------------------------");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Parser.Default.ParseArguments<SearchOptions, CompareOptions, OptionsFile>(args)
            .WithParsed<CompareOptions>(options =>
            {
                CompareExecute(options);
            })
            .WithParsed<OptionsFile>(options =>
            {
                var parafile = File.ReadAllLines(options.File);
                if (parafile[0].ToLowerInvariant().StartsWith("search"))
                {
                    Parser.Default.ParseArguments<SearchOptions>(parafile).WithParsed<SearchOptions>(opts => { SearchExecute(opts); });
                }
                else if (parafile[0].ToLowerInvariant().StartsWith("compare"))
                {
                    Parser.Default.ParseArguments<CompareOptions>(parafile).WithParsed<CompareOptions>(opts => { CompareExecute(opts); });
                }
            })
            .WithParsed<SearchOptions>(options =>
            {
                SearchExecute(options);
            });
            watch.Stop();
            Console.WriteLine($" Finished in {watch.Elapsed.TotalSeconds} second(s) / {watch.Elapsed.TotalMinutes} minute(s).");
        }


        private static void SearchExecute(SearchOptions options)
        {
            Console.WriteLine("Started to Search, please wait...");
            var command = new SearchCommand(options);
            command.Execute();
            Console.WriteLine("Summary");
            Console.WriteLine($" Number of files searched were {command.Stats.TotalFiles}.");
            Console.WriteLine($" Number of instances of text/strings found were {command.Stats.TotalInstances}.");
            Console.WriteLine($" Number files found that contained text/strings were {command.Stats.FoundFiles}.");
        }

        private static void CompareExecute(CompareOptions options)
        {
            if (options.Folders.Count() != 1) throw new ArgumentOutOfRangeException("--folder must provide single folder/path");
            Console.WriteLine("Started to Compare, please wait...");
            var command = new CompareCommand(options);
            command.Execute();
            Console.WriteLine("Summary");
            Console.WriteLine($" Same name files existed and compared were {command.Stats.TotalFiles}.");
            Console.WriteLine($" Number of instances of files found at both sides were {command.Stats.TotalInstances}.");
            Console.WriteLine($" Number of files found that were different {command.Stats.FoundFiles}.");
        }
    }
}
