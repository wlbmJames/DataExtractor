using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace ATAPY.Document.Data.Core
{
    [Serializable]
    public class RegExRule
    {
        [Category("Basic")]
        [DisplayName("Search Rule")]
        public string Rule
        {
            get;
            set;
        }
        [Browsable(false)]
        public bool HasGroups
        {
            get
            {
                if (string.IsNullOrEmpty(this.Rule)) {
                    return false;
                }
                return System.Text.RegularExpressions.Regex.IsMatch(this.Rule, @"[\(]{1}[^\(\)]*[\)]{1}");
            }
        }
        [Browsable(false)]
        public int GroupsCount
        {
            get
            {
                if (string.IsNullOrEmpty(this.Rule)) {
                    return 0;
                }
                var Matches = System.Text.RegularExpressions.Regex.Match(this.Rule, @"[\(]{1}[^\(\)]*[\)]{1}");
                return Matches.Groups.Count;
            }
        }
        public bool IsRuleValid
        {
            get
            {
                try {
                    new Regex(Rule);
                    return true;
                } catch { }
                return false;
            }
        }
        public List<string> GetGroups()
        {
            List<string> Ret = new List<string>();

            if (string.IsNullOrEmpty(this.Rule)) {
                return Ret;
            }
            Regex Reg = new Regex(@"[\(]{1}([^\(\)]*)[\)]{1}");
            var Matches = Reg.Matches(this.Rule);
            string matchResult = string.Empty;
            if (Matches.Count > 0) {
                for (int i = 0; i < Matches.Count; i++) {
                    if (Matches[i].Groups.Count > 1) {
                        for (int j = 1; j < Matches[i].Groups.Count; j++) {
                            Ret.Add(Matches[i].Groups[j].Value);
                        }
                    }
                }
            }
            return Ret;

        }
    }
}
