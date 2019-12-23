using System.Windows;

namespace DataAnalyzer.SearchRules
{
    public class CharacterStringRule : Rule
    {
        public CharacterStringRule(string title, RuleBinding ruleBinding) : this(title, ruleBinding, new Rect())
        {
        }

        public CharacterStringRule(string title, RuleBinding ruleBinding, Rect searchArea) : base(title, "", ruleBinding, searchArea)
        {
        }

        public override object Clone()
        {
            return new CharacterStringRule(Title, RuleBinding, SearchArea)
            {
                SearchResult = (SearchResult)this.SearchResult?.Clone(),
                TextToSearch = this.TextToSearch,
                DependencyArea = (DependencyArea)this.DependencyArea?.Clone()
            };
        }

    }
}
