using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATAPY.Document.Data.Core
{
    public class Pages : List<Page>
    {
        internal Pages(Document Parent)
        {
            this.Parent = Parent;
        }
        public Document Parent
        {
            get;
            internal set;
        }
        public new void Add(Page Page)
        {
            base.Add(Page);
            Page.Parent = this;
        }
        public void AddPages(int PagesCount)
        {
            for (int i = 0; i < PagesCount; i++) {
                Page Page = new Page();
                this.Add(Page);
            }
        }
    }
}
