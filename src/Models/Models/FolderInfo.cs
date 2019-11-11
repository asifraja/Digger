using System.Collections.Generic;

namespace Common.Models
{
    public class FolderInfo
    {
        public FolderInfo()
        {
            Path = string.Empty;
            Exts = new List<string>();
            ExcludedPaths = new List<string>();
            Files = new List<FileDetails>();
            SeekStrings = new Dictionary<string, int>();
        }
        public int Order { get; set; }
        public string Path { get; set; }
        public IEnumerable<string> Exts { get; set; }
        public IEnumerable<string> ExcludedPaths { get; set; }
        public IDictionary<string,int> SeekStrings { get; set; }
        public IEnumerable<FileDetails> Files { get; set; }
        public bool Recursive { get; set; }
    }
}
