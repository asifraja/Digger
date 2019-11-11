using System.Collections.Generic;

namespace Common.Models
{
    public class FileDetails
    {
        public FileDetails()
        {
            Path = string.Empty;
            FoundLines = new List<FoundLine>();
        }
        public string Path { get; set; }
        public IList<FoundLine> FoundLines { get; set; }
    }
}
