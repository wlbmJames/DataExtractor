using ATAPY.Document.Data.Core;
using DataAnalyzer.Extensions;
using DataAnalyzer.SearchRules.ConstraintsAdd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DataAnalyzer.SearchRules
{
    public abstract class Rule : ICloneable
    {
        protected Rule(string title, string textToSearch, RuleBinding ruleBinding) : this(title, textToSearch, ruleBinding, new Rect())
        {
        }

        protected Rule(string title, string textToSearch, RuleBinding ruleBinding, Rect searchArea)
        {
            Title = title;
            TextToSearch = textToSearch;
            SearchArea = searchArea;
            this.RuleBinding = ruleBinding;
            SearchConstraints = new List<SearchConstraint>();

        }
        public List<SearchConstraint> SearchConstraints;
        public string Title { get; private set; }
        public string TextToSearch { get; set; }
        public Rect SearchArea { get; set; }
        public RuleBinding RuleBinding { get; set; }
        public List<Rule> Parent { get; set; }
        public SearchResult SearchResult { get; protected set; }
        public virtual void Check(Page page, int pageNumber = -1)
        {
            var rBuilder = new RuleChecker(this, page);
            Rect rect = rBuilder.GetSearchArea();
            #region Legacy
            //if (DependencyRule == null || !DependencyRule.SearchResult.IsFound)
            //{
            //    rect = SearchArea;
            //}
            //else
            //{
            //    if (DependencyRule.SearchResult.PageNumber != pageNumber)
            //        return;
            //    var left = 0;
            //    var top = 0;
            //    var right = 0;
            //    var bot = 0;
            //    left = CheckDependency(DependencyArea.RightOf);
            //    top = CheckDependency(DependencyArea.Below);
            //    right = CheckDependency(DependencyArea.LeftOf);
            //    bot = CheckDependency(DependencyArea.Above);
            //    var leftT = new Point(left, top);
            //    var rightB = new Point(right, bot);
            //    rect = new Rect(leftT, rightB);
            //}
            #endregion Legacy
            var result = page.FindWords(rect, this.TextToSearch);
            if (result.Count < 1)
                SearchResult = new SearchResult(pageNumber);
            else
                SearchResult = new SearchResult(result.First().Tag, result.First().Bound, true, pageNumber);

        }
        public abstract object Clone();
        public Rule DependencyRule { get; set; } = null;
        public DependencyArea DependencyArea { get; set; } = null;
        protected int CheckDependency(Relation relation)
        {
            var area = DependencyRule.SearchResult.Area;
            var startval = 0.0d;
            switch (relation.Type)
            {
                case RelationTypes.Left:
                    startval = area.Left;
                    break;
                case RelationTypes.Top:
                    startval = area.Top;
                    break;
                case RelationTypes.Right:
                    startval = area.Right;
                    break;
                case RelationTypes.Bot:
                    startval = area.Bottom;
                    break;
                case RelationTypes.None:
                    break;
                default:
                    break;
            }
            var result = (int)startval + relation.Offset;
            return result;
        }
    }
}
