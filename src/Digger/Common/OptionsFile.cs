using CommandLine;

namespace Digger.Common
{
    [Verb("optionsfile", HelpText = "Provide action and its options flags value via the file")]
    public class OptionsFile
    {
        [Option('F', "file", Required = true, HelpText = "path to the options file")]
        public string File { get; set; }
    }
}
