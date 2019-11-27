using ATAPY.Document.Data.Core;
using DataAnalyzer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalyzer
{
    public class ClassifiedDocument
    {
        public Document Document { get; set; }
        public DocClass DocClass { get; set; }
    }
}
