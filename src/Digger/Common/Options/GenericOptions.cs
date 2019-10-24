using CommandLine;
using System.Collections.Generic;

namespace Digger.Common.Options
{
    public class GenericOptions : BaseOptions
    {
        [Option('F', "folder", Required = true, HelpText = "one or more folder paths")]
        public IEnumerable<string> Folders { get; set; }

        [Option('E', "ext", Required = true, HelpText = "one or more files extensions type of files to search")]
        public IEnumerable<string> Exts { get; set; }

        [Option('x', "exclude", HelpText = "exclude folders or files during file treversing with sub-folders")]
        public IEnumerable<string> ExcludeFolders { get; set; }

        [Option('o', "output", Default="console", Required = false, HelpText = "export output to file")]
        public string Output { get; set; }

        [Option('n', "nested", Default = true, HelpText = "nested search in sub-folders on each provided folder")]
        public bool Recursive { get; set; }

        [Option('v', "verbose", Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

    }
}
