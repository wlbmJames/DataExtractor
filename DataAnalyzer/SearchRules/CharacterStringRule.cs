using ATAPY.Document.Data.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DataAnalyzer.Extensions;

namespace DataAnalyzer.SearchRules
{
    public class CharacterStringRule : Rule
    {
        public CharacterStringRule(string title, RuleBinding ruleBinding) : this(title, ruleBinding, new Rect())
        {
        }

        public CharacterStringRule(string title, RuleBinding ruleBinding, Rect searchArea) : base(title,"", ruleBinding, searchArea)
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
