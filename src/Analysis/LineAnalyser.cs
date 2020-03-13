using Common.Models;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Analysis
{
    class LineAnalyser
    {
        public static void Analyse(FolderInfo folderInfo, Options options, int folderIndex, string filename, string filenameExt, string[] sourceLines, string line, int lineNo)
        {
            var result = new List<FoundLine>();
            if (line.Contains("class"))
            {
                var x = 1;
            }
        }
    }
}