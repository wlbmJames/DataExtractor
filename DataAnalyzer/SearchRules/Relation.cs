using DataAnalyzer.SearchRules.ConstraintsAdd;
using System;

namespace DataAnalyzer.SearchRules
{
    public class Relation : ICloneable
    {
        public Relation()
        {
            Type = RelationTypes.None;
            Offset = 1;
        }
        public RelationTypes Type { get; set; }
        public int Offset { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
