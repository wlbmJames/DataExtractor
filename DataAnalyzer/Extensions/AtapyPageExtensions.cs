using ATAPY.Document.Data.Core;
using System.Windows;
using System.Collections.Generic;
using System.Linq;

namespace DataAnalyzer.Extensions
{
    public static class AtapyPageExtensions
    {
        public static List<Words> FindWords(this Page page, Rect Area, string SearchPattern)
        {
            var lines = page.TextLines.Where(l => l.Bound.IntersectsWith(Area)).ToList();
            List<Words> result = new List<Words>();
            foreach (var line in lines)
            {
                List<Word> wordsInArea = line.Where(w=> w.Bound.IntersectsWith(Area)).ToList();
                if (wordsInArea.Count < 1)
                    continue;
                wordsInArea.Sort((x, y) => x.Bound.Left.CompareTo(y.Bound.Left));
                var strLine = string.Concat(wordsInArea).Replace("*", @"\*");
                var Match = System.Text.RegularExpressions.Regex.Match(strLine, SearchPattern, System.Text.RegularExpressions.RegexOptions.ExplicitCapture | System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace);
                while (Match.Success)
                {
                    string FoundString = Match.Value;
                    var FoundWords = line.GetWordsMatchingString(FoundString);
                    result.Add(FoundWords);
                    Match = Match.NextMatch();
                }

            }
            return result;
        }
    }
}
