using CommandLine;
using Digger.Common.Options;

namespace Digger.Compare
{
    [Verb("compare", HelpText = "Compare difference between files and folders")]
    public class CompareOptions : GenericOptions
    {
        [Option('W', "with", Required = true, HelpText = "file or folder to compare with")]
        public string WithFolder { get; set; }
        [Option('i', "inline", Required = false, HelpText = "default is side by side view, set it to display inline view")]
        public bool Inline { get; set; }
        [Option('m', "missing", Required = false, HelpText = "show missing files in either folders")]
        public bool IncludeMissing { get; set; }
    }
}
