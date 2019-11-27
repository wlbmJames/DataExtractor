using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DataAnalyzer.SearchRules
{
    public class SearchResult: ICloneable
    {
        public SearchResult(int PageNumber = -1): this("", new Rect(), false)
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
        public string Text { get; private set; }
        public Rect Area { get; private set; }
        public bool IsFound { get; set; }
        public int PageNumber { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
