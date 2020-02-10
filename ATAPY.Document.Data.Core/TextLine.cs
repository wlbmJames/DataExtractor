using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace ATAPY.Document.Data.Core
{
    public enum E_TextLineType
    {
        Regular,
        TableHeader,
        TableStop,
        TableDrop
    }
    public class TextLine : Words
    {
        #region Declaration Area
        private const double SPACE_BORDER_RATIO = 0.7;
        private const double TAB_BORDER_RATIO = 2.2;
        #endregion

        #region Constructors
        public TextLine(TextLines Parent)
        {
            this.Parent = Parent;
            this.Type = E_TextLineType.Regular;
        }
        #endregion

        public string Text
        {
            get
            {
                return GetText();
            }
        }
        public string NoSpaceText
        {
            get
            {
                return GetTextWithNoSpaces();
            }
        }
        public string NoSpaceTextForRegEx
        {
            get
            {
                return this.NoSpaceText.Replace("*", @"\*");//.Replace("(", @"\(").Replace(")", @"\)");
            }
        }
        public List<string> NonSplitWords
        {
            get;
            set;
        }
        public TextLines Parent
        {
            get;
            internal set;
        }
        public E_TextLineType Type
        {
            get;
            set;
        }
        public List<Rect> WhiteGapes
        {
            get
            {
                if (this.Count < 1) {
                    return null;
                }
                this.SortWords();
                List<Rect> Ret = new List<Rect>();
                Ret.Add(new Rect(0, 0, this[0].Bound.Left, PageBound.Height));
                var MinGapeWidth = GetMaxAverageCharWidth() * Parent.Parent.Settings.MinWhiteGapeWidthFactor;
                for (int i = 0; i < this.Count - 1; i++) {
                    var Word = this[i];
                    var NextWord = this[i + 1];
                    if (NextWord.Bound.Left - Word.Bound.Right > MinGapeWidth) {
                        Ret.Add(new Rect(Word.Bound.Right, 0, NextWord.Bound.Left - Word.Bound.Right, PageBound.Height));
                    }
                }
                if (PageBound.Width - this.Last().Bound.Right > MinGapeWidth) {
                    Ret.Add(new Rect(this.Last().Bound.Right, 0, PageBound.Width - this.Last().Bound.Right, PageBound.Height));
                }
                if (this.NonSplitWords != null && this.NonSplitWords.Count > 0) {
                    List<Rect> GapesToRemove = new List<Rect>();
                    foreach (var item in this.NonSplitWords) {
                        var Words = GetWordsMatchingString(item);
                        if (Words != null && Words.Count > 1) {
                            var WordGapes = Words.GetGapes();
                            foreach (var WordGape in WordGapes) {
                                foreach (var Gape in Ret) {
                                    if (Gape.IntersectsWith(WordGape)) {
                                        GapesToRemove.Add(Gape);
                                    }
                                }
                            }

                        }
                    }
                    for (int i = 0; i < GapesToRemove.Count; i++) {
                        Ret.Remove(GapesToRemove[i]);
                    }
                }
                return Ret;
            }
        }
        public Word GetWordAtPoint(Point Pt, out string PointSymbol)
        {
            PointSymbol = string.Empty;
            var WordsFound = from Word in this where Word.Bound.Contains(Pt) select Word;
            if (WordsFound.Count() < 1) {
                return null;
            }
            if (WordsFound.Count() > 1) {
                throw new Exception("Overlapped words");
            }
            var Char = WordsFound.First().GetCharAtPoint(Pt);
            if (Char != null) {
                PointSymbol = Char.Symbol.ToString();
            }
            return WordsFound.First();
        }
        public Word GetWordAtPoint(Point Pt)
        {
            string PointSymbol;
            return GetWordAtPoint(Pt, out PointSymbol);
        }
        public Words GetWordsMatchingString(string SearchString)
        {
            string Text = this.NoSpaceText;
            if (!Text.Contains(SearchString)) {
                return null;
            }
            int StartIndex = Text.IndexOf(SearchString);
            Trace.Assert(StartIndex >= 0);
            int CheckedLength = 0;
            Words Ret = new Words();
            Ret.Tag = SearchString;
            for (int i = 0; i < this.Count; i++) {
                CheckedLength += this[i].Text.Length;
                if (CheckedLength > StartIndex) {
                    if (StartIndex + SearchString.Length > CheckedLength - this[i].Text.Length) {
                        Ret.Add(this[i]);
                    } else {
                        break;
                    }
                }
            }
            return Ret;
        }
        public List<Words> GetWordsMatchingPatternExact(string SearchPattern)
        {
            List<Words> Ret = new List<Words>();
            var Match = System.Text.RegularExpressions.Regex.Match(this.Text, SearchPattern);
            while (Match.Success) {
                string FoundString = Match.Value;
                var FoundWords = this.GetWordsMatchingStringExact(FoundString);
                Ret.Add(FoundWords);
                Match = Match.NextMatch();
            }
            return Ret;
        }
        public Words GetWordsMatchingStringExact(string SearchString, int NewStartIndex = 0)
        {
            string Text = this.Text;
            if (!Text.Contains(SearchString)) {
                return null;
            }
            int StartIndex = Text.IndexOf(SearchString, NewStartIndex);
            Trace.Assert(StartIndex >= 0);
            int CheckedLength = 0;
            Words Ret = new Words();
            Ret.Tag = SearchString;
            for (int i = 0; i < this.Count; i++) {
                CheckedLength += this[i].Text.Length;
                if (CheckedLength > StartIndex) {
                    if (StartIndex + SearchString.Length > CheckedLength - this[i].Text.Length) {
                        Ret.Add(this[i]);
                    } else {
                        break;
                    }
                }
                CheckedLength++;
            }
            return Ret;
        }

        #region Private Area
        internal void SortWords()
        {
            this.Sort((x, y) => x.Bound.Left.CompareTo(y.Bound.Left));
        }
        private double GetMaxAverageCharWidth()
        {
            double Ret = 0.0;
            if (this.Count < 1) {
                return Ret;
            }
            Ret = this[0].AverageCharWidth;
            for (int i = 1; i < this.Count; i++) {
                Ret = Math.Max(Ret, this[i].AverageCharWidth);
            }
            return Ret;
        }
        private Rect PageBound
        {
            get
            {
                if (this.Count < 1) {
                    return Rect.Empty;
                }
                return this[0].Parent.Parent.Bound;
            }
        }
        private string GetText()
        {
            if (Parent.Parent.Parent.Parent.SourceFormat == E_SourceFormat.XPS) {
                return GetTextXps();
            } else {
                return GetTextPdf();
            }
        }
        private string GetTextXps()
        {
            if (this.Count < 1) {
                return null;
            }
            if (this.Count == 1) {
                return this[0].Text;
            }
            //sort all elements from left to right
            this.SortWords();

            //analyze space between text fragments
            double[] Spaces = new double[this.Count - 1];
            for (int i = 0; i < this.Count - 1; i++) {
                Spaces[i] = this[i + 1].Bound.Left - this[i].Bound.Right;
            }
            string[] JoinChars = new string[Spaces.Length];
            for (int i = 0; i < Spaces.Length; i++) {
                /*double AvgBetweenTwo = (this[i].AverageCharWidth + this[i + 1].AverageCharWidth) / 2;
                if (Spaces[i] < AvgBetweenTwo * SPACE_BORDER_RATIO) {
                    JoinChars[i] = string.Empty;
                    continue;
                }
                if (Spaces[i] >= AvgBetweenTwo * SPACE_BORDER_RATIO && Spaces[i] < AvgBetweenTwo * TAB_BORDER_RATIO) {
                    JoinChars[i] = " ";
                    continue;
                } else {
                    JoinChars[i] = "\t";
                }*/
                double AvgBetweenTwo = (this[i].AverageCharWidth + this[i + 1].AverageCharWidth) / 2;
                if (Spaces[i] < AvgBetweenTwo * TAB_BORDER_RATIO) {
                    JoinChars[i] = " ";
                    continue;
                } else {
                    JoinChars[i] = "\t";
                }
            }
            string Res = this[0].Text;
            for (int i = 0; i < JoinChars.Length; i++) {
                Res += JoinChars[i] + this[i + 1].Text;
            }
            return Res;
        }
        private string GetTextPdf()
        {
            if (this.Count < 1) {
                return null;
            }
            if (this.Count == 1) {
                return this[0].Text;
            }
            //sort all elements from left to right
            this.SortWords();

            //analyze space between text fragments
            double[] Spaces = new double[this.Count - 1];
            for (int i = 0; i < this.Count - 1; i++) {
                Spaces[i] = this[i + 1].Bound.Left - this[i].Bound.Right;
            }
            string[] JoinChars = new string[Spaces.Length];
            for (int i = 0; i < Spaces.Length; i++) {
                double AvgBetweenTwo = (this[i].AverageCharWidth + this[i + 1].AverageCharWidth) / 2;
                if (Spaces[i] < AvgBetweenTwo * TAB_BORDER_RATIO) {
                    JoinChars[i] = " ";
                    continue;
                } else {
                    JoinChars[i] = "\t";
                }
            }
            string Res = this[0].Text;
            for (int i = 0; i < JoinChars.Length; i++) {
                Res += JoinChars[i] + this[i + 1].Text;
            }
            return Res;
        }
        private string GetTextWithNoSpaces()
        {
            if (this.Count < 1) {
                return null;
            }
            if (this.Count == 1) {
                return this[0].Text;
            }
            //sort all elements from left to right
            this.SortWords();
            string Ret = string.Empty;
            for (int i = 0; i < this.Count; i++) {
                Ret += this[i].Text;
            }
            return Ret;
        }
        #endregion

    }
}
