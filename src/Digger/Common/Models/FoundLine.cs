using System;
using System.Collections.Generic;

namespace Digger.Common.Models
{
    public class FoundLine
    {
        public string Line { get; }
        public string SeekString { get; }
        public string Filename { get; }
        public string FilenameExt { get; }
        public int LineNo { get; }
        public int FodlerIndex { get; }
        public FoundLine(string filename, string filenameExt, string line, int lineNo, string seekString, int folderIndex)
        {
            FilenameExt = filenameExt;
            Filename = filename;
            Line = line;
            LineNo = lineNo;
            SeekString = seekString;
        }
    }
}
