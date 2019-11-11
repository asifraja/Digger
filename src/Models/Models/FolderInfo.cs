using System.Collections.Generic;

namespace Common.Models
{
    public class FolderInfo
    {
        public FolderInfo()
        {
            Path  = string.Empty;
            FoundLines = new List<FoundLine>();
            Exts = new List<string>();
            ExcludedPaths = new List<string>();
            Files = new List<string>();
        }
        public int Order { get; set; }
        public string Path { get; set; }
        public IEnumerable<string> Exts { get; set; }
        public IEnumerable<string> ExcludedPaths { get; set; }
        public IEnumerable<string> Files { get; set; }
        public IEnumerable<FoundLine> FoundLines { get; set; }
        public bool Recursive { get; set; }
    }
}
