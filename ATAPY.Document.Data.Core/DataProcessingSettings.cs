using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATAPY.Document.Data.Core
{
    public class DataProcessingSettings
    {
        public DataProcessingSettings()
        {
            TableSearch = new DocumentTableSearchSettings();
        }
        public DocumentTableSearchSettings TableSearch
        {
            get;
            set;
        }

    }
}
