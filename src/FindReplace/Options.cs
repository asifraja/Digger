using CommandLine;
using Plugins;
using System.Collections.Generic;

namespace FindReplace 
{
    //https://github.com/commandlineparser/commandline/wiki
    public class Options 
    {
        [Option('F', "folder", Required = true, HelpText = "one or more folder paths")]
        public IEnumerable<string> Folders { get; set; }

        [Option('E', "ext", Required = true, HelpText = "one or more files extensions type of files to search")]
        public IEnumerable<string> Exts { get; set; }

        [Option('S', "seek", Required = true, HelpText = "filter files based on one or more text/strings")]
        public IEnumerable<string> SeekStrings { get; set; }

        [Option('x', "exclude", HelpText = "exclude folders or files during file treversing with sub-folders")]
        public IEnumerable<string> ExcludeFolders { get; set; }

        [Option('o', "output", Default = "console", Required = false, HelpText = "export output to file")]
        public string Output { get; set; }

        [Option('n', "nested", Default = true, HelpText = "nested search in sub-folders on each provided folder")]
        public bool Recursive { get; set; }

        [Option('v', "verbose", Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }
        
        [Option('u', "buffer", Default = 2048, HelpText = "output max number of chars for of line")]
        public int BufferSize { get; set; }

        [Option('c', "case-sensitive", Default = false, HelpText = "case sensitive search")]
        public bool CaseSensitive { get; set; }

        [Option('e', "extract", HelpText = "find extact value between start and end string i.e. tokenStart***tokenEnd, must have three *")]
        public IEnumerable<string> Extract { get; set; }

        [Option('f', "find", HelpText = "find token text/string to find|replace with")]
        public IEnumerable<string> Find { get; set; }

        [Option("for", HelpText = "locate files for certain but only files that were found by the -S text")]
        public string For { get; set; }

        [Option('u', "commit", Default = false, HelpText = "commit and update files, see flags -d, --find/--replace.")]
        public bool Commit { get; set; }

        [Option('b', "before", Default = 0, HelpText = "include additional lines before the found text, see flag --text.")]
        public int BeforeLines { get; set; }

        [Option('a', "after", Default = 0, HelpText = "include additional lines after the found text, see --text")]
        public int AfterLines { get; set; }

        [Option('j', "join", Default = false, HelpText = "join/concatenate all lines, see flags --after and --before.")]
        public bool Join { get; set; }

        [Option('p', "purge", Default = false, HelpText = "purge line(s) where text/string was found, see flag --commit and --text")]
        public bool PurgeLine { get; set; }

        [Option("and", HelpText = "Seek must have and text on the same line")]
        public IEnumerable<string> And { get; set; }
    }
}
