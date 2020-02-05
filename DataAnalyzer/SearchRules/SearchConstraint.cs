using DataAnalyzer.SearchRules.ConstraintsAdd;
using System;
using System.Windows;

namespace DataAnalyzer.SearchRules
{
    public class SearchConstraint: ICloneable
    {
        internal ConstraintTypes ConstraintType { get; set; }
        internal AreaTypes AreaType { get; set; }
        public string RuleTitle { get; set; }
        public RelationTypes RelationType { get; set; }
        public Func<Rect, double> OffsetCalc { get; set; }

        public SearchConstraint()
        {
            this.ConstraintType = ConstraintTypes.None;
            this.AreaType = AreaTypes.None;
            this.RuleTitle = string.Empty;
            this.RelationType = RelationTypes.None;
        }
        #region Init
        public SearchConstraint RightOf()
        {
            this.ConstraintType = ConstraintTypes.RightOf;
            return this;
        }

        public SearchConstraint LeftOf()
        {
            this.ConstraintType = ConstraintTypes.LeftOf;
            return this;
        }

        public SearchConstraint Below()
        {
            this.ConstraintType = ConstraintTypes.Below;
            return this;
        }

        public SearchConstraint Above()
        {
            this.ConstraintType = ConstraintTypes.Above;
            return this;
        }

        public SearchConstraint Page()
        {
            this.AreaType = AreaTypes.Page;
            return this;
        }

        public SearchConstraint Rule(string ruleTitle)
        {
            this.AreaType = AreaTypes.Rule;
            this.RuleTitle = ruleTitle;
            return this;
        }

        public SearchConstraint Bot()
        {
            this.RelationType = RelationTypes.Bot;
            return this;
        }

        public SearchConstraint Top()
        {
            this.RelationType = RelationTypes.Top;
            return this;
        }

        public SearchConstraint Right()
        {
            this.RelationType = RelationTypes.Right;
            return this;
        }

        public SearchConstraint Left()
        {
            this.RelationType = RelationTypes.Left;
            return this;
        }

        public SearchConstraint XCenter()
        {
            this.RelationType = RelationTypes.XCenter;
            return this;
        }

        public SearchConstraint YCenter()
        {
            this.RelationType = RelationTypes.YCenter;
            return this;
        }
        public SearchConstraint Offset(Func<Rect, double> offsetCalc)
        {
            OffsetCalc = offsetCalc;
            return this;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion Init
    }
}
