using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Digger.Common.Utils
{
    public static class StringExtensions
    {
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
    }
}

