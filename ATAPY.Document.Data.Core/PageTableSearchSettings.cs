using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATAPY.Common;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ATAPY.Document.Data.Core
{
    public enum E_ColumnsMergeDirection
    {
        Left,
        Right
    }
    public enum E_TableSearch
    {
        MaxStringsCount,
        MostUsedCount,
        ByMasterColumn
    }
    public enum E_VerticalTextCellAlignment
    {
        Top,
        Bottom,
        Medium
    }
    [Serializable]
    [CategoryOrder(TextResources.CATEGORY_BASIC, 0)]
    public class PageTableSearchSettings : ICloneUpdatable<PageTableSearchSettings>
    {
        public const double DEFAULT_MIN_WHITE_GAPE_WIDTH = 1.2;
        public PageTableSearchSettings()
        {
            Clear();
            /*
            MasterColumnIndex = 0;
            RowsSearchRule = E_TableSearch.ByMasterColumn;
            MasterColumnCellAlignment = E_VerticalTextCellAlignment.Top;
            StopRules = new RegExRules();
            HeaderRules = new RegExRules();
            FiltrationRules = new FiltrationRules();
            FooterRules = new RegExRules();
            UnnamedColumnsMergeDirection = E_ColumnsMergeDirection.Left;
            MinWhiteGapeWidthFactor = DEFAULT_MIN_WHITE_GAPE_WIDTH;
            ExtraColumnsSplitIndices = new List<int>();// ExtraColumnSplitSettings();*/
        }
        [Browsable(false)]
        [CloneProperty(PropType = E_CloneType.ObjectList)]
        public RegExRules HeaderRules
        {
            get;
            set;
        }
        //[LocalizedCategory("CATEGORY_BASIC", ResourceType = typeof(Properties.Resources))]
        //[DisplayName("Header Search Rules")]
        [Category(TextResources.CATEGORY_BASIC)]
        [LocalizedDisplayName(TextResources.HEADER_RULES, ResourceType = typeof(Properties.Resources))]
        [LocalizedDescription(TextResources.HEADER_RULES_DESCRIPTION, ResourceType = typeof(Properties.Resources))]
        [PropertyOrder(0)]
        [XmlIgnore]
        public string HeaderRulesForEditor
        {
            get
            {
                return this.HeaderRules.ToString();
            }
            set
            {
                this.HeaderRules.FillFromString(value);
            }
        }
        [Browsable(false)]
        [CloneProperty(PropType = E_CloneType.ObjectList)]
        public RegExRules StopRules
        {
            get;
            set;
        }
        [Category(TextResources.CATEGORY_BASIC)]
        [LocalizedDisplayName(TextResources.STOP_RULE, ResourceType = typeof(Properties.Resources))]
        [LocalizedDescription(TextResources.STOP_RULE_DESCRIPTION, ResourceType = typeof(Properties.Resources))]
        [PropertyOrder(1)]
        [XmlIgnore]
        public string StopRulesForEditor
        {
            get
            {
                return this.StopRules.ToString();
            }
            set
            {
                this.StopRules.FillFromString(value);
            }
        }
        [Browsable(false)]
        [CloneProperty(PropType = E_CloneType.ObjectList)]
        public RegExRules FooterRules
        {
            get;
            set;
        }
        [Category(TextResources.CATEGORY_BASIC)]
        [LocalizedDisplayName(TextResources.FOOTER_RULE, ResourceType = typeof(Properties.Resources))]
        [LocalizedDescription(TextResources.FOOTER_RULE_DESCRIPTION, ResourceType = typeof(Properties.Resources))]
        [PropertyOrder(4)]
        [XmlIgnore]
        public string FooterRulesForEditor
        {
            get
            {
                return this.FooterRules.ToString();
            }
            set
            {
                this.FooterRules.FillFromString(value);
            }
        }
        [Browsable(false)]
        [CloneProperty(PropType = E_CloneType.ObjectList)]
        public FiltrationRules FiltrationRules
        {
            get;
            set;
        }
        [Category(TextResources.CATEGORY_BASIC)]
        [LocalizedDisplayName(TextResources.FILTRATION_RULES, ResourceType = typeof(Properties.Resources))]
        [LocalizedDescription(TextResources.FILTRATION_RULES_DESCRIPTION, ResourceType = typeof(Properties.Resources))]
        [PropertyOrder(3)]
        [XmlIgnore]
        public string FiltrationRulesForEditor
        {
            get
            {
                return this.FiltrationRules.ToString();
            }
            set
            {
                this.FiltrationRules.FillFromString(value);
            }
        }
        [Category(TextResources.CATEGORY_BASIC)]
        [LocalizedDisplayName(TextResources.ROWS_SPLIT_RULE, ResourceType = typeof(Properties.Resources))]
        [LocalizedDescription(TextResources.ROWS_SPLIT_RULE_DESCRIPTION, ResourceType = typeof(Properties.Resources))]
        [PropertyOrder(2)]
        [CloneProperty(PropType = E_CloneType.Value)]
        //[TypeConverter(typeof(EnumTypeConverter))]
        public E_TableSearch RowsSearchRule
        {
            get;
            set;
        }
        [Browsable(false)]
        [CloneProperty(PropType = E_CloneType.Value)]
        public int MasterColumnIndex
        {
            get;
            set;
        }
        [Category(TextResources.CATEGORY_BASIC)]
        [LocalizedDisplayName(TextResources.MASTER_COLUMN_INDEX, ResourceType = typeof(Properties.Resources))]
        [LocalizedDescription(TextResources.MASTER_COLUMN_INDEX_DESCRIPTION, ResourceType = typeof(Properties.Resources))]
        [PropertyOrder(5)]
        [CloneProperty(PropType = E_CloneType.Value)]
        [XmlIgnore]
        public int DisplayMasterColumnIndex
        {
            get
            {
                return MasterColumnIndex + 1;
            }
            set
            {
                MasterColumnIndex = value - 1;
            }
        }
        [Category(TextResources.CATEGORY_BASIC)]
        [LocalizedDisplayName(TextResources.MASTER_COLUMN_CELL_ALIGNMENT, ResourceType = typeof(Properties.Resources))]
        [LocalizedDescription(TextResources.MASTER_COLUMN_CELL_ALIGNMENT_DESCRIPTION, ResourceType = typeof(Properties.Resources))]
        [PropertyOrder(6)]
        [CloneProperty(PropType = E_CloneType.Value)]
        public E_VerticalTextCellAlignment MasterColumnCellAlignment
        {
            get;
            set;
        }
        [Browsable(false)]
        [CloneProperty(PropType = E_CloneType.Value)]
        public bool MergeUnnamedColumns
        {
            get;
            set;
        }
        [Category(TextResources.CATEGORY_ADVANCED)]
        [LocalizedDisplayName(TextResources.PROHIBIT_UNNAMED_COLUMNS, ResourceType = typeof(Properties.Resources))]
        [LocalizedDescription(TextResources.PROHIBIT_UNNAMED_COLUMNS_DESCRIPTION, ResourceType = typeof(Properties.Resources))]
        [PropertyOrder(4)]
        [CloneProperty(PropType = E_CloneType.Value)]
        public bool ProhibitUnnamedColumns
        {
            get;
            set;
        }
        [Category(TextResources.CATEGORY_ADVANCED)]
        [LocalizedDisplayName(TextResources.UNNAMED_COLUMNS_MERGE_DIRECTION, ResourceType = typeof(Properties.Resources))]
        [LocalizedDescription(TextResources.UNNAMED_COLUMNS_MERGE_DIRECTION_DESCRIPTION, ResourceType = typeof(Properties.Resources))]
        [PropertyOrder(5)]
        [CloneProperty(PropType = E_CloneType.Value)]
        public E_ColumnsMergeDirection UnnamedColumnsMergeDirection
        {
            get;
            set;
        }
        [Category(TextResources.CATEGORY_ADVANCED)]
        [LocalizedDisplayName(TextResources.MIN_WHITE_GAPE_FACTOR, ResourceType = typeof(Properties.Resources))]
        [LocalizedDescription(TextResources.MIN_WHITE_GAPE_FACTOR_DESCRIPTION, ResourceType = typeof(Properties.Resources))]
        [PropertyOrder(3)]
        [CloneProperty(PropType = E_CloneType.Value)]
        public double MinWhiteGapeWidthFactor
        {
            get;
            set;
        }
        [Browsable(false)]
        [CloneProperty(PropType = E_CloneType.Value)]
        public bool CanHaveNoHeaders
        {
            get;
            set;
        }
        [Browsable(false)]
        [XmlIgnore]
        public int DesiredColumnsCount
        {
            get;
            set;
        }
        [Category(TextResources.CATEGORY_ADVANCED)]
        [LocalizedDisplayName(TextResources.EXTRA_COLUMNS_SPLIT, ResourceType = typeof(Properties.Resources))]
        [LocalizedDescription(TextResources.EXTRA_COLUMNS_SPLIT_DESCRIPTION, ResourceType = typeof(Properties.Resources))]
        //[PropertyOrder(3)]
        [CloneProperty(PropType = E_CloneType.Value)]
        public bool ExtraColumnSplit
        {
            get;
            set;
        }
        /*[Category(TextResources.CATEGORY_ADVANCED)]
        //[LocalizedDisplayName(TextResources.MIN_WHITE_GAPE_FACTOR, ResourceType = typeof(Properties.Resources))]
        //[LocalizedDescription(TextResources.MIN_WHITE_GAPE_FACTOR_DESCRIPTION, ResourceType = typeof(Properties.Resources))]
        //[PropertyOrder(3)]
        [CloneProperty(PropType = E_CloneType.Value)]
        public int ExtraColumnSplitIndex
        {
            get;
            set;
        }*/
        [Category(TextResources.CATEGORY_ADVANCED)]
        [LocalizedDisplayName(TextResources.EXTRA_COLUMNS_SPLIT_INDICES, ResourceType = typeof(Properties.Resources))]
        [LocalizedDescription(TextResources.EXTRA_COLUMNS_SPLIT_INDICES_DESCRIPTION, ResourceType = typeof(Properties.Resources))]
        [CloneProperty(PropType = E_CloneType.ObjectList)]
        public List<int> ExtraColumnsSplitIndices
        {
            get;
            set;
        }
        [Category(TextResources.CATEGORY_ADVANCED)]
        [LocalizedDisplayName(TextResources.ALLOW_MULTIPLE_TABLES_ON_PAGE, ResourceType = typeof(Properties.Resources))]
        [LocalizedDescription(TextResources.ALLOW_MULTIPLE_TABLES_ON_PAGE_DESCRIPTION, ResourceType = typeof(Properties.Resources))]
        [CloneProperty(PropType = E_CloneType.Value)]
        public bool MultipleTablesOnPage
        {
            get;
            set;
        }
        public virtual void Clear()
        {
            MasterColumnIndex = 0;
            RowsSearchRule = E_TableSearch.MostUsedCount;
            MasterColumnCellAlignment = E_VerticalTextCellAlignment.Top;
            StopRules = new RegExRules();
            HeaderRules = new RegExRules();
            FiltrationRules = new FiltrationRules();
            FooterRules = new RegExRules();
            UnnamedColumnsMergeDirection = E_ColumnsMergeDirection.Left;
            CanHaveNoHeaders = false;
            MinWhiteGapeWidthFactor = DEFAULT_MIN_WHITE_GAPE_WIDTH;
            ExtraColumnsSplitIndices = new List<int>();
        }
        public PageTableSearchSettings Clone()
        {
            return DefaultClone.Clone(this);
        }
        public void UpdateFrom(PageTableSearchSettings Source)
        {
            DefaultClone.UpdateFrom(Source, this);
        }
        public void CopyObject(PageTableSearchSettings Source, PageTableSearchSettings Destination)
        {
            DefaultClone.CopyObject(Source, Destination);
        }

    }
}
