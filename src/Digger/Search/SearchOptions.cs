﻿using CommandLine;
using Digger.Common.Options;
using System.Collections.Generic;
using System.Text;

namespace Digger.Search
{
    [Verb("search", HelpText = "Search for one or more text/strings")]
    public class SearchOptions : GenericOptions
    {
        [Option('S', "seek", Required = true, HelpText = "filter files based on one or more text/strings")]
        public IEnumerable<string> SeekStrings { get; set; }

        [Option('c', "case-sensitive", Default = false, HelpText = "case sensitive search")]
        public bool CaseSensitive { get; set; }
        [Option('e', "extract", HelpText = "find extact value between start and end string i.e. tokenStart***tokenEnd, must have three *")]
        public IEnumerable<string> Extract { get; set; }

        [Option('f', "find", HelpText = "find token text/string to find|replace with")]
        public IEnumerable<string> Find { get; set; }
        [Option("for", HelpText = "locate files for certain but only files that were found by the -S text")]
        public string For { get; set; }

        [Option("commit", Default = false, HelpText = "commit and update files, see flags -d, --find/--replace.")]
        public bool Commit { get; set; }

        [Option('b', "before", Default = 0, HelpText = "include additional lines before the found text, see flag --text.")]
        public int BeforeLines { get; set; }

        [Option('a', "after", Default = 0, HelpText = "include additional lines after the found text, see --text")]
        public int AfterLines { get; set; }

        [Option('j', "join", Default = false, HelpText = "join/concatenate all lines, see flags --after and --before.")]
        public bool Join { get; set; }

        [Option('p', "purge", Default = false, HelpText = "purge line(s) where text/string was found, see flag --commit and --seek")]
        public bool PurgeLine { get; set; }

        [Option("and",  HelpText = "Seek must have and text on the same line")]
        public IEnumerable<string> And { get; set; }
        
        [Option("not", HelpText = "Exclude the found line if any of the 'not' string is found")]
        public IEnumerable<string> Not { get; set; }

        [Option("or", HelpText = "Include the found line if any of the or text exists")]
        public IEnumerable<string> Or { get; set; }

        [Option('m', "mute", Default = false, HelpText = "do not display before line after the find and replace operation")]
        public bool Mute { get; set; }
    }
}
