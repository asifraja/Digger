using Digger.Common.Interfaces;
using Digger.Common.Models;
using Digger.Common.Options;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Digger.Search.Output
{

    public class HtmlOutput : SearchProcess, IProcess
    {
        private string _headerTemplatePath = "";
        private string _contentTemplatePath = "";
        private string _lineHeadTemplatePath = "";
        private string _lineTemplatePath = "";
        private string _lineFootTemplatePath = "";
        private string _footerTemplatePath = "";
        private string _title = "";

        private string _headerTemplate = "{{header}}";
        private string _contentTemplate = "{{content}}";
        private string _lineHeadTemplate = "<b>";
        private string _lineFootTemplate = "</b>";
        private string _lineTemplate = "<p><b>[{{lineno}}] [{{filename}}]</b></p><p>{{line}}</p>";
        private string _footerTemplate = "{{footer}}";

        private readonly StringBuilder _content = new StringBuilder();
        private readonly StringBuilder _header = new StringBuilder();
        private readonly StringBuilder _footer = new StringBuilder();

        private string HtmlTemplateDirectory(string fileName)
        {
            const string templateFolder = @"Search\Output\HtmlTemplates";
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.Combine(Path.GetDirectoryName(path), templateFolder, fileName);
        }

        private string Content
        {
            get
            {
                return (_headerTemplate.Replace("{{header}}", _header.ToString())
                    + _contentTemplate.Replace("{{content}}", _content.ToString())
                    + _footerTemplate.Replace("{{footer}}", _footer.ToString())).Replace("{{title}}", _title);
            }
        }

        public HtmlOutput(SearchOptions options, IOrderedEnumerable<IGrouping<string, FoundLine>> foundlines, CommandStats stats) : base (options,foundlines,stats)
        {
            _headerTemplatePath = HtmlTemplateDirectory("header.html");
            _contentTemplatePath = HtmlTemplateDirectory("content.html");
            _lineHeadTemplatePath = HtmlTemplateDirectory("line-head.html");
            _lineTemplatePath = HtmlTemplateDirectory("line.html");
            _lineFootTemplatePath = HtmlTemplateDirectory("line-foot.html");
            _footerTemplatePath = HtmlTemplateDirectory("footer.html");

            if (File.Exists(_headerTemplatePath)) _headerTemplate = File.ReadAllText(_headerTemplatePath);
            if (File.Exists(_contentTemplatePath)) _contentTemplate = File.ReadAllText(_contentTemplatePath);
            if (File.Exists(_lineHeadTemplatePath)) _lineHeadTemplate = File.ReadAllText(_lineHeadTemplatePath);
            if (File.Exists(_lineTemplatePath)) _lineTemplate = File.ReadAllText(_lineTemplatePath);
            if (File.Exists(_lineFootTemplatePath)) _lineFootTemplate = File.ReadAllText(_lineFootTemplatePath);
            if (File.Exists(_footerTemplatePath)) _footerTemplate = File.ReadAllText(_footerTemplatePath);
        }     

        public void Execute()
        {
            _title = Options.Output + " : " + DateTime.Now.ToLocalTime();
            _header.AppendLine("Digger CLI - &copy; 2019-" + (DateTime.Now.Year + 1));

            foreach (var group in FoundLineCollection.OrderBy(o => o.Key))
            {
                _content.AppendLine(_lineHeadTemplate.Replace("{{line-head}}", group.Key).Replace("{{line-count}}", group.Count().ToString()));
                Stats.TotalInstances += group.Count();
                if (Options.Verbose)
                {
                    var sameLine = true;
                    var prevLine = string.Empty;
                    foreach (var foundFile in group.OrderBy(o=>o.Filename).ThenBy(x=>x.LineNo))
                    {
                        var line = _lineTemplate.Replace("{{lineno}}", foundFile.LineNo.ToString())
                            .Replace("{{filename}}", foundFile.Filename)
                            .Replace("{{filenameExtPill}}", Ext2ColorCode(foundFile.Filename, foundFile.FilenameExt))
                            .Replace("{{filenameExt}}", foundFile.FilenameExt)
                            .Replace("{{line}}", WebUtility.HtmlEncode(foundFile.Line.Substring(0, Math.Min(foundFile.Line.Length, 255) - 1).Trim()) + (foundFile.Line.Length > 254 ? "<b>...</b>" : ""));
                        if (string.IsNullOrEmpty(prevLine)) prevLine = foundFile.Line;
                        if (prevLine.Trim().Replace(" ", "").Replace("\t", "") != foundFile.Line.Trim().Replace(" ", "").Replace("\t", "")) sameLine = false;
                        _content.AppendLine(line);
                    }
                    if(!sameLine)  _content.AppendLine("<span class=\"badge badge-warning\">DIFFERENT</span>");
                }
            }
            _footer.AppendLine("Digger CLI by Asif Raja");
            
            File.WriteAllText(Options.Output, Content);
        }

        private string Ext2ColorCode(string filename, string ext)
        {
            switch (ext)
            {
                case ".sql": return "secondary";
                case ".cs": return "success";
                case ".cshtml": return "success";
                case ".resx": return "danger";
                case ".js": return "warning";
                case ".html": return "warning";
                case ".css": return "warning";
                default: return "primary";
            }
        }
    }
}
