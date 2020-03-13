using System;
using System.Collections.Generic;
using System.Text;

namespace Analysis
{
    public class CsFileAnalyser
    {
        readonly string[] _lines;
        readonly string _filePath;

        public CsFileAnalyser(string[] lines, string filePath)
        {
            _lines = lines;
            _filePath = filePath;
        }
    }
}
