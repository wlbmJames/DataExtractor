using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATAPY.Common;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ATAPY.Document.Data.Core
{
    [Serializable]
    [LocalizedDisplayName("TABLE_SEARCH_SETTINGS", ResourceType = typeof(Properties.Resources))]
    public class DocumentTableSearchSettings : PageTableSearchSettings, ICloneUpdatable<DocumentTableSearchSettings>
    {
        public DocumentTableSearchSettings()
            : base()
        {
            StartPage = 1;
            EndPage = ATAPY.Common.Const.INT_UNINITIALIZED;
            IsMultipageTable = true;
        }
        [Category(TextResources.CATEGORY_ADVANCED)]
        [LocalizedDisplayName(TextResources.MULTIPAGE_TABLE, ResourceType = typeof(Properties.Resources))]
        [LocalizedDescription(TextResources.MULTIPAGE_TABLE_DESCRIPTION, ResourceType = typeof(Properties.Resources))]
        [PropertyOrder(0)]
        [CloneProperty(PropType = E_CloneType.Value)]
        public bool IsMultipageTable
        {
            get;
            set;
        }
        [Category(TextResources.CATEGORY_ADVANCED)]
        [LocalizedDisplayName(TextResources.START_PAGE, ResourceType = typeof(Properties.Resources))]
        [LocalizedDescription(TextResources.START_PAGE_DESCRIPTION, ResourceType = typeof(Properties.Resources))]
        [PropertyOrder(1)]
        [CloneProperty(PropType = E_CloneType.Value)]
        public int StartPage
        {
            get;
            set;
        }
        [Category(TextResources.CATEGORY_ADVANCED)]
        [LocalizedDisplayName(TextResources.END_PAGE, ResourceType = typeof(Properties.Resources))]
        [LocalizedDescription(TextResources.END_PAGE_DESCRIPTION, ResourceType = typeof(Properties.Resources))]
        [PropertyOrder(2)]
        [CloneProperty(PropType = E_CloneType.Value)]
        public int EndPage
        {
            get;
            set;
        }
        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                if (this.HeaderRules.Count < 1) {
                    return false;
                }
                return true;
            }
        }
        public override void Clear()
        {
            StartPage = 1;
            EndPage = ATAPY.Common.Const.INT_UNINITIALIZED;
            IsMultipageTable = true;
            base.Clear();
        }
        public new DocumentTableSearchSettings Clone()
        {
            return DefaultClone.Clone(this);
        }
        public void UpdateFrom(DocumentTableSearchSettings Source)
        {
            DefaultClone.UpdateFrom(Source, this);
        }
        public void CopyObject(DocumentTableSearchSettings Source, DocumentTableSearchSettings Destination)
        {
            //base.CopyObject(Source, Destination);
            DefaultClone.CopyObject(Source, Destination);
        }
    }
}
