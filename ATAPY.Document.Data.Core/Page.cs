using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;

namespace ATAPY.Document.Data.Core
{
    public class Page : BasicElement
    {
        #region Declaration Area
        //private Dictionary<double, int> SymbolsCountPerFontSize = new Dictionary<double, int>();
        //private Dictionary<double, double> SymbolsWidthPerFontSize = new Dictionary<double, double>();
        #endregion

        #region Constructors
        public Page()
        {
            this.TextAreas = new TextAreas();
            this.Paths = new List<System.Windows.Shapes.Path>();
            this.Rectangles = new List<Rect>();
            this.VerticalLines = new List<Rect>();
            this.HorizontallLines = new List<Rect>();
            this.Tables = new Tables();
            this.Settings = new PageTableSearchSettings();
            this.TextLines = new TextLines(this);
            this.Words = new List<Word>();
        }
        #endregion

        #region Public Area

        #region Properties
        public PageTableSearchSettings Settings
        {
            get;
            set;
        }
        public TextAreas TextAreas
        {
            get;
            private set;
        }
        public TextLines TextLines
        {
            get;
            internal set;
        }
        public List<Word> Words
        {
            get;
            private set;
        }
        public List<System.Windows.Shapes.Path> Paths
        {
            get;
            private set;
        }
        public List<Rect> Rectangles
        {
            get;
            private set;
        }
        public List<Rect> VerticalLines
        {
            get;
            private set;
        }
        public List<Rect> HorizontallLines
        {
            get;
            private set;
        }
        public Tables Tables
        {
            get;
            internal set;
        }
        public Pages Parent
        {
            get;
            internal set;
        }
        public ATAPY.Common.IO.File PageImage
        {
            get;
            set;
        }
        public int Index
        {
            get
            {
                if (this.Parent == null) {
                    return 0;
                }
                return this.Parent.IndexOf(this);
            }
        }
        public bool HasTable
        {
            get
            {
                return this.Tables.Count > 0;
            }
        }
        public bool IsStartTable
        {
            get
            {
                var Pages = Parent;
                for (int i = 0; i < Pages.Count; i++) {
                    if (Pages[i].HasTable) {
                        return Pages[i].Index == this.Index;
                    }
                }
                return false;
            }
        }
        #endregion

        #region Methods
        public void AnalyzeData()
        {
            AnalyzeText();
            AnalyzeGeometry();
            //SearchTables();
        }
        public void AnalyzeText()
        {
            //calc average char widths
            CalcAverageSymbolWidths();
            //split TextAreas To TextLines
            CalcTextLines();

        }
        public void SearchTables(out bool FooterFound)
        {
            FooterFound = false;
            try {
                ProcessingTableSearch.SearchTable(this, out FooterFound);
            } catch (ATAPY.Common.Warning Warn) {
                ATAPY.Common.Messages.ShowWarningMessage(Warn);
            }
        }
        public void AnalyzeGeometry()
        {
            if (Parent.Parent.SourceFormat == E_SourceFormat.XPS) {
                XpsGeometryAnalyzer.ParsePaths(this.Paths, this.Rectangles, this.VerticalLines, this.HorizontallLines, Parent.Parent.Scale);
            }
        }
        public Word GetWordForPoint(Point Pt, out string PointSymbol)
        {
            return this.TextLines.GetWordAtPoint(Pt, out PointSymbol);
        }
        public void UpdateSettings(PageTableSearchSettings NewSettings)
        {
            this.Settings.UpdateFrom(NewSettings as PageTableSearchSettings);
        }
        #endregion

        #endregion

