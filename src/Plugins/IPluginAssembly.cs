using Common.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Plugins
{
    public interface IPluginAssembly
    {
        string Name { get; }
        string Verb { get; }
        string Description { get; }
        BlockingCollection<FolderInfo> FolderInfos { get;  }
        long CompletedInMilliseconds { get; }
        IEnumerable<FolderInfo> Execute(string[] args);
    }
}