using System.Windows;
using System.Linq;

namespace DataAnalyzer.SearchRules
{
    class StaticTextRule : Rule
    {
        public StaticTextRule(string title, RuleBinding ruleBinding) : this(title, ruleBinding, new Rect())
        {
        }

        public StaticTextRule(string title, RuleBinding ruleBinding, Rect searchArea) : base(title, "", ruleBinding, searchArea)
        {
        }

        public override object Clone()
        {
            return new StaticTextRule(Title, RuleBinding, SearchArea)
            {
                SearchResult = (SearchResult)this.SearchResult?.Clone(),
                TextToSearch = this.TextToSearch,
                DependencyArea = (DependencyArea)this.DependencyArea?.Clone(),
                SearchConstraints = this.SearchConstraints.Select(t => t.Clone() as SearchConstraint).ToList()
            };
        }

    }
}
