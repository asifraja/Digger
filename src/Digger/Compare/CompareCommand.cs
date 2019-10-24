using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Digger.Common.Interfaces;
using Digger.Common.Models;
using Digger.Common.Utils;
using Digger.Common.Commands;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digger.Compare
{
    public class CompareCommand : BaseCommand, ICommand
    {
        protected BlockingCollection<EjDiffModel> _models = new BlockingCollection<EjDiffModel>();

        public string[] WithFiles { get; }
        public CompareCommand(CompareOptions options) : base(options)
        {
            WithFiles = GetFiles(new[] { options.WithFolder }, options.ExcludeFolders, options.Exts, options.Recursive).ToArray();
        }

        public void Execute()
        {
            var options = Options as CompareOptions;
            try
            {
                Parallel.For(0, Stats.TotalFiles, fileIndex =>
                {
                    {
                        var fileName1 = Files[fileIndex];
                        var withFilename = options.WithFolder + fileName1.TrailFilePath(Options.Folders.FirstOrDefault());
                        string filenameExt = Path.GetExtension(fileName1).ToLower();
                        if (File.Exists(fileName1) && File.Exists(withFilename))
                        {
                            var content1 = File.ReadAllText(fileName1);
                            var content2 = File.ReadAllText(withFilename);
                            if (options.Inline)
                            {
                                _models.Add(new EjDiffModel(new InlineDiffBuilder(new Differ()).BuildDiffModel(content1, content2), fileName1, withFilename));
                            }
                            else
                            {
                                _models.Add(new EjDiffModel(new SideBySideDiffBuilder(new Differ()).BuildDiffModel(content1, content2), fileName1, withFilename));
                            }
                            Stats.TotalInstances++;
                            Stats.FoundFiles++;
                        }
                    }
                });
            }
            finally
            {
                _models.CompleteAdding();
            }
            if (Stats.TotalFiles == 0) return;
            Process();
        }
        private void Process()
        {
            var options = Options as CompareOptions;
            var output = new StringBuilder();
            output.Append("<html><style>.lineno {color:blue;padding:5px;margin-right:5px;} .filename {color:green;} .deleted{color:red;background-color:blue;} .modified{color:red;background-color:blue;} .inserted {color:green;background-color:yellow;} .unchanged{color:black;background-color:lightgrey;} .container {width: 98%;margin: auto;padding: 10px;} .file1 {width: 50%;background: lightgrey;float: left;} .file2 {margin-left: 5%;background: lightgrey;}</style><body>");
            output.Append("<section class=\"container\">");
            if (options.IncludeMissing)
            {
                foreach (var file in Files)
                {
                    var f1 = options.WithFolder + file.TrailFilePath(options.Folders.FirstOrDefault());
                    if (File.Exists(f1)) continue;
                    output.Append(BuildDiffPanel("file1", file, null));
                    output.Append(BuildDiffPanel("file2", "MISSING", null));
                    Stats.TotalInstances++;
                }
                output.Append($"<hr/>");
                foreach (var file in WithFiles)
                {
                    var f1 = options.Folders.FirstOrDefault() + file.TrailFilePath(options.WithFolder);
                    if (File.Exists(f1)) continue;
                    output.Append(BuildDiffPanel("file1", "MISSING", null));
                    output.Append(BuildDiffPanel("file2", file, null));
                    Stats.TotalInstances++;
                }
                output.Append($"<hr/>");
            }
            var models = _models.Where(m => m.HasDifference);
            if (options.Inline)
            {
                foreach (var model in models)
                {
                    output.Append(BuildInlinePanel("inline", model));
                    if (_models.Count > 1) output.Append($"<hr/>");
                }
            }
            else
            {
                foreach (var model in models)
                {
                    output.Append(BuildDiffPanel("file1", model.ThisFilename, model.SideBySideDiffModel.OldText));
                    output.Append(BuildDiffPanel("file2", model.WithFilename, model.SideBySideDiffModel.NewText));
                    if (_models.Count > 1) output.Append($"<hr/>");
                }
            }
            output.Append("</section");
            output.Append("</body></html>");
            if (!string.IsNullOrEmpty(options.Output))
            {
                File.WriteAllText(options.Output, output.ToString());
            }
        }

        private string BuildInlinePanel(string panelName, EjDiffModel model)
        {
            var options = Options as CompareOptions;
            var output = new StringBuilder($"<div class='{panelName}'>");
            //output.Append($"<div>");
            output.Append($"<span class='filename'>{model.ThisFilename}</span><br/>");
            output.Append($"<span class='filename'>{model.WithFilename}</span><br/>");
            foreach (var line in model.DiffPaneModel.Lines)
            {
                if (line.Type == ChangeType.Inserted)
                {
                    output.Append($"<span class='lineno'>{line.Position}</span>");
                    output.Append("<span class='inserted'>" + System.Net.WebUtility.HtmlEncode(line.Text) + "</span>");
                    output.Append("<br/>");
                }
                else if (line.Type == ChangeType.Deleted)
                {
                    output.Append($"<span class='lineno'>{line.Position}</span>");
                    output.Append("<span class='deleted'>" + System.Net.WebUtility.HtmlEncode(line.Text) + "</span>");
                    output.Append("<br/>");
                }
                else if (options.Verbose)
                {
                    output.Append($"<span class='lineno'>{line.Position}</span>");
                    output.Append("<span class='unchanged'>" + System.Net.WebUtility.HtmlEncode(line.Text) + "</span>");
                    output.Append("<br/>");
                }
            }
            output.Append($"</div>");
            if (_models.Count > 1) output.Append($"<hr/>");
            return output.ToString().Replace(Environment.NewLine, "<br/>");
        }

        private string BuildDiffPanel(string panelName, string filename, DiffPaneModel model)
        {
            var options = Options as CompareOptions;
            var output = new StringBuilder($"<div class='{panelName}'>");
            output.Append($"<span class='filename'>{filename}</span><br/><br/>");
            if (model != null)
            {
                foreach (var line in model.Lines)
                {
                    var lineShown = false;
                    if (line.Type == ChangeType.Deleted || line.Type == ChangeType.Inserted || line.Type == ChangeType.Unchanged && options.Verbose)
                    {
                        output.Append($"<span class='lineno'>{line.Position}</span>");
                        output.Append("<span class='" + line.Type.ToString() + "'>" + System.Net.WebUtility.HtmlEncode(line.Text) + "</span>");
                        lineShown = true;
                    }
                    else if (line.Type == ChangeType.Modified)
                    {
                        output.Append($"<span class='lineno'>{line.Position}</span>");
                        lineShown = true;
                        foreach (var character in line.SubPieces)
                        {
                            if (character.Type == ChangeType.Imaginary) { continue; }
                            output.Append("<span class='" + character.Type.ToString() + "'>" + System.Net.WebUtility.HtmlEncode(character.Text) + "</span>");
                        }
                    }
                    if (lineShown) output.Append("<br/>");
                }
            }
            output.Append("</div>");
            return output.ToString().Replace(Environment.NewLine, "<br/>");
        }
    }
}

