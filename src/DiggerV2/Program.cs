using Plugins;
using System;
using System.Linq;

namespace Digger
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string[] pluginPaths = new string[]
                {
                    @"D:\Work\Digger\src\FindReplace\bin\Debug\netcoreapp3.0\FindReplace.Plugin.dll"
                };

                var plugins = pluginPaths.SelectMany(pluginPath => { return PluginLoader.CreateInstance(pluginPath); }).ToList();

                if (args.Length == 0)
                {
                    Prompt(ConsoleColor.White, "Digger CLI by Asif Raja", ConsoleColor.DarkCyan, " (https://github.com/asifraja)", true);
                    Console.WriteLine();
                    Prompt("Usage:", true);
                    foreach (IPluginAssembly command in plugins)
                    {
                        Prompt(ConsoleColor.White, "  digger");
                        Prompt($" {command.Verb}");
                        Prompt(ConsoleColor.DarkGreen, " [options]");
                        Prompt(ConsoleColor.DarkCyan, $" ## {command.Description}", true);
                    }
                    Prompt(ConsoleColor.White, "  digger");
                    Prompt(ConsoleColor.DarkGreen, " [path-to-options-file] |");
                    Prompt(ConsoleColor.DarkGreen, " [options]");
                    Prompt(ConsoleColor.DarkCyan, " ## provide options in a file", true);
                    Console.WriteLine();
                    Prompt(ConsoleColor.DarkGreen, "options:", true);
                    Prompt("  --help");
                    Prompt(ConsoleColor.DarkCyan, " ## List all available options", true);
                    Prompt("  --version", true);
                    Console.WriteLine();
                    Prompt(ConsoleColor.DarkGreen, "path-to-options-file:", true);
                    Prompt("  [<path to file containg option flags>]");
                }
                else
                {
                    var commandName = args[0].ToLower().Trim();
                    Console.WriteLine($"-- {commandName} --");
                    IPluginAssembly command = plugins.FirstOrDefault(c => c.Verb == commandName);
                    if (command == null)
                    {
                        Console.WriteLine($"{args[0]} is an unknown command.");
                        return;
                    }
                    var fi = command.Execute(args);
                    foreach (var f in fi.OrderBy(o => o.Order))
                    {
                        Prompt(ConsoleColor.Magenta, $"{f.Files.Count()} files in {f.Path}", true);
                        foreach (var str in f.SeekStrings)
                        {
                            Prompt(ConsoleColor.DarkMagenta, $"  {str.Value} instances of {str.Key} in {}", true);
                            foreach (var fl in f.Files.Where(f => f.FoundLines.Count() > 0  && f.FoundLines.Any(x=>x.SeekString==str.Key)).OrderBy(f => f.Path))
                            {
                                Prompt(ConsoleColor.DarkGreen, $"    {fl.Path}", true);
                                foreach (var l in fl.FoundLines.OrderBy(f => f.SeekedString).ThenBy(f => f.Filename).ThenBy(f => f.LineNo))
                                {
                                    Prompt(ConsoleColor.Green, $"     {l.LineNo} {l.Line.Trim()}", true);
                                }
                            }
                        }
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadKey();
        }

        private static void Prompt(string text, bool newLine = false)
        {
            Console.Write(text);
            if (newLine) Console.WriteLine();
        }

        private static void Prompt(ConsoleColor color, string text, bool newLine = false)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = oldColor;
            if (newLine) Console.WriteLine();
        }
        private static void Prompt(ConsoleColor color, string text, ConsoleColor color2, string text2, bool newLine = false)
        {
            Prompt(color, text, false);
            Prompt(color2, text2, newLine);
        }


        //static IEnumerable<IPlugedInAssembly> LoadPlugin(string relativePath)
        //{
        //    Navigate up to the solution root
        //    /*string root = Path.GetFullPath(Path.Combine(
        //        Path.GetDirectoryName(
        //            Path.GetDirectoryName(
        //                Path.GetDirectoryName(
        //                    Path.GetDirectoryName(
        //                        Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

        //    string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar))); */
        //    return PluginLoader.CreateInstance(relativePath);
        //}
    }
}
