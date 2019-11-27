using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DataAnalyzer.SearchRules
{
    public class DependencyArea: ICloneable
    {
        public DependencyArea()
        {
            Below = new Relation();
            Above = new Relation();
            RightOf = new Relation();
            LeftOf = new Relation();
        }
        public Relation Below { get; set; }
        public Relation Above { get; set; }
        public Relation RightOf { get; set; }
        public Relation LeftOf { get; set; }

        public object Clone()
        {
            return new DependencyArea()
            {
                Above = (Relation)this.Above.Clone(),
                Below = (Relation)this.Below.Clone(),
                LeftOf = (Relation)this.LeftOf.Clone(),
                RightOf = (Relation)this.RightOf.Clone()
            };
        }
    }
}
