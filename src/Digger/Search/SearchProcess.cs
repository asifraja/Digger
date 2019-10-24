using Digger.Common.Models;
using System.Linq;

namespace Digger.Search.Output
{
    public abstract class SearchProcess
    {
        protected CommandStats Stats { get; }
        protected readonly SearchOptions Options;
        protected readonly IOrderedEnumerable<IGrouping<string, FoundLine>> FoundLineCollection;
        private SearchProcess()
        {
        }

        public SearchProcess(SearchOptions options, IOrderedEnumerable<IGrouping<string, FoundLine>> foundlines, CommandStats stats)
        {
            Options = options;
            FoundLineCollection = foundlines;
            Stats = stats;
        }
    }
}
