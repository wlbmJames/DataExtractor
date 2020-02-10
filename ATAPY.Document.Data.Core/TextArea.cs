using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace ATAPY.Document.Data.Core
{
    public class TextArea : BasicTextElement
    {
        public TextArea(Rect Bound, string Text, Page Parent)
        {
            this.Bound = Bound;
            this.Text = Text;
            this.Parent = Parent;
            this.CharParams = new List<Char>();
        }
        public Page Parent
        {
            get;
            set;
        }
        public List<Char> CharParams
        {
            get;
            private set;
        }
        public List<Word> GetWords()
        {
            List<Word> Ret = new List<Word>();
            Word Word = CreateWord();
            Word.Bound = CharParams[0].Bound;
            double AvgWidth = this.AverageCharWidth;

            for (int i = 0; i < CharParams.Count; i++) {
                var Char = CharParams[i];
                var PrevChar = Char;
                if (i > 0) {
                    PrevChar = CharParams[i - 1];
                }
                if (Char.Symbol == Document.GLYPH_SYMBOLS_WITH_NO_GEOMETRY[0] || Char.Symbol == Document.GLYPH_SYMBOLS_WITH_NO_GEOMETRY[1]) {
                    if (Word.Text != null) {
                        Ret.Add(Word);
                    }
                    Word = CreateWord();
                } else if (i > 0 && (Char.Bound.Left - PrevChar.Bound.Right > AvgWidth)) {
                    if (Word.Text != null) {
                        Ret.Add(Word);
                    }
                    Word = CreateWord();
                    Word.Bound = Char.Bound;
                    Word.Text += Char.Symbol.ToString();
                    Word.CharParams.Add(Char);
                } else {
                    if (Word.Bound.Width == 0.0 && Word.Bound.Height == 0.0) {
                        Word.Bound = Char.Bound;
                    } else {
                        Word.Bound = Rect.Union(Word.Bound, Char.Bound);
                    }
                    Word.Text += Char.Symbol.ToString();
                    Word.CharParams.Add(Char);
                }
            }
            if (!string.IsNullOrEmpty(Word.Text)) {
                Ret.Add(Word);
            }
            return Ret;
        }
        public void SetCharProperties(Rect[] CharSizes)
        {
            int Offset = 0;
            CorrectTrailingSpaces(CharSizes);
            this.CharParams.Clear();
            if (Parent.Parent.Parent.SourceFormat == E_SourceFormat.XPS) {
                //for XPS format it's the case when CharSizes do not contains any information about spaces inside text area and it's size can be less that Text.Length
                for (int i = 0; i < Text.Length; i++) {
                    Char Param = new Char();
                    Param.Symbol = Text[i];
                    if (Param.Symbol == ' ' || Param.Symbol == ' ') {
                        Trace.Assert(i > 0 && i < Text.Length - 1);
                        var LeftSymbol = GetThisHeighRect(CharSizes[i - 1 - Offset]);
                        System.Windows.Rect RightSymbol;
                        RightSymbol = CharSizes[i - Offset];
                        Rect SpaceRect = new System.Windows.Rect(LeftSymbol.Right + 0.5, LeftSymbol.Top, RightSymbol.Left - LeftSymbol.Right - 1, LeftSymbol.Height);
                        //check for previous symbol[s] are spaces
                        Param.Bound = GetRectForSpaceChar(SpaceRect);
                        Offset++;
                    } else {
                        try {
                            Param.Bound = GetThisHeighRect(CharSizes[i - Offset]);
                        } catch (Exception) {
                            throw;
                        }
                    }
                    this.CharParams.Add(Param);
                }
            } else {
                for (int i = 0; i < Text.Length; i++) {
                    Char Param = new Char();
                    Param.Symbol = Text[i];
                    Param.Bound = GetThisHeighRect(CharSizes[i]);
                    this.CharParams.Add(Param);
                }
            }
            Trace.Assert(this.CharParams.Count == this.Text.Length);

        }
        internal double GetAllCharsWidth()
        {
            double Ret = 0.0;
            foreach (var Char in this.CharParams) {
                Ret += Char.Width;
            }
            return Ret;
        }
        public override string ToString()
        {
            return base.Text;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public static bool operator ==(TextArea Area1, TextArea Area2)
        {
            if (ReferenceEquals(Area1, Area2)) {
                return true;
            }
            if (ReferenceEquals(Area1, null) || ReferenceEquals(Area2, null)) {
                return false;
            }
            if (Area1.Text == Area2.Text && Area1.Bound.IntersectRatio(Area2.Bound) > 0.8) {
                return true;
            }
            return false;
        }
        public static bool operator !=(TextArea Area1, TextArea Area2)
        {
            object Obj1 = Area1;
            object Obj2 = Area2;
            if (Obj1 == null && Obj2 == null) {
                return false;
            }
            if (Obj1 == null) {
                return true;
            }
            return !Area1.Equals(Area2);
        }
        #region Private Area
        private Word CreateWord()
        {
            Word Ret = new Word(this);
            Ret.FontSize = this.FontSize;
            Ret.AverageCharWidth = this.AverageCharWidth;
            Ret.Orientation = this.Orientation;
            return Ret;
        }
        private Rect GetThisHeighRect(Rect CharRect)
        {
            return new Rect(CharRect.Left, this.Bound.Top, CharRect.Width, this.Bound.Height);
        }
        private void CorrectTrailingSpaces(Rect[] CharSizes)
        {
            int Offset = 0;
            //correct starting spaces
            for (int i = 0; i < Text.Length; i++) {
                if (Text[i] == ' ' || Text[i] == ' ') {
                    Offset++;
                } else {
                    break;
                }
            }
            if (Offset > 0) {
                Text = Text.Substring(Offset);
                this.Bound = new System.Windows.Rect(CharSizes[0].Left, this.Bound.Top,
                        this.Bound.Right - CharSizes[0].Left, this.Bound.Height);
            }
            //correct ending spaces and no break spaces
            if (Text.EndsWith(" ") || Text.EndsWith(" ")) {
                Text = Text.TrimEnd(new char[] { ' ', ' ' });
                this.Bound = new System.Windows.Rect(
                        this.Bound.Left,
                        this.Bound.Top,
                        CharSizes.Last().Right - this.Bound.Left,
                        this.Bound.Height);
            }
        }
        private Rect GetRectForSpaceChar(Rect InitialCharRect)
        {
            Trace.Assert(this.CharParams.Count > 0);
            int FirstIndex = -1;
            for (int i = this.CharParams.Count - 1; i > 0; i--) {
                if (this.CharParams[i].Symbol == Document.GLYPH_SYMBOLS_WITH_NO_GEOMETRY[0] || this.CharParams[i].Symbol == Document.GLYPH_SYMBOLS_WITH_NO_GEOMETRY[1]) {
                    //here we set the index of first space elements sequence prior to current
                    FirstIndex = i;
                } else {
                    break;
                }
            }
            if (FirstIndex == -1) {
                return InitialCharRect;
            } else {
                double NewWidth = (InitialCharRect.Width + 1.0) / (this.CharParams.Count - FirstIndex + 1);
                //do width an left position correction to previous elements
                double Left = this.CharParams[FirstIndex].Bound.Left;
                double Top = this.CharParams[FirstIndex].Bound.Top;
                double Height = this.CharParams[FirstIndex].Bound.Height;
                double LeftOffset = 0.0;
                for (int i = FirstIndex; i < this.CharParams.Count; i++) {
                    this.CharParams[i].Bound = new Rect(Left + LeftOffset, Top, NewWidth - 0.5, Height);
                    LeftOffset += NewWidth;
                }
                return new Rect(Left + LeftOffset, Top, NewWidth - 1.0, Height);
            }
        }
        #endregion
    }
}
