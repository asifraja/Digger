using CommandLine;
using Digger.Common.Options;
using System.Collections.Generic;
using System.Text;

namespace Digger.Search
{
    [Verb("optionsfile", HelpText = "Provide action and its options flags value via the file")]
    public class OptionsFile 
    {
        [Option('F', "file", Required = true, HelpText = "path to the options file")]
        public string Parafile { get; set; }
    }
}
