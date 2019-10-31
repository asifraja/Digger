using System;
using System.Collections.Generic;

namespace Digger.Common.Models
{
    public class FoundLine
    {
        public string PreviousLine { get; set; }
        public string Line { get; }
        public string SeekString { get; }
        public string Filename { get; }
        public string FilenameExt { get; }
        public int LineNo { get; }
        public int FolderIndex { get; }
        public bool LineIsUpdated { get { return !string.IsNullOrEmpty(PreviousLine) && !string.IsNullOrEmpty(Line) && PreviousLine != Line; } }

        public FoundLine(string filename, string filenameExt, string line, string previousLine, int lineNo, string seekString, int folderIndex)
        {
            FilenameExt = filenameExt;
            Filename = filename;
            Line = line;
            LineNo = lineNo;
            SeekString = seekString;
            PreviousLine = previousLine;
        }
    }
}
