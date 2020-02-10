using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace ATAPY.Document.Data.Core
{
    public class TextLines : List<TextLine>
    {
        #region Declaration Area
        internal const double MIN_GAPE_WIDTH = 10.0;
        internal const double ONE_LINE_MIN_INTERSECT_RATIO = 0.7;
        #endregion

        #region Constructors
        internal TextLines(Page Parent)
        {
            this.Parent = Parent;
        }
        #endregion

        #region Public Area

        #region Properties
        public Rect Bound
        {
            get
            {
                if (this.Count < 1) {
                    return Rect.Empty;
                }
                Rect Ret = this.First().Bound;
                for (int i = 1; i < this.Count; i++) {
                    Ret.Union(this[i].Bound);
                }
                return Ret;
            }
        }
        public string Text
        {
            get
            {
                if (this.Count < 1) {
                    return string.Empty;

                }
                string[] Ret = new string[this.Count];
                for (int i = 0; i < this.Count; i++) {
                    Ret[i] = this[i].Text;
                }
                return string.Join("\r\n", Ret);
            }
        }
        public Page Parent
        {
            get;
            internal set;
        }
        #endregion

        #region Methods
        public Word GetWordAtPoint(Point Pt)
        {
            string PointSymbol;
            return GetWordAtPoint(Pt, out PointSymbol);
        }
        public Word GetWordAtPoint(Point Pt, out string PointSymbol)
        {
            PointSymbol = string.Empty;
            var Line = GetTextLineAtPoint(Pt);
            if (Line == null) {
                return null;
            }
            return Line.GetWordAtPoint(Pt, out PointSymbol);
        }
        public TextLine GetTextLineAtPoint(Point Pt)
        {
            var LinesFound = from Line in this where Line.Bound.Contains(Pt) select Line;
            if (LinesFound.Count() < 1) {
                return null;
            }
            if (LinesFound.Count() > 1) {
                throw new Exception("Overlapped text lines");
            }
            return LinesFound.First();
        }
        public TextLine GetTextLine(Word Word)
        {
            var LinesFound = from Line in this where Line.Contains(Word) select Line;
            if (LinesFound.Count() < 1) {
                return null;
            }
            if (LinesFound.Count() > 1) {
                throw new Exception("Overlapped text lines");
            }
            return LinesFound.First();
        }
        public TextLines GetLinesInArea(Rect SearchArea)
        {
            if (this.Count < 1) {
                return null;
            }
            TextLines Ret = new TextLines(this.Parent);
            foreach (var Line in this) {
                var WordsInLine = (from Word in Line where Word.Bound.IntersectsWith(SearchArea) && Word.Bound.IntersectRatio(SearchArea) > ONE_LINE_MIN_INTERSECT_RATIO orderby Word.Bound.Left select Word).ToList();
                if (WordsInLine.Count > 0) {
                    TextLine NewLine = new TextLine(this);
                    NewLine.AddRange(WordsInLine);
                    Ret.Add(NewLine);
                }
            }
            return Ret;
        }
        public TextLines GetLinesIntersectingArea(Rect SearchArea)
        {
            if (this.Count < 1) {
                return null;
            }
            TextLines Ret = new TextLines(this.Parent);
            Ret.AddRange(from Line in this where Line.Bound.IntersectsWith(SearchArea) && Line.Bound.IntersectRatio(SearchArea) > ONE_LINE_MIN_INTERSECT_RATIO orderby Line.Bound.Top select Line);
            return Ret;
        }
        public TextLines GetLinesTouchesArea(Rect SearchArea)
        {
            if (this.Count < 1) {
                return null;
            }
            TextLines Ret = new TextLines(this.Parent);
            Ret.AddRange(from Line in this where Line.Bound.IntersectsWith(SearchArea) orderby Line.Bound.Top select Line);
            return Ret;
        }
        public string GetTextInArea(Rect SearchArea)
        {
            if (this.Count < 1) {
                return string.Empty;
            }
            return GetLinesInArea(SearchArea).Text;
        }
        public List<Rect> GetCommonWhiteGapes()
        {
            if (this.Count < 1) {
                return null;
            }
            List<Rect> Ret = this[0].WhiteGapes;
            for (int i = 1; i < this.Count; i++) {
                Ret = GetIntersections(Ret, this[i].WhiteGapes);
            }
            return Ret;
        }
        public List<Words> GetWordsMatchingPattern(string SearchPattern)
        {
            List<Words> Ret = new List<Words>();
            foreach (var Line in this) {
                var Match = System.Text.RegularExpressions.Regex.Match(Line.NoSpaceTextForRegEx, SearchPattern, System.Text.RegularExpressions.RegexOptions.ExplicitCapture | System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace);
                while (Match.Success) {
                    string FoundString = Match.Value;
                    var FoundWords = Line.GetWordsMatchingString(FoundString);
                    Ret.Add(FoundWords);
                    Match = Match.NextMatch();
                }
            }
            return Ret;
        }
        public List<Words> GetWordsMatchingPatternExact(string SearchPattern, bool AllowMultiline = false, List<string> BlackList = null)
        {
            List<Words> Ret = new List<Words>();
            foreach (var Line in this) {
                var Match = System.Text.RegularExpressions.Regex.Match(Line.Text, SearchPattern);
                int StartIndex = 0;
                while (Match.Success) {
                    string FoundString = Match.Value.Trim();
                    bool Add = true;
                    var FoundWords = Line.GetWordsMatchingStringExact(FoundString, StartIndex);
                    if (BlackList != null && BlackList.Count > 0) {
                        foreach (var Black in BlackList) {
                            foreach (var Word in FoundWords) {
                                if (Word.Text.Contains(Black)) {
                                    Add = false;
                                    break;
                                }
                            }
                            if (!Add) {
                                break;
                            }
                        }
                    }
                    if (Add) {
                        TextLine TempLine = new TextLine(this);
                        TempLine.AddRange(FoundWords);
                        if (TempLine.Text.IndexOf(FoundString) > 0) {
                            FoundWords.StartOffset = TempLine.Text.IndexOf(FoundString);
                            FoundWords.EndOffset = TempLine.Text.Length - (FoundWords.StartOffset + FoundString.Length);
                        }
                        Ret.Add(FoundWords);
                    }
                    Match = Match.NextMatch();
                    StartIndex = Line.Text.IndexOf(FoundString, StartIndex) + FoundString.Length + 1;
                }
            }
            if (AllowMultiline) {
                for (int i = 0; i < this.Count - 1; i++) {
                    string UnionText = this[i].Text + this[i + 1].Text;
                    var Match = System.Text.RegularExpressions.Regex.Match(UnionText, SearchPattern);
                    int LastFoundIndex = 0;
                    int LastFoundLength = 0;
                    while (Match.Success) {
                        string FoundString = Match.Value;
                        var FoundWordsLine1 = this[i].GetWordsMatchingStringExact(FoundString);
                        var FoundWordsLine2 = this[i + 1].GetWordsMatchingStringExact(FoundString);
                        /*if (FoundWordsLine1 != null) {
                            Ret.Add(FoundWordsLine1);
                            LastFoundIndex = this[i].Text.LastIndexOf(FoundString);
                            LastFoundLength = FoundString.Length;
                        }
                        if (FoundWordsLine2 != null) {
                            Ret.Add(FoundWordsLine2);
                        }*/
                        if (FoundWordsLine1 == null && FoundWordsLine2 == null) {
                            //matched on line brake
                            int MergeStartIndex = UnionText.IndexOf(FoundString, LastFoundIndex + LastFoundLength);
                            var SubstringLine1 = FoundString.Substring(0, this[i].Text.Length - MergeStartIndex);
                            var SubstringLine2 = FoundString.Substring(SubstringLine1.Length);
                            var SplitWordsLine1 = this[i].GetWordsMatchingStringExact(SubstringLine1);
                            var SplitWordsLine2 = this[i + 1].GetWordsMatchingStringExact(SubstringLine2);
                            Ret.Add(SplitWordsLine1);
                            Ret.Add(SplitWordsLine2);
                        }
                        Match = Match.NextMatch();
                    }
                }
            }
            return Ret;
        }
        public List<Words> GetWordsMatchingPattern(Rect Area, string SearchPattern)
        {
            List<Words> Ret = new List<Words>();
            foreach (var Line in this) {
                if (!Line.Bound.IntersectsWith(Area)) {
                    continue;
                }
                var Match = System.Text.RegularExpressions.Regex.Match(Line.NoSpaceTextForRegEx, SearchPattern, System.Text.RegularExpressions.RegexOptions.ExplicitCapture | System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace);
                while (Match.Success) {
                    string FoundString = Match.Value;
                    var FoundWords = Line.GetWordsMatchingString(FoundString);
                    Ret.Add(FoundWords);
                    Match = Match.NextMatch();
                }
            }
            return Ret;
        }
        #endregion

        #endregion

        #region Private Area
        private List<Rect> GetIntersections(List<Rect> Line1Gapes, List<Rect> Line2Gapes, double MinGapeWidth = MIN_GAPE_WIDTH)
        {
            List<Rect> Ret = new List<Rect>();
            for (int i = 0; i < Line1Gapes.Count; i++) {
                var Line = Line1Gapes[i];
                for (int j = 0; j < Line2Gapes.Count; j++) {
                    if (Line.IntersectsWith(Line2Gapes[j])) {
                        var NewLine = Line;
                        NewLine.Intersect(Line2Gapes[j]);
                        if (NewLine.Width < MinGapeWidth) {
                            continue;
                        }
                        Ret.Add(NewLine);
                        //continue;
                    }
                }
            }
            return Ret;
        }
        #endregion
    }
}
