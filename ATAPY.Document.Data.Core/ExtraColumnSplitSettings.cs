using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ATAPY.Common;

namespace ATAPY.Document.Data.Core
{
    [Serializable]
    public class ExtraColumnSplitSettings : ICloneUpdatable<ExtraColumnSplitSettings>
    {
        public ExtraColumnSplitSettings()
        {
            ColumnIndices = new List<int>();
        }
        [CloneProperty(PropType = E_CloneType.Value)]
        public bool Enabled
        {
            get;
            set;
        }
        [CloneProperty(PropType = E_CloneType.ObjectList)]
        public List<int> ColumnIndices
        {
            get;
            set;
        }

        public ExtraColumnSplitSettings Clone()
        {
            return DefaultClone.Clone(this);
        }

        public void UpdateFrom(ExtraColumnSplitSettings Source)
        {
            DefaultClone.UpdateFrom(Source, this);
        }

        public void CopyObject(ExtraColumnSplitSettings Source, ExtraColumnSplitSettings Destination)
        {
            DefaultClone.CopyObject(Source, Destination);
        }
    }
}
