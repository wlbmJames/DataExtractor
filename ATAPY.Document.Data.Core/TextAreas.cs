using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATAPY.Document.Data.Core
{
    public class TextAreas : List<TextArea>
    {
        public void RemoveClones()
        {
            List<TextArea> AreasToRemove = new List<TextArea>();
            for (int i = 0; i < this.Count - 1; i++) {
                var Area = this[i];
                for (int j = i + 1; j < this.Count; j++) {
                    if (Area == this[j]) {
                        AreasToRemove.Add(Area);
                    }
                }
            }
            if (AreasToRemove.Count > 0) {
                foreach (var item in AreasToRemove) {
                    this.Remove(item);
                }

            }
        }
    }
}
