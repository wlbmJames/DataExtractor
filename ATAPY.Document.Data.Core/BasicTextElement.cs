using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATAPY.Document.Data.Core
{
    public class BasicTextElement : BasicElement
    {
        public string Text
        {
            get;
            set;
        }
        public double FontSize
        {
            get;
            set;
        }
        public double AverageCharWidth
        {
            get;
            set;
        }
    }
}
