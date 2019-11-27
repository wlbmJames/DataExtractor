using ATAPY.Document.Data.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DataAnalyzer.SearchRules
{
    public abstract class Rule: ICloneable
    {
        protected Rule(string title, string textToSearch, RuleBinding ruleBinding) : this(title,textToSearch, ruleBinding, new Rect())
        {
        }

        protected Rule(string title, string textToSearch, RuleBinding ruleBinding, Rect searchArea)
        {
            Title = title;
            TextToSearch = textToSearch;
            SearchArea = searchArea;
            this.RuleBinding = ruleBinding;
        }

        public string Title { get; private set; }
        public string TextToSearch { get; set; }
        public Rect SearchArea { get; set; }
        public RuleBinding RuleBinding { get; set; }
        public List<Rule> Parent { get; set; }
        public SearchResult SearchResult { get; protected set; }
        public abstract void Check(Page page, int pageNumber = -1);
        public abstract object Clone();
        public Rule DependencyRule { get; set; } = null;
        public DependencyArea DependencyArea { get; set; } = null;
    }
}
