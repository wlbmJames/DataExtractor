using ATAPY.Document.Data.Core;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DataAnalyzer.Extensions
{
    public static class AtapyPageExtensions
    {
        public static List<Words> FindWords(this Page page, Rect Area, string SearchPattern)
        {
            //var lines = page.TextLines.Where(l => Area.Contains(l.Bound)).ToList();
            var lines = page.TextLines.Where(l => l.Bound.IntersectsWith(Area)).ToList();
            List<Words> result = new List<Words>();
            foreach (var line in lines)
            {
                List<Word> wordsInArea = line.Where(w => Area.Contains(w.Bound)).ToList();
                //List<Word> wordsInArea = line.Where(w => w.Bound.IntersectsWith(Area)).ToList();
                if (wordsInArea.Count < 1)
                    continue;
                wordsInArea.Sort((x, y) => x.Bound.Left.CompareTo(y.Bound.Left));
                var strLine = string.Concat(wordsInArea).Replace("*", @"\*");
                var Match = System.Text.RegularExpressions.Regex.Match(strLine, SearchPattern, System.Text.RegularExpressions.RegexOptions.ExplicitCapture | System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace);
                while (Match.Success)
                {
                    string FoundString = Match.Value;
                    var FoundWords = GetWordsMatchingString(wordsInArea, strLine, FoundString);
                    result.Add(FoundWords);
                    Match = Match.NextMatch();
                }

            }
            return result;
        }

        private static Words GetWordsMatchingString(List<Word> words, string fullText, string SearchString)
        {
            if (!fullText.Contains(SearchString))
            {
                return null;
            }
            int StartIndex = fullText.IndexOf(SearchString);
            int CheckedLength = 0;
            Words Ret = new Words();
            Ret.Tag = SearchString;
            for (int i = 0; i < words.Count; i++)
            {
                CheckedLength += words[i].Text.Length;
                if (CheckedLength > StartIndex)
                {
                    if (StartIndex + SearchString.Length > CheckedLength - words[i].Text.Length)
                    {
                        Ret.Add(words[i]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return Ret;
        }

    }
}
