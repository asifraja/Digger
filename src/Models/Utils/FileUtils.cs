using Common.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Common.Utils
{
    public static class FileUtils
    {
        private static void GetFiles(FolderInfo folderInfo)
        {
            var searchOption = folderInfo.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var subFolders = Directory.EnumerateDirectories(folderInfo.Path, "*.*",  SearchOption.TopDirectoryOnly).ToList();
            var filesInSubfolders = subFolders.AsParallel().SelectMany(folder=>folderInfo.Exts.AsParallel().SelectMany(searchPattern => Directory.EnumerateFiles(folder, searchPattern, searchOption))).ToList();
            var filesInTopFolder = folderInfo.Exts.AsParallel().SelectMany(searchPattern => Directory.EnumerateFiles(folderInfo.Path, searchPattern, SearchOption.TopDirectoryOnly)).ToList();
            filesInTopFolder.ForEach(t=>filesInSubfolders.Add(t));
            //var files = folderInfo.Exts.AsParallel().SelectMany(searchPattern => Directory.EnumerateFiles(folderInfo.Path, searchPattern, searchOption));
            folderInfo.Files = filesInSubfolders.Where(f => folderInfo.ExcludedPaths.All(e => !@f.Contains(@e, StringComparison.OrdinalIgnoreCase))).ToList();
           
        }

        private static FolderInfo GetFolderInfo(int order, string path, IEnumerable<string> exts, IEnumerable<string> excludedFolders, bool recursive)
        {
            return new FolderInfo
            {
                Order = order,
                Path = path,
                Exts = exts,
                ExcludedPaths = excludedFolders,
                Recursive = recursive,
                Files = new List<string>()
            };
        }

        public static BlockingCollection<FolderInfo> GetFolderInfos(IEnumerable<string> foldersPath, IEnumerable<string> exts, IEnumerable<string> excludedFolders, bool recursive)
        {
            var infos  = new BlockingCollection<FolderInfo>();
            for (var order = 0; order < foldersPath.Count(); order++)
            {
                infos.Add(FileUtils.GetFolderInfo(order, foldersPath.ElementAt(order), exts, excludedFolders, recursive));
            }
            infos.AsParallel().ForAll(folerInfo => FileUtils.GetFiles(folerInfo));
            return infos;
        }
    }
}
