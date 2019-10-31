using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace Digger.Common.Utils
{
    public static class StringExtensions
    {
        public static int GetElementIndex(this string[] elements, string element)
        {
            for (var ndx = 0; ndx<elements.Count();ndx++)
            {
                if (element.StartsWith(elements[ndx])) return ndx;
            }
            return -1;
        }

        public static string TrailFilePath(this string filePath, string folder)
        {
            return filePath.Replace(folder, "");
        }

        public static string ToClean(this string source)
        {
            return source.Replace(@"\", "").ToLower();
        }
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        public static bool AllContains(this string source, IEnumerable<string> toCheck, StringComparison comp)
        {
            foreach(var tok  in toCheck)
            {
                if (!source.Contains(tok, comp)) return false;
            }
            return true;
        }

        public static bool AllContains(this string source, IEnumerable<string> toCheck)
        {
            foreach (var tok in toCheck)
            {
                if (!source.Contains(tok)) return false;
            }
            return true;
        }

        public static string FilePathAtAssemblyDirectory(this string foldername, string fileName)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.Combine(Path.GetDirectoryName(path), foldername, fileName);
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            if (index + length > data.Length)
            {
                length = data.Length - index;
            }
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static T[] SubArrayDeepClone<T>(this T[] data, int index, int length)
        {
            T[] arrCopy = new T[length];
            Array.Copy(data, index, arrCopy, 0, length);
            using (MemoryStream ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, arrCopy);
                ms.Position = 0;
                return (T[])bf.Deserialize(ms);
            }
        }

        public static string[] ExtractUsingTokens(this string text, IEnumerable<string> tokens)
        {
            var result = new List<string>();
            foreach (var token in tokens)
            {
                var toks = token.Split(new []{"***"}, StringSplitOptions.None);
                Regex regex = new Regex($"{Regex.Escape(toks[0])}(.*?){Regex.Escape(toks[1])}");
                var index = 0;
                while (true)
                {
                    var v = regex.Match(text.Substring(index + 1));
                    if (v.Success)
                    {
                        index= index + v.Index + v.Groups[1].ToString().Length;
                        if(!result.Any(x=>x== v.Groups[1].ToString().Trim()))
                        result.Add(v.Groups[1].ToString().Trim());
                        if (result.Count > 9999) throw new OverflowException("Too many entries found while extracting >= 10000");
                    }
                    else
                        break;
                }                
            }
            return result.ToArray();
        }
    }
}

