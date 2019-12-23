using System;
using System.Windows;

namespace DataAnalyzer.SearchRules
{
    [Serializable]
    public class SearchResult : ICloneable
    {
        public SearchResult() : this(-1)
        {
        }
        public SearchResult(int PageNumber) : this("", new Rect(), false)
        {

        }
        public SearchResult(string text, Rect area, int pageNum = -1) : this(text, area, false, pageNum)
        {

        }
        public SearchResult(string text, Rect area, bool isFound, int pageNum = -1)
        {
            Text = text;
            Area = area;
            IsFound = isFound;
            PageNumber = pageNum;
        }
        public string Text { get; set; }
        public Rect Area { get; set; }
        public bool IsFound { get; set; }
        public int PageNumber { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
