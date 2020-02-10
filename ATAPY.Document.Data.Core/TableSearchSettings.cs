using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATAPY.Document.Data.Core
{
    public enum E_ColumnsMergeDirection
    {
        Left,
        Right
    }
    public enum E_TableSearch
    {
        Simple,
        SimpleMaxCount,
        SimpleMostUsedCount,
        SimpleBySpecifiedColumn
    }
    public enum E_VerticalTextCellAlignment
    {
        Top,
        Bottom
    }
    public class TableSearchSettings
    {
        public const double DEFAULT_MIN_WHITE_GAPE_WIDTH = 1.2;
        public TableSearchSettings()
        {
            MasterColumnIndex = -1;
            RowsSearchRule = E_TableSearch.SimpleMostUsedCount;
            MasterColumnCellAlignment = E_VerticalTextCellAlignment.Top;
            StopRules = new List<string>();
            HeaderRules = new List<string>();
            FiltrationRules = new List<string>();
            UnnamedColumnsMergeDirection = E_ColumnsMergeDirection.Left;
            MinWhiteGapeWidthFactor = DEFAULT_MIN_WHITE_GAPE_WIDTH;
        }
        public List<string> HeaderRules
        {
            get;
            set;
        }
        public List<string> StopRules
        {
            get;
            set;
        }
        public List<string> FiltrationRules
        {
            get;
            set;
        }
        public E_TableSearch RowsSearchRule
        {
            get;
            set;
        }
        public int MasterColumnIndex
        {
            get;
            set;
        }
        public E_VerticalTextCellAlignment MasterColumnCellAlignment
        {
            get;
            set;
        }
        public bool IsMultipageTable
        {
            get;
            set;
        }
        public bool MergeUnnamedColumns
        {
            get;
            set;
        }
        public E_ColumnsMergeDirection UnnamedColumnsMergeDirection
        {
            get;
            set;
        }
        public double MinWhiteGapeWidthFactor
        {
            get;
            set;
        }
    }
}
