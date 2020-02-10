using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATAPY.Document.Data.Core
{
    [Serializable]
    public class RegExRules : List<RegExRule>
    {
        public bool HasRuleWithGroups
        {
            get
            {
                foreach (var rule in this) {
                    if (rule.HasGroups) {
                        return true;
                    }
                }
                return false;
            }
        }
        public List<string> GetAllGroups()
        {
            List<string> Ret = new List<string>();
            foreach (var rule in this) {
                if (rule.HasGroups) {
                    Ret.AddRange(rule.GetGroups());
                }
            }
            return Ret;
        }
        public override string ToString()
        {
            if (this.Count < 0) {
                return string.Empty;
            }
            string[] Ret = new string[this.Count];
            for (int i = 0; i < this.Count; i++) {
                Ret[i] = this[i].Rule;
            }
            return string.Join("|", Ret);
        }
        public void FillFromString(string Rules)
        {
            var Array = Rules.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            this.Clear();
            foreach (var item in Array) {
                this.Add(new RegExRule() { Rule = item });
            }
        }
    }
}
