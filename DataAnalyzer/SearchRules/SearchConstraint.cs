
using DataAnalyzer.SearchRules.ConstraintsAdd;
using System;
using System.Windows;

namespace DataAnalyzer.SearchRules
{
    public class SearchConstraint
    {
        private ConstraintType _constraintType;
        private AreaType _areaType;
        private string _ruleTitle;
        private RelationType _relationType;
        private Func<Rect, double> _offsetCalc;
        public SearchConstraint()
        {
            ConstraintType = ConstraintType.None;
            AreaType = AreaType.None;
            RuleTitle = string.Empty;
            RelationType = RelationType.None;
        }
        #region Init
        public SearchConstraint RightOf
        {
            get
            {
                ConstraintType = ConstraintType.RightOf;
                return this;
            }
        }
        public SearchConstraint LeftOf
        {
            get
            {
                ConstraintType = ConstraintType.LeftOf;
                return this;
            }
        }
        public SearchConstraint Below
        {
            get
            {
                ConstraintType = ConstraintType.Below;
                return this;
            }
        }
        public SearchConstraint Above
        {
            get
            {
                ConstraintType = ConstraintType.Above;
                return this;
            }

        }
        public SearchConstraint Page()
        {
            AreaType = AreaType.Page;
            return this;
        }
        public SearchConstraint Rule(string ruleTitle)
        {
            AreaType = AreaType.Rule;
            RuleTitle = ruleTitle;
            return this;
        }
        public SearchConstraint Bot
        {
            get
            {
                RelationType = RelationType.Bot;
                return this;
            }
        }
        public SearchConstraint Top
        {
            get
            {
                RelationType = RelationType.Top;
                return this;
            }
        }
        public SearchConstraint Right
        {
            get
            {
                RelationType = RelationType.Right;
                return this;
            }
        }
        public SearchConstraint Left
        {
            get
            {
                RelationType = RelationType.Left;
                return this;
            }
        }
        public SearchConstraint XCenter
        {
            get
            {
                RelationType = RelationType.XCenter;
                return this;
            }
        }
        public SearchConstraint YCenter
        {
            get
            {
                RelationType = RelationType.YCenter;
                return this;
            }
        }
        public SearchConstraint Offset(Func<Rect, double> offsetCalc)
        {
            OffsetCalc = offsetCalc;
            return this;
        }
        #endregion Init
        internal ConstraintType ConstraintType { get => _constraintType; private set => _constraintType = value; }
        internal AreaType AreaType { get => _areaType; private set => _areaType = value; }
        public string RuleTitle { get => _ruleTitle; private set => _ruleTitle = value; }
        public RelationType RelationType { get => _relationType; private set => _relationType = value; }
        public Func<Rect, double> OffsetCalc { get => _offsetCalc; private set => _offsetCalc = value; }
    }
}
