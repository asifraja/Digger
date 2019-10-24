namespace Digger
{
    public class Processor
    {
        //public int TotalFiles { get; private set; }
        //public int FoundFiles { get; private set; }
        //public int TotalInstances { get; private set; }
        //public IEnumerable<string> GetFiles(GenericOptions options)
        //{
        //    var searchOption = options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        //    var files = options.Folders.AsParallel().SelectMany(path => options.Exts.AsParallel().SelectMany(searchPattern => Directory.EnumerateFiles(path, searchPattern, searchOption)));
        //    return files.Where(f => options.ExcludeFolders.All(e => !@f.Contains(@e, StringComparison.OrdinalIgnoreCase))).ToArray();
        //}

        //public void SearchInFiles(SearchOptions options)
        //{
        //    var foundLines = new BlockingCollection<FoundLine>();
        //    var files = GetFiles(options).ToArray();
        //    TotalFiles = files.Count();
        //    var stage1 = Task.Run(() =>
        //    {
        //        try
        //        {
        //            if (options.Interactive)
        //            {
        //                Console.WriteLine($"Are you sure you want to search through {TotalFiles} many files.");
        //                Console.WriteLine($"Press Return key to continue or CTRL+C to abort.");
        //                Console.ReadLine();
        //            }
        //            Parallel.For(0, files.Count(), fileIndex =>
        //            {                      
        //                {
        //                    var sourceLines = File.ReadAllLines(files[fileIndex]);
        //                    string filenameExt = Path.GetExtension(files[fileIndex]).ToLower();
        //                    var lastLines = new FixedSizedQueue<string>(3);
        //                    FoundFiles++;
        //                    for ( var lineNo = 0; lineNo < sourceLines.Length; lineNo++)
        //                    {
        //                        var line = sourceLines[lineNo];
        //                        lastLines.Enqueue(line);
        //                        var lines = options.CaseSensitive ? ExactFilter.Match(options, files[fileIndex], filenameExt, sourceLines, line, lineNo) : ContainsFilter.Match(options, files[fileIndex], filenameExt, sourceLines, line, lineNo);
        //                        if (lines.Count() > 0)
        //                        {
        //                            foreach (var processedLine in lines)
        //                            {
        //                                foundLines.Add(processedLine);
        //                            }
        //                        }
        //                    }
        //                }
        //            });
        //        }
        //        finally
        //        {
        //            foundLines.CompleteAdding();
        //        }
        //    });
        //    var stage2 = Task.Run(() =>
        //    {
        //        var result = foundLines.GetConsumingEnumerable().ToList();

        //        var searchStringsLinq =
        //            from line in result
        //            group line by line.SearchString into searchString
        //            orderby searchString.Key
        //            select searchString;

        //        if (!string.IsNullOrEmpty(options.Export) && options.Export.Contains(".html", StringComparison.OrdinalIgnoreCase))
        //        {
        //            var html = new Html(options, searchStringsLinq);
        //            html.Render();
        //            TotalInstances = html.FoundInstances;
        //            File.WriteAllText(options.Export, html.Body);
        //        }
        //        else
        //        {
        //            foreach (var searchGroup in searchStringsLinq)
        //            {
        //                var title = $"Searched: {searchGroup.Key} found ({searchGroup.Count()}) instances.";
        //                Console.WriteLine(new String('-', title.Length));
        //                Console.ForegroundColor = ConsoleColor.Yellow;
        //                Console.WriteLine(title);
        //                Console.WriteLine(new String('-', title.Length));
        //                TotalInstances += searchGroup.Count();
        //                if (options.Verbose)
        //                {
        //                    foreach (var foundFile in searchGroup.OrderBy(f=> f.Filename).ThenBy(f=>f.LineNo))
        //                    {
        //                        var line = foundFile.Line.Substring(1, Math.Min(foundFile.Line.Length, 1024) - 1).TrimStart() + (foundFile.Line.Length > 1023 ? "<b>...</b>" : "");
        //                        Console.ForegroundColor = ConsoleColor.DarkYellow;
        //                        Console.WriteLine($"[{foundFile.LineNo}] [{foundFile.Filename}]");
        //                        Console.ForegroundColor = ConsoleColor.Green;
        //                        Console.WriteLine($"{line}");
        //                    }
        //                }
        //            }
        //        }
                
        //        var fileQuery =
        //            from line in result
        //            group line by line.Filename into file
        //            orderby file.Key
        //            select file;
        //        FoundFiles = fileQuery.Count();
                
        //        if (options.DeleteLine)
        //        {
        //            foreach (var file in fileQuery.AsParallel())
        //            {
        //                var sourceLines = File.ReadAllLines(file.Key);
        //                File.WriteAllLines(@"D:\Temp\" + Path.GetFileName(file.Key), sourceLines);
        //                foreach (var foundFile in file.OrderBy(f => f.Filename).ThenBy(f => f.LineNo))
        //                {
        //                    if(options.DeleteLine)  sourceLines[foundFile.LineNo-1] = "";
        //                }
        //                File.WriteAllLines(@"D:\Temp\"+Path.GetFileName(file.Key) + ".[NEW]",sourceLines);
        //            }
        //        }
        //    });
        //    Task.WhenAll(stage1, stage2).GetAwaiter().GetResult();
        //    Console.ForegroundColor = ConsoleColor.White;
        //}
    }
}