        #region Private Area
        private void CalcTextLines()
        {
            FillPageWords();
            this.TextLines = GetTextLines(this.Words);
        }
        private void CalcAverageSymbolWidths()
        {
            Dictionary<double, int> SymbolsCountPerFontSize = new Dictionary<double, int>();
            Dictionary<double, double> SymbolsWidthPerFontSize = new Dictionary<double, double>();
            foreach (var Area in TextAreas) {
                double Size = Area.FontSize;
                if (!SymbolsCountPerFontSize.ContainsKey(Area.FontSize)) {
                    SymbolsCountPerFontSize.Add(Size, Area.CharParams.Count);
                    SymbolsWidthPerFontSize.Add(Size, Area.GetAllCharsWidth());
                } else {
                    SymbolsCountPerFontSize[Size] += Area.CharParams.Count;
                    SymbolsWidthPerFontSize[Size] += Area.GetAllCharsWidth();
                }
            }
            foreach (var Area in TextAreas) {
                double Size = Area.FontSize;
                Area.AverageCharWidth = SymbolsWidthPerFontSize[Size] / SymbolsCountPerFontSize[Size];
            }

        }
        private void FillPageWords()
        {
            //List<Word> Ret = new List<Word>();
            this.Words = new List<Word>();
            foreach (var item in this.TextAreas) {
                this.Words.AddRange(item.GetWords());
            }
        }
        private List<Word> GetWordsInRect(Rect SearchArea)
        {
            //var AllWords = GetPageWords();
            var Ret = (from Word in this.Words where Word.Bound.IntersectsWith(SearchArea) && Word.Bound.IntersectRatio(SearchArea) > TextLines.ONE_LINE_MIN_INTERSECT_RATIO orderby Word.Bound.Top select Word).ToList();
            return Ret;
        }
        public TextLines GetAllPageTextLines()
        {
            //var Words = GetPageWords();
            return GetTextLines(this.Words);
        }
        public TextLine GetTextLineAtPoint(Point Pt)
        {
            //var Words = GetWordsInRect(SearchArea);
            return this.TextLines.GetTextLineAtPoint(Pt);
        }
        public TextLines GetTextLinesInRect(Rect SearchArea)
        {
            var Words = GetWordsInRect(SearchArea);
            return GetTextLines(Words);
        }
        public TextLines GetFilteredTextLinesInRect(Rect SearchArea)
        {
            var Words = GetWordsInRect(SearchArea);
            var AllLines = GetTextLines(Words);
            ProcessingTableSearch.FilterLines(AllLines, this.Settings.FiltrationRules);
            return AllLines;
        }
        public string GetTextInRect(Rect SearchArea)
        {
            var Lines = GetTextLinesInRect(SearchArea);
            if (Lines.Count > 0) {
                return Lines.Text;
            }
            return string.Empty;
        }
        public Table GetTableAtPoint(Point TablePoint)
        {
            var TablesForPoint = (from table in Tables
                                  where table.Bounds.Contains(TablePoint)
                                  select table).ToList();
            if (TablesForPoint.Count != 1) {
                return null;
            }
            return TablesForPoint[0];
        }
        private TextLines GetTextLines(List<Word> Words)
        {
            if (Words.Count < 1) {
                return new TextLines(this);
            }
            var SortedWords = (from Word in Words orderby Word.Bound.Top select Word).ToList();
            TextLines Lines = new TextLines(this);
            Lines.Add(new TextLine(Lines));
            Lines[0].Add(SortedWords[0]);
            double Midpoint = SortedWords[0].Bound.Top + SortedWords[0].Bound.Height * 0.7;
            for (int i = 1; i < SortedWords.Count; i++) {
                if (SortedWords[i].Bound.Top > Midpoint && !string.IsNullOrEmpty(SortedWords[i].Text.Replace(".", "").Replace(",", "").Replace("_", ""))) {
                    Lines.Add(new TextLine(Lines));
                    Midpoint = SortedWords[i].Bound.Top + SortedWords[i].Bound.Height * 0.7;
                }
                Lines[Lines.Count - 1].Add(SortedWords[i]);
            }
            foreach (var Line in Lines) {
                Line.SortWords();
            }
            return Lines;
        }


        #endregion

    }
}
