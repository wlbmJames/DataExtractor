using DataAnalyzer.SearchRules;
using System;

namespace DataAnalyzer
{
    [Serializable]
    public class NamedResult
    {
        public string Title { get; set; }
        public SearchResult Result { get; set; }
    }
}
