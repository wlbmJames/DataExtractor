using ATAPY.Document.Data.Core;
using System.Linq;
using System.Windows;
using DataAnalyzer.Extensions;

namespace DataAnalyzer.SearchRules
{
    class StaticTextRule : Rule
    {
        public StaticTextRule(string title, RuleBinding ruleBinding) : this(title, ruleBinding, new Rect())
        {
        }

        public StaticTextRule(string title, RuleBinding ruleBinding, Rect searchArea) : base(title,"", ruleBinding, searchArea)
        {
        }

        public override void Check(Page page, int pageNumber = -1)
        {
            Rect rect;
            if (DependencyRule == null ||!DependencyRule.SearchResult.IsFound)
            {
                rect = SearchArea;
            }
            else
            {
                if (DependencyRule.SearchResult.PageNumber != pageNumber)
                    return;
                var left = 0;
                var top = 0;
                var right = 0;
                var bot = 0;
                left = CheckDependency(DependencyArea.RightOf);
                top = CheckDependency(DependencyArea.Below);
                right = CheckDependency(DependencyArea.LeftOf);
                bot = CheckDependency(DependencyArea.Above);
                var leftT = new Point(left, top);
                var rightB = new Point(right, bot);
                rect = new Rect(leftT, rightB);
            }
            var result = page.FindWords(rect, this.TextToSearch);
            if (result.Count < 1)
                SearchResult = new SearchResult(pageNumber);
            else
                SearchResult = new SearchResult(result.First().Tag, result.First().Bound, true, pageNumber);
        }

        public override object Clone()
        {
            return new StaticTextRule(Title, RuleBinding, SearchArea)
            {
                SearchResult = (SearchResult)this.SearchResult?.Clone(),
                TextToSearch = this.TextToSearch,
                DependencyArea = (DependencyArea)this.DependencyArea?.Clone()
            };
        }

        private int CheckDependency(Relation relation)
        {
            var area = DependencyRule.SearchResult.Area;
            var startval = 0.0d;
            switch (relation.Type)
            {
                case RelationType.Left:
                    startval = area.Left;
                    break;
                case RelationType.Top:
                    startval = area.Top;
                    break;
                case RelationType.Right:
                    startval = area.Right;
                    break;
                case RelationType.Bot:
                    startval = area.Bottom;
                    break;
                case RelationType.None:
                    break;
                default:
                    break;
            }
            var result = (int)startval + relation.Offset;
            return result;
        }
    }
}
