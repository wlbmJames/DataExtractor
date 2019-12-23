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

        public bool Check()
        {
            foreach (var rule in _rulesToCheck)
            {
                //rule.Check(_currentPage);
            }
            throw new NotImplementedException();
        }

        private Rect GetSearchArea(Rule rule)
        {
            var constraints = rule.SearchConstraints;
            var resultRect = _currentPage.Bound;
            foreach (var constraint in constraints)
            {
                switch (constraint.AreaType)
                {
                    case SearchRules.ConstraintsAdd.AreaType.Page:
                        resultRect = CheckPageConstraint(constraint, resultRect);
                        break;
                    case SearchRules.ConstraintsAdd.AreaType.Rule:
                        resultRect = CheckRuleConstraint(constraint, resultRect);
                        break;
                    case SearchRules.ConstraintsAdd.AreaType.None:
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
            //switch (constraint.ConstraintType)
            //{
            //    case SearchRules.ConstraintsAdd.ConstraintType.RightOf:
            //        break;
            //    case SearchRules.ConstraintsAdd.ConstraintType.LeftOf:
            //        break;
            //    case SearchRules.ConstraintsAdd.ConstraintType.Below:
            //        break;
            //    case SearchRules.ConstraintsAdd.ConstraintType.Above:
            //        break;
            //    case SearchRules.ConstraintsAdd.ConstraintType.None:
            //        break;
            //    default:
            //        break;
            //}
            //switch (constraint.RelationType)
            //{
            //    case SearchRules.ConstraintsAdd.RelationType.Left:
            //        break;
            //    case SearchRules.ConstraintsAdd.RelationType.Top:
            //        break;
            //    case SearchRules.ConstraintsAdd.RelationType.Right:
            //        break;
            //    case SearchRules.ConstraintsAdd.RelationType.Bot:
            //        break;
            //    case SearchRules.ConstraintsAdd.RelationType.XCenter:
            //        break;
            //    case SearchRules.ConstraintsAdd.RelationType.YCenter:
            //        break;
            //    case SearchRules.ConstraintsAdd.RelationType.None:
            //        break;
            //    default:
            //        break;
            //}
            throw new NotImplementedException();
        }

        private Rect CheckRuleConstraint(SearchConstraint constraint, Rect currentRect)
        {
            throw new NotImplementedException();
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

            }
        }
    }
}
