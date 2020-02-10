using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace ATAPY.Document.Data.Core
{
    public class Word : BasicTextElement
    {
        internal Word(TextArea Parent)
        {
            this.Parent = Parent;
            this.CharParams = new List<Char>();
        }
        public TextArea Parent
        {
            get;
            internal set;
        }
        public List<Char> CharParams
        {
            get;
            private set;
        }
        public Char GetCharAtPoint(Point Pt)
        {
            var CharFound = from Char in this.CharParams where Char.Bound.Contains(Pt) select Char;
            if (CharFound.Count() < 1) {
                return null;
            }
            if (CharFound.Count() > 1) {
                throw new Exception("Overlaped symbols");
            }
            return CharFound.First();
        }
        public override string ToString()
        {
            return base.Text;
        }
    }
}
