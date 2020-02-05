using ATAPY.Document.Data.Core;
using DataAnalyzer.SearchRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DataAnalyzer
{
    public class RulesChecker
    {
        private List<Rule> _rulesToCheck;
        private Page _currentPage;

        public RulesChecker(List<Rule> rulesToCheck, Page currentPage)
        {
            _rulesToCheck = rulesToCheck;
            _currentPage = currentPage;
        }       
        public RulesChecker(Page currentPage)
        {
            _currentPage = currentPage;
        }

        public bool Check()
        {
            foreach (var rule in _rulesToCheck)
            {
                var area = GetSearchArea(rule);
            }
            throw new NotImplementedException();
        }

        public Rect GetSearchArea(Rule rule)
        {
            var constraints = rule.SearchConstraints;
            var resultRect = _currentPage.Bound;
            foreach (var constraint in constraints)
            {
                switch (constraint.AreaType)
                {
                    case SearchRules.ConstraintsAdd.AreaTypes.Page:
                        resultRect = CheckPageConstraint(constraint, resultRect);
                        break;
                    case SearchRules.ConstraintsAdd.AreaTypes.Rule:
                        resultRect = CheckRuleConstraint(constraint, resultRect);
                        break;
                    case SearchRules.ConstraintsAdd.AreaTypes.None:
                        break;
                    default:
                        continue;
                        break;
                }

            }
            return resultRect;
        }

        private Rect CheckPageConstraint(SearchConstraint constraint, Rect currentRect)
        {
            var rectToCheck = _currentPage.Bound;
            Rect result = RestrictArea(rectToCheck, constraint);
            return Rect.Intersect(currentRect, result);
        }

        private Rect RestrictArea(Rect rectToCheck, SearchConstraint constraint)
        {

            double dependencyCoord = 0;
            switch (constraint.RelationType)
            {
                case SearchRules.ConstraintsAdd.RelationTypes.Left:
                    dependencyCoord = rectToCheck.Left;
                    break;
                case SearchRules.ConstraintsAdd.RelationTypes.Top:
                    dependencyCoord = rectToCheck.Top;
                    break;
                case SearchRules.ConstraintsAdd.RelationTypes.Right:
                    dependencyCoord = rectToCheck.Right;
                    break;
                case SearchRules.ConstraintsAdd.RelationTypes.Bot:
                    dependencyCoord = rectToCheck.Bottom;
                    break;
                case SearchRules.ConstraintsAdd.RelationTypes.XCenter:
                    dependencyCoord = rectToCheck.X + rectToCheck.Width / 2;
                    break;
                case SearchRules.ConstraintsAdd.RelationTypes.YCenter:
                    dependencyCoord = rectToCheck.Y + rectToCheck.Height / 2;
                    break;
                case SearchRules.ConstraintsAdd.RelationTypes.None:
                    dependencyCoord = -1;
                    break;
                default:
                    dependencyCoord = -1;
                    break;
            }
            if (dependencyCoord < 0)
                return rectToCheck;
            var result = _currentPage.Bound;
            switch (constraint.ConstraintType)
            {
                case SearchRules.ConstraintsAdd.ConstraintTypes.RightOf:
                    result.X = dependencyCoord;
                    break;
                case SearchRules.ConstraintsAdd.ConstraintTypes.LeftOf:
                    result.Width = dependencyCoord;
                    break;
                case SearchRules.ConstraintsAdd.ConstraintTypes.Below:
                    result.Y = dependencyCoord;
                    break;
                case SearchRules.ConstraintsAdd.ConstraintTypes.Above:
                    result.Height = dependencyCoord;
                    break;
                case SearchRules.ConstraintsAdd.ConstraintTypes.None:
                    break;
                default:
                    break;
            }
            return result;
        }

        private Rect CheckRuleConstraint(SearchConstraint constraint, Rect currentRect)
        {
            //throw new NotImplementedException();
            var ruleName = constraint.RuleTitle;
            var rule = _rulesToCheck.FirstOrDefault(r => r.Title == ruleName);
            if (rule == null)
                throw new Exception($"Can not find rule {ruleName}");
            if (rule.SearchResult == null)
                throw new Exception("Wrong rules sequence!!!");
            var rectToCheck = new Rect();
            if (rule.SearchResult.IsFound)
            {
                rectToCheck = rule.SearchResult.Area;
                var strctedRect = RestrictArea(rectToCheck, constraint);
                currentRect = Rect.Intersect(currentRect, strctedRect);
            }
            return currentRect;
        }
    }
}
