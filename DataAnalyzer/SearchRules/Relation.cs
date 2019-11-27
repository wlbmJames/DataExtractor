using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalyzer.SearchRules
{
    public class Relation: ICloneable
    {
        public Relation()
        {
            Type = RelationType.None;
            Offset = 0;
        }
        public RelationType Type { get; set; }
        public int Offset { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
