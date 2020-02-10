using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ATAPY.Document.Data.Core
{
    public class Words : List<Word>
    {
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
        public List<Rect> GetGapes()
        {
            List<Rect> Ret = new List<Rect>();
            this.Sort((x, y) => x.Bound.Left.CompareTo(y.Bound.Left));
            if (this.Count < 2) {
                return Ret;
            }
            for (int i = 0; i < this.Count - 1; i++) {
                var Word = this[i];
                var NextWord = this[i + 1];
                Ret.Add(new Rect(Word.Bound.Right, Bound.Top, NextWord.Bound.Left - Word.Bound.Right, Bound.Height));
            }
            return Ret;
        }
        public void Remove(Words WordsToRemove)
        {
            foreach (var word in WordsToRemove) {
                this.Remove(word);
            }
        }
        public string Tag
        {
            get;
            set;
        }
        public int SymbolsCount
        {
            get
            {
                int Ret = 0;
                foreach (var Word in this) {
                    Ret += Word.CharParams.Count;
                }
                return Ret;
            }
        }
        public int StartOffset
        {
            get;
            set;
        }
        public int EndOffset
        {
            get;
            set;
        }
    }
}
