using ATAPY.Document.Data.Core;
using System.Linq;
using System.Windows;
using DataAnalyzer.Extensions;

namespace DataAnalyzer.SearchRules
{
    public class RepeatingCSRule : Rule
    {
        public bool HasAnotherInstance { get; set; } = false;
        public int InterlineSpaces { get; set; } = 0;
        public RepeatingCSRule PreviousInstance { get; set; } = null;
        public RepeatingCSRule(string title, RuleBinding ruleBinding) : this(title, ruleBinding, new Rect())
        {
        }

        public RepeatingCSRule(string title, RuleBinding ruleBinding, Rect searchArea) : base(title,"", ruleBinding, searchArea)
        {
        }
        public override void Check(Page page, int pageNumber = -1)
        {
            Rect rect;
            if (PreviousInstance != null)
            {
                if (PreviousInstance.SearchResult.PageNumber != pageNumber)
                    return;
                var prevRect = PreviousInstance.SearchResult.Area;
                var left = prevRect.Left - 10;
                var top = prevRect.Bottom + 1;
                var right = prevRect.Right + 10;
                var space = InterlineSpaces == 0 ? 100 : InterlineSpaces;
                var bot = prevRect.Bottom + space;
                var leftTop = new Point(left, top);
                var rightBot = new Point(right, bot);
                rect = new Rect(leftTop, rightBot);
            }
            else if (DependencyRule == null || !DependencyRule.SearchResult.IsFound)
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

            if (this.SearchResult != null && this.SearchResult.IsFound)
            {
                this.Parent.Add(GetNewInstance());
                HasAnotherInstance = true;
            }
            else
                HasAnotherInstance = false;

        }
        private RepeatingCSRule GetNewInstance()
        {
            string newTitle = "";
            var substrings = this.Title.Split('_');
            if (substrings.Length == 1)
                newTitle = this.Title + "_0";
            else if (substrings.Length == 2)
            {
                var index = int.Parse(substrings[1]);
                index++;
                newTitle = substrings[0] + "_" + index;
            }
            else
            {
                var index = int.Parse(substrings[substrings.Length - 1]);
                index++;
                for (int i = 0; i < substrings.Length - 1; i++)
                {
                    newTitle = newTitle + substrings[i] + "_";
                }
                newTitle = newTitle + index;
            }
            return new RepeatingCSRule(newTitle, RuleBinding, SearchArea)
            {
                TextToSearch = this.TextToSearch,
                PreviousInstance = this,
                Parent = this.Parent,
                InterlineSpaces = this.InterlineSpaces
                
            };
        }
        public override object Clone()
        {
            return new RepeatingCSRule(Title, RuleBinding, SearchArea)
            {
                SearchResult = (SearchResult)this.SearchResult?.Clone(),
                TextToSearch = this.TextToSearch,
                DependencyArea = (DependencyArea)this.DependencyArea?.Clone(),
                InterlineSpaces = this.InterlineSpaces
            };
        }
    }
}
