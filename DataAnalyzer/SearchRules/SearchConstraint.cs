using DataAnalyzer.SearchRules.ConstraintsAdd;
using System;
using System.Windows;

namespace DataAnalyzer.SearchRules
{
    public class SearchConstraint: ICloneable
    {
        internal ConstraintTypes ConstraintType { get; private set; }
        internal AreaTypes AreaType { get; private set; }
        public string RuleTitle { get; private set; }
        public RelationTypes RelationType { get; private set; }
        public double XOffset { get; private set; }
        public double YOffset { get; private set; }

        public SearchConstraint()
        {
            this.ConstraintType = ConstraintTypes.None;
            this.AreaType = AreaTypes.None;
            this.RuleTitle = string.Empty;
            this.RelationType = RelationTypes.None;
            XOffset = 0.0;
            YOffset = 0.0;
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
        public SearchConstraint GetX(double offset)
        {
            XOffset = offset;
            return this;
        }        
        public SearchConstraint GetY(double offset)
        {
            YOffset = offset;
            return this;
        }        
        //public SearchConstraint Offset(int offset)
        //{
        //    OffInt = offset;
        //    return this;
        //}

        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion Init
    }
}
