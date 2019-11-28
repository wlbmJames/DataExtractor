using System;
using DataAnalyzer.SearchRules;

namespace DataAnalyzer
{
    [Serializable]
    public class NamedResult
    {
        public string Title { get; set; }
        public SearchResult Result { get; set; }
    }
}
