using Common.Models;
using System.Collections.Concurrent;

namespace Plugins
{
    public interface IPluginAssembly
    {
        string Name { get; }
        string Verb { get; }
        string Description { get; }
        BlockingCollection<FolderInfo> FolderInfos { get;  }
        long CompletedInMilliseconds { get; }
        long Execute(string[] args);
    }
}