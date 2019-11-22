using ATAPY.Document.Data.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextExtractor
{
    public interface IExtractor
    {
        Document GetDocument(ATAPY.Common.IO.File file);
        Document GetDocument(ATAPY.Common.IO.File file, string language);
    }
}
