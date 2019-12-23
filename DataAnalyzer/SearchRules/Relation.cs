using DataAnalyzer.SearchRules.ConstraintsAdd;
using System;

namespace DataAnalyzer.SearchRules
{
    public class Relation : ICloneable
    {
        public Relation()
        {
            Type = RelationType.None;
            Offset = 1;
        }
        public RelationType Type { get; set; }
        public int Offset { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
