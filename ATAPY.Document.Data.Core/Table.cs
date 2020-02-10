using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using System.Diagnostics;

namespace ATAPY.Document.Data.Core
{
    public class Table
    {
        #region Declaration Area

        #endregion

        #region Constructors
        internal Table()
        {
            VSeparators = new List<Rect>();
            HSeparators = new List<Rect>();
            VLines = new List<Rect>();
            //HLines = new List<Rect>();
        }
        #endregion

        #region Public Area

        #region Properties
        public Rect Bounds
        {
            get;
            internal set;
        }
        public Rect HeaderBounds
        {
            get
            {
                if (HSeparators.Count < 1) {
                    return Bounds;
                }
                return new Rect(Bounds.Left, Bounds.Top , Bounds.Width, HSeparators[0].Bottom - Bounds.Top);
            }
        }
        public Rect RowsBounds
        {
            get
            {
                if (HSeparators.Count < 1) {
                    return Rect.Empty;
                }
                return new Rect(Bounds.Left, HSeparators[0].Bottom, Bounds.Width, Bounds.Bottom - HSeparators[0].Bottom);
            }
        }
        public string Name
        {
            get;
            set;
        }
        public List<Rect> VSeparators
        {
            get;
            internal set;
        }
        public List<Rect> VLines
        {
            get;
            internal set;
        }
        public List<Rect> HSeparators
        {
            get;
            internal set;
        }
        /*public List<Rect> HLines
        {
            get;
            internal set;
        }*/
        public DataTable Data
        {
            get;
            internal set;
        }
        public bool NoHeader
        {
            get;
            set;
        }
        #endregion

        #region Methods
        public void SaveToCSV(ATAPY.Common.IO.File CsvFile)
        {
            bool IsNew = (!CsvFile.Exists || CsvFile.FileLength < 1);
            string SeparatorChar = ";";
            using (System.IO.StreamWriter File = new System.IO.StreamWriter(CsvFile.FullPath, false)) {
                File.WriteLine(string.Join(SeparatorChar, this.GetHeaders()));

                for (int i = 0; i < this.Data.Rows.Count; i++) {
                    string RowValue = this.ToCsvString(SeparatorChar, i);
                    File.WriteLine(RowValue);
                }
            }
        }
        public Rect GetDataRowBounds(int RowIndex)
        {
            Trace.Assert(RowIndex < HSeparators.Count);
            double Top;
            double Bottom;
            if (RowIndex == HSeparators.Count - 1) {
                Top = HSeparators.Last().Bottom;
                Bottom = Bounds.Bottom;
            } else {
                Top = HSeparators[RowIndex].Bottom;
                Bottom = HSeparators[RowIndex + 1].Top;
            }
            return new Rect(Bounds.Left, Top, Bounds.Width, Bottom - Top);
        }
        public Rect GetDataColumnBoundsStrict(int ColumnIndex)
        {
            Trace.Assert(ColumnIndex <= VSeparators.Count);
            double Left;
            double Right;
            if (ColumnIndex == 0) {
                Left = Bounds.Left;
                Right = VSeparators.First().Left;
            } else if (ColumnIndex == VSeparators.Count) {
                Right = Bounds.Right;
                Left = VSeparators.Last().Right;
            } else {
                Left = VSeparators[ColumnIndex - 1].Right;
                Right = VSeparators[ColumnIndex].Left;
            }
            return new Rect(Left, RowsBounds.Top, Right - Left, RowsBounds.Height);
        }
        public Rect GetDataColumnBounds(int ColumnIndex)
        {
            Trace.Assert(ColumnIndex <= VLines.Count);
            double Left;
            double Right;
            if (ColumnIndex == 0) {
                Left = Bounds.Left;
                Right = VLines.First().Left;
            } else if (ColumnIndex == VLines.Count) {
                Right = Bounds.Right;
                Left = VLines.Last().Right;
            } else {
                Left = VLines[ColumnIndex - 1].Right;
                Right = VLines[ColumnIndex].Left;
            }
            if (HSeparators.Count < 1) {
                return new Rect(Left, Bounds.Bottom, Right - Left, 0);
            }
            return new Rect(Left, RowsBounds.Top, Right - Left, RowsBounds.Height);
        }
        public Rect GetColumnHeaderBounds(int ColumnIndex, double TextLineBottom)
        {
            Trace.Assert(ColumnIndex <= VSeparators.Count);
            double Left;
            double Right;
            if (ColumnIndex == 0) {
                Left = Bounds.Left;
                Right = VSeparators.First().Left;
            } else if (ColumnIndex == VSeparators.Count) {
                Right = Bounds.Right;
                Left = VSeparators.Last().Right;
            } else {
                Left = VSeparators[ColumnIndex - 1].Right;
                Right = VSeparators[ColumnIndex].Left;
            }
            return new Rect(Left, Bounds.Top, Right - Left, TextLineBottom - Bounds.Top);
        }
        #endregion

        #endregion

        #region Private Area
        private List<string> GetHeaders()
        {
            List<string> Ret = new List<string>();
            for (int i = 0; i < this.Data.Columns.Count; i++) {
                Ret.Add("\"" + this.Data.Columns[i].ColumnName + "\"");
            }
            return Ret;
        }
        private string ToCsvString(string SeparatorChar, int RowIndex)
        {
            string[] RowValues = new string[this.Data.Columns.Count];
            for (int i = 0; i < this.Data.Columns.Count; i++) {
                RowValues[i] = "\"" + this.Data.Rows[RowIndex][i] + "\"";
            }
            return string.Join(SeparatorChar, RowValues);
        }
        #endregion

    }
}
