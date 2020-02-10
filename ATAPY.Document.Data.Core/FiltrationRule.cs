using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATAPY.Document.Data.Core
{
    [Serializable]
    public class FiltrationRule : RegExRule
    {
        public bool DropWholeString
        {
            get;
            set;
        }
    }
}
