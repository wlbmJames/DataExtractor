using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATAPY.Document.Data.Core
{
    [Serializable]
    public class FiltrationRules : List<FiltrationRule>
    {
        public override string ToString()
        {
            if (this.Count < 0) {
                return string.Empty;
            }
            string[] Ret = new string[this.Count];
            for (int i = 0; i < this.Count; i++) {
                Ret[i] = this[i].Rule;// +"|" + this[i].DropWholeString.ToString();
            }
            return string.Join("|", Ret);
        }
        public void FillFromString(string Rules)
        {
            var Array = Rules.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            this.Clear();
            foreach (var item in Array) {
                this.Add(new FiltrationRule() { Rule = item, DropWholeString = true });
            }
        }
    }
}
