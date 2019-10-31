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
                var foundFolder = new int[Options.Folders.Count()];
                if (Options.Verbose)
                {
                    var sameLine = true;
                    var prevLine = string.Empty;
                    var keyContent = new StringBuilder();
                    foreach (var foundFile in group.OrderBy(o=>o.FolderIndex).ThenBy(x=>x.Filename).ThenBy(x => x.LineNo))
                    {
                        foundFolder[foundFile.FolderIndex] = 1;
                        var line = _lineTemplate.Replace("{{lineno}}", foundFile.LineNo.ToString())
                            .Replace("{{filename}}", foundFile.Filename)
                            .Replace("{{filenameExtPill}}", Ext2ColorCode(foundFile.Filename, foundFile.FilenameExt))
                            .Replace("{{filenameExt}}", foundFile.FilenameExt)
                            .Replace("{{prevline}}", WebUtility.HtmlEncode(WebUtility.HtmlDecode(foundFile.PreviousLine.Substring(0, Math.Min(Options.BufferSize, foundFile.PreviousLine.Length)))))
                            .Replace("{{line}}", WebUtility.HtmlEncode(WebUtility.HtmlDecode(foundFile.Line.Substring(0,Math.Min(Options.BufferSize, foundFile.Line.Length)))));
                        if (string.IsNullOrEmpty(prevLine)) prevLine = foundFile.Line;
                        if (prevLine.Trim().Replace(" ", "").Replace("\t", "") != foundFile.Line.Trim().Replace(" ", "").Replace("\t", "")) sameLine = false;
                        keyContent.AppendLine(line);
                    }
                    //for (var n =0; n<foundFolder.Length;n++)
                    //{
                    //    if (foundFolder[n] == 0)
                    //    {
                    //        _content.AppendLine($"<div class='alert alert-warning'><strong>Warning!</strong> {group.Key} is missing in {Options.Folders.ToArray()[n]}, please validate the entry.</div>");
                    //    }
                    //}
                    if (!sameLine)
                    {
                        _content.AppendLine($"<div class='alert alert-danger'><strong>Note!</strong> {group.Key} has different values, please review.</div>");
                    }
                    _content.AppendLine(keyContent.ToString());
                }
            }
            _footer.AppendLine("Digger CLI : Authored by Asif Raja");
            
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
