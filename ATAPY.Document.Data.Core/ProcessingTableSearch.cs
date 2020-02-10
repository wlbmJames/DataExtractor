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
    internal class ProcessingTableSearch
    {
        public static void SearchTable(Page Page, out bool FooterFound)
        {
            FooterFound = false;
            TextLines Lines = new TextLines(Page);
            Lines.AddRange(Page.TextLines);
            var Options = Page.Settings;
            Page.Tables.Clear();
            //var HeaderLines = GetTextLinesInRect(Header);
            //var HeaderLine = GetHeaderLine(Lines, Options.HeaderRules);

            bool HeaderFound = true;
            while (HeaderFound) {
                var HeaderLines = GetHeaderLines(Lines, Options.HeaderRules);
                if (HeaderLines.Count < 1) {
                    //no header found
                    if (!Options.CanHaveNoHeaders) {
                        return;
                    }
                    HeaderFound = false;
                }
                //temp
                //HeaderLines[0].RemoveRange(2, 2);
                TextLines LinesBelowHeader;
                if (HeaderFound) {
                    LinesBelowHeader = Lines.GetLinesInArea(new Rect(0, HeaderLines.Last().Bound.Bottom + 1.0, Page.Width, Page.Height - HeaderLines.Last().Bound.Bottom));
                } else {
                    LinesBelowHeader = SearchTableDataStartWithoutHeader(Page);
                }
                if (LinesBelowHeader.Count < 1) {
                    return;
                }
                TextLines AllTableLines = new TextLines(Page);
                AllTableLines.AddRange(HeaderLines);
                var StopLine = GetStopLine(LinesBelowHeader, Options.StopRules);
                if (StopLine != null) {
                    var IndexToRemove = LinesBelowHeader.IndexOf(StopLine);
                    LinesBelowHeader.RemoveRange(IndexToRemove, LinesBelowHeader.Count - IndexToRemove);
                    FooterFound = true;
                } else {
                    var FooterLine = GetFooterLine(LinesBelowHeader, Options.FooterRules);
                    if (FooterLine != null) {
                        var IndexToRemove = LinesBelowHeader.IndexOf(FooterLine);
                        LinesBelowHeader.RemoveRange(IndexToRemove, LinesBelowHeader.Count - IndexToRemove);
                    }

                }

                FilterLines(LinesBelowHeader, Options.FiltrationRules);
                AllTableLines.AddRange(LinesBelowHeader);

                if (Page.Settings.HeaderRules.HasRuleWithGroups) {
                    var Groups = Page.Settings.HeaderRules.GetAllGroups();
                    foreach (var item in HeaderLines) {
                        item.NonSplitWords = Groups;
                    }
                }

                var CommonGapes = AllTableLines.GetCommonWhiteGapes();
                /*if (Page.Settings.ProhibitUnnamedColumns) {
                    List<Rect> GapesToRemove = new List<Rect>();
                    var HeaderWhiteGapes = HeaderLines.GetCommonWhiteGapes();
                    //тут нужно проверять не совпадение количества, а сравнивать WhiteGapes заголовка со всеми найденными CommonGapes (может быть ситуация, когда в заголовке на 1 больше
                    //(дальше не прошел), но при поиска нашелся лишний общий. Тогда правило фильтрации не сработает.
                    //if (CommonGapes.Count > HeaderWhiteGapes.Count) {
                    foreach (var Hgape in HeaderWhiteGapes) {
                        var Intersects = Hgape.GetIntersecions(CommonGapes);
                        if (Intersects.Count > 1) {
                            if (Page.Settings.UnnamedColumnsMergeDirection == E_ColumnsMergeDirection.Left) {
                                Intersects.Remove(Intersects.Last()); //Gape we like to keep. Most right
                            } else {
                                Intersects.Remove(Intersects.First()); //Gape we like to keep. Most left
                            }
                            for (int i = 0; i < Intersects.Count; i++) {
                                GapesToRemove.Add(Intersects[i]);
                            }
                        }
                    }
                    for (int i = 0; i < GapesToRemove.Count; i++) {
                        CommonGapes.Remove(GapesToRemove[i]);
                    }
                    //}
                }*/
                if (Page.Settings.ExtraColumnSplit && Page.Settings.ExtraColumnsSplitIndices.Count > 0) {
                    var Indices = Page.Settings.ExtraColumnsSplitIndices.OrderByDescending(x => x).ToList();
                    for (int i = 0; i < Indices.Count; i++) {
                        int SplitColIndex = Indices[i];
                        if (SplitColIndex <= CommonGapes.Count - 1) {
                            var DataCommonGapes = LinesBelowHeader.GetCommonWhiteGapes();
                            if (DataCommonGapes == null) {
                                continue;
                            }
                            Rect ColumnArea = new Rect(CommonGapes[SplitColIndex - 1].Right, 0, CommonGapes[SplitColIndex].Left - CommonGapes[SplitColIndex - 1].Right, Page.Height);
                            var DataGapesInColumn = ColumnArea.GetIntersecions(DataCommonGapes);
                            if (DataGapesInColumn.Count > 2) {
                                if (DataGapesInColumn[0].IntersectsWith(CommonGapes[SplitColIndex - 1])) {
                                    DataGapesInColumn.RemoveAt(0);
                                }
                                if (DataGapesInColumn.Last().IntersectsWith(CommonGapes[SplitColIndex])) {
                                    DataGapesInColumn.Remove(DataGapesInColumn.Last());
                                }
                                /*if (DataGapesInColumn.Count == 1) {
                                    CommonGapes.Insert(SplitColIndex, DataGapesInColumn[0]);
                                    CommonGapes.InsertRange(SplitColIndex, DataGapesInColumn);
                                }*/
                                CommonGapes.InsertRange(SplitColIndex, DataGapesInColumn);
                            }
                        }
                    }
                }

                if (Page.Settings.ProhibitUnnamedColumns && HeaderFound) {
                    List<Rect> GapesToRemove = new List<Rect>();
                    var HeaderWhiteGapes = HeaderLines.GetCommonWhiteGapes();
                    //тут нужно проверять не совпадение количества, а сравнивать WhiteGapes заголовка со всеми найденными CommonGapes (может быть ситуация, когда в заголовке на 1 больше
                    //(дальше не прошел), но при поиска нашелся лишний общий. Тогда правило фильтрации не сработает.
                    //if (CommonGapes.Count > HeaderWhiteGapes.Count) {
                    foreach (var Hgape in HeaderWhiteGapes) {
                        var Intersects = Hgape.GetIntersecions(CommonGapes);
                        if (Intersects.Count > 1) {
                            if (Page.Settings.UnnamedColumnsMergeDirection == E_ColumnsMergeDirection.Left) {
                                if (!(Intersects.Count == 2 && CommonGapes.First() == Intersects.First())) {
                                    Intersects.Remove(Intersects.Last()); //Gape we like to keep. Most right
                                }
                            } else {
                                if (!(Intersects.Count == 2 && CommonGapes.Last() == Intersects.Last())) {
                                    Intersects.Remove(Intersects.First()); //Gape we like to keep. Most left
                                }
                            }
                            for (int i = 0; i < Intersects.Count; i++) {
                                GapesToRemove.Add(Intersects[i]);
                            }
                        }
                    }
                    for (int i = 0; i < GapesToRemove.Count; i++) {
                        CommonGapes.Remove(GapesToRemove[i]);
                    }
                    //}
                }

                //Trace.Assert(CommonGapes.Count == HeaderLines.GetCommonWhiteGapes().Count); ?????????
                Table Table = new Table();
                Table.Bounds = AllTableLines.Bound;
                Table.VSeparators.AddRange(CommonGapes);
                //don't remove if table is glued to the left
                if (CommonGapes.First().Left == 0.0) {
                    Table.VSeparators.Remove(CommonGapes.First());
                }
                //don't remove if table is glued to the right
                if (CommonGapes.Last().Right == Page.Bound.Right) {
                    Table.VSeparators.Remove(CommonGapes.Last());
                }

                Table.VLines.Clear();
                if (Table.VSeparators.Count < 1) {
                    throw new ATAPY.Common.Warning(MessagesID.ERR_NO_COLUMNS_FOUND);
                }
                foreach (var VSep in Table.VSeparators) {
                    Table.VLines.Add(new Rect(VSep.Left + VSep.Width / 2 - 0.5, Table.Bounds.Top, 1.0, Table.Bounds.Height));
                }
                //Table.HLines.Clear();
                if (LinesBelowHeader.Count > 0) {
                    if (HeaderFound) {
                        //Table.HSeparators.Add(new Rect(Table.Bounds.Left, HeaderLines.Last().Bound.Bottom + 1.0, Table.Bounds.Width, LinesBelowHeader.First().Bound.Top - HeaderLines.Last().Bound.Bottom - 1.0));
                        Table.HSeparators.Add(new Rect(Table.Bounds.Left, HeaderLines.Last().Bound.Bottom + (LinesBelowHeader.First().Bound.Top - HeaderLines.Last().Bound.Bottom) / 2 - 0.5, Table.Bounds.Width, 1.0));
                    } else {
                        Table.HSeparators.Add(new Rect(Table.Bounds.Left, LinesBelowHeader.First().Bound.Top, Table.Bounds.Width, 1.0));
                    }
                } else {
                    if (!HeaderFound) {
                        return;
                    }
                    Table.HSeparators.Add(new Rect(Table.Bounds.Left, HeaderLines.Last().Bound.Bottom +  1.5, Table.Bounds.Width, 1.0));
                }
                /*else {
                    if (!HeaderFound) {
                        return;
                    }
                    Table.Name = "Table_" + (Page.Tables.Count + 1).ToString();
                    if (Table != null) {
                        Page.Tables.Add(Table);
                    }
                    foreach (var TLine in AllTableLines) {
                        Lines.Remove(TLine);
                    }
                    continue;
                    //return;
                    //table is empty. no table content found
                }*/
                Table.Name = "Table_" + (Page.Tables.Count + 1).ToString();
                AssignDataToTable(Table, HeaderLines, LinesBelowHeader, Options);
                if (LinesBelowHeader.Count < 1) {
                    Table.HSeparators.Clear();
                }
                if (LinesBelowHeader.Count == 0) {
                    return;
                }
                if (HeaderLines.Count < 1 && Options.CanHaveNoHeaders) {
                    Table.NoHeader = true;
                }
                if (Table != null) {
                    Page.Tables.Add(Table);
                }
                if (!Options.MultipleTablesOnPage) {
                    return;
                }
                foreach (var TLine in AllTableLines) {
                    Lines.Remove(TLine);
                }
            }
        }
        public static DataTable MergeTables(Pages Pages)
        {
            if (Pages == null || Pages.Count < 1) {
                return null;
            }
            var AllTables = GetPagesTables(Pages);
            if (AllTables.Count < 1) {
                return null;
            }
            return MergeTables(AllTables);
        }
        public static DataTable MergeTables(List<DataTable> Tables)
        {
            if (Tables == null || Tables.Count < 1) {
                return null;
            }
            DataTable Ret = Tables[0];
            for (int i = 1; i < Tables.Count; i++) {
                Ret = MergeTwoTables(Ret, Tables[i], i > 1, i < Tables.Count - 1);
            }
            return Ret;
        }
        private static List<DataTable> GetPagesTables(Pages Pages)
        {
            List<DataTable> Ret = new List<DataTable>();
            for (int i = 0; i < Pages.Count; i++) {
                for (int j = 0; j < Pages[i].Tables.Count; j++) {
                    Ret.Add(Pages[i].Tables[j].Data);
                }
            }
            return Ret;
        }
        private static DataTable MergeTwoTables(DataTable MasterTable, DataTable AddTable, bool SkipFirst = true, bool SkipLast = true)
        {
            DataTable Ret = MasterTable.Copy();
            if (AddTable.Rows.Count < 1) {
                return Ret;
            }
            int ColumnsCount = Ret.Columns.Count;
            if (ColumnsCount != AddTable.Columns.Count) {
                throw new Exception("Cannot merge two tables with different number of columns");
            }
            int[] MinLines;
            int[] MaxLines;
            int[] MinLinesAdd;
            int[] MaxLinesAdd;
            GetMinMaxColumnTextLines(MasterTable, SkipFirst, SkipLast, out MinLines, out MaxLines);
            if (GetMinMaxColumnTextLines(AddTable, SkipFirst, SkipLast, out MinLinesAdd, out MaxLinesAdd)) {
                for (int i = 0; i < ColumnsCount; i++) {
                    MinLines[i] = Math.Min(MinLines[i], MinLinesAdd[i]);
                    MaxLines[i] = Math.Max(MaxLines[i], MaxLinesAdd[i]);
                }
            }
            //check last master and firs add table rows
            bool[] LastRowUses = new bool[ColumnsCount];
            bool[] FirsRowUses = new bool[ColumnsCount];
            bool CanJoinTwoRows = true;
            if (Ret.Rows.Count < 1) {
                return AddTable;
            }
            if (AddTable.Rows.Count > 0) {
                for (int i = 0; i < ColumnsCount; i++) {
                    int LastRowLines = GetTextLinesCount(Ret.Rows[Ret.Rows.Count - 1][i].ToString());
                    int FirstRowLines = GetTextLinesCount(AddTable.Rows[0][i].ToString());
                    LastRowUses[i] = LastRowLines > 0;
                    FirsRowUses[i] = FirstRowLines > 0;
                    if (LastRowLines + FirstRowLines < MinLines[i] || LastRowLines + FirstRowLines > MaxLines[i]) {
                        CanJoinTwoRows = false;
                    }
                }
                int MergeOffset = 0;
                if (CanJoinTwoRows) {
                    MergeOffset = 1;
                    for (int i = 0; i < ColumnsCount; i++) {
                        if (FirsRowUses[i] && LastRowUses[i]) {
                            Ret.Rows[Ret.Rows.Count - 1][i] = Ret.Rows[Ret.Rows.Count - 1][i] + "\r\n" + AddTable.Rows[0][i];
                        } else if (FirsRowUses[i]) {
                            Ret.Rows[Ret.Rows.Count - 1][i] = AddTable.Rows[0][i];
                        }
                    }
                }
                for (int i = MergeOffset; i < AddTable.Rows.Count; i++) {
                    Ret.Rows.Add(AddTable.Rows[i].ItemArray);
                }
            }
            return Ret;
        }
        private static bool GetMinMaxColumnTextLines(DataTable Table, bool SkipFirst, bool SkipLast, out int[] MinLines, out int[] MaxLines)
        {
            var ColumnsCount = Table.Columns.Count;
            MinLines = new int[ColumnsCount];
            MaxLines = new int[ColumnsCount];

            int StartOffset = SkipFirst ? 1 : 0;
            int EndOffset = SkipLast ? 1 : 0;
            if (Table.Rows.Count < StartOffset + EndOffset + 1) {
                return false;
            }
            for (int j = 0; j < ColumnsCount; j++) {
                var Row = Table.Rows[StartOffset];
                if (string.IsNullOrEmpty(Row[j].ToString())) {
                    MinLines[j] = 0;
                    MaxLines[j] = 0;
                } else {
                    int CellLines = GetTextLinesCount(Row[j].ToString());
                    MinLines[j] = CellLines;
                    MaxLines[j] = CellLines;
                }
            }
            for (int i = 1 + StartOffset; i < Table.Rows.Count - EndOffset; i++) {
                var Row = Table.Rows[i];
                for (int j = 0; j < ColumnsCount; j++) {
                    if (string.IsNullOrEmpty(Row[j].ToString())) {
                        MinLines[j] = 0;
                    } else {
                        int CellLines = GetTextLinesCount(Row[j].ToString());
                        MinLines[j] = Math.Min(MinLines[j], CellLines);
                        MaxLines[j] = Math.Max(MaxLines[j], CellLines);
                    }
                }
            }
            return true;
        }
        private static int GetTextLinesCount(string Text)
        {
            if (string.IsNullOrEmpty(Text)) {
                return 0;
            }
            return Text.Replace("\r\n", "\r").Replace("\n", "\r").Split(new char[] { '\r' }).Length;
        }
        private static TextLines SearchTableDataStartWithoutHeader(Page Page)
        {
            int LastIndex = Page.TextLines.Count;
            int DesiredColumnsCount = Page.Settings.DesiredColumnsCount;
            var StopLine = GetStopLine(Page.TextLines, Page.Settings.StopRules);
            if (StopLine != null) {
                LastIndex = Page.TextLines.IndexOf(StopLine);
            }
            int[] GapesSpread = new int[LastIndex];
            int[] ColumnsPerLine = new int[LastIndex];
            TextLine FirstLineMatchCriteria = null;
            for (int i = 0; i < LastIndex; i++) {
                if (Page.TextLines[i].WhiteGapes.Count < DesiredColumnsCount) {
                    continue;
                }
                //here we found first line that match desired criteria
                FirstLineMatchCriteria = Page.TextLines[i];
                break;

                /*ColumnsPerLine[i] = Page.TextLines[i].WhiteGapes.Count - 1;
                TextLines CheckLines = new TextLines(Page);
                CheckLines.AddRange(Page.TextLines.GetRange(i, Page.TextLines.Count - i));
                GapesSpread[i] = CheckLines.GetCommonWhiteGapes().Count - 2;*/
            }
            if (FirstLineMatchCriteria == null) {
                return new TextLines(Page);
            }
            int StartIndex = Page.TextLines.IndexOf(FirstLineMatchCriteria);
            TextLines CheckLines = new TextLines(Page);
            CheckLines.Add(FirstLineMatchCriteria);
            for (int i = StartIndex + 1; i < LastIndex; i++) {
                CheckLines.Add(Page.TextLines[i]);
                if (CheckLines.GetCommonWhiteGapes().Count < DesiredColumnsCount) {
                    CheckLines.Remove(CheckLines.Last());
                    break;
                }
            }
            //check top lines
            for (int i = StartIndex - 1; i >= 0; i--) {
                CheckLines.Insert(0, Page.TextLines[i]);
                if (CheckLines.GetCommonWhiteGapes().Count < DesiredColumnsCount) {
                    CheckLines.Remove(CheckLines.First());
                    break;
                }
            }
            return CheckLines;
        }
        internal static void FilterLines(TextLines TextLines, List<FiltrationRule> FiltrationRules)
        {
            List<TextLine> LinesToRemove = new List<TextLine>();
            foreach (var Line in TextLines) {
                foreach (var Rule in FiltrationRules) {
                    if (!Rule.IsRuleValid) {
                        continue;
                    }
                    var Match = System.Text.RegularExpressions.Regex.Match(Line.NoSpaceText, Rule.Rule, System.Text.RegularExpressions.RegexOptions.ExplicitCapture | System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace);
                    while (Match.Success) {
                        if (Rule.DropWholeString) {
                            if (!LinesToRemove.Contains(Line)) {
                                LinesToRemove.Add(Line);
                            }
                        } else {
                            string StringToRemove = Match.Value;
                            var ElementsToRemove = Line.GetWordsMatchingString(StringToRemove);
                            foreach (var item in ElementsToRemove) {
                                Line.Remove(item);
                            }
                        }
                        Match = Match.NextMatch();
                    }
                }
            }
            //remove lines that have filtered elements with DropeWholeString option
            if (LinesToRemove.Count > 0) {
                for (int i = 0; i < LinesToRemove.Count; i++) {
                    TextLines.Remove(LinesToRemove[i]);
                }
            }
            //remove lines that have empty elements after remove found substrings in them
            for (int i = TextLines.Count - 1; i >= 0; i--) {
                if (TextLines[i].Count < 1) {
                    TextLines.RemoveAt(i);
                }
            }
        }
        private static TextLine GetStopLine(TextLines TextLines, RegExRules StopRules)
        {
            foreach (var Line in TextLines) {
                foreach (var Rule in StopRules) {
                    var Match = System.Text.RegularExpressions.Regex.Match(Line.NoSpaceText, Rule.Rule, System.Text.RegularExpressions.RegexOptions.ExplicitCapture | System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace);
                    if (Match.Success) {
                        return Line;
                    }
                }
            }
            return null;
        }
        private static TextLine GetFooterLine(TextLines TextLines, RegExRules FooterRules)
        {
            foreach (var Line in TextLines) {
                foreach (var Rule in FooterRules) {
                    if (!Rule.IsRuleValid) {
                        continue;
                    }
                    var Match = System.Text.RegularExpressions.Regex.Match(Line.NoSpaceText, Rule.Rule, System.Text.RegularExpressions.RegexOptions.ExplicitCapture | System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace);
                    if (Match.Success) {
                        return Line;
                    }
                }
            }
            return null;
        }
        private static TextLines GetHeaderLines(TextLines TextLines, RegExRules HeaderRules)
        {
            //int MatchCount = 0;
            TextLines Ret = new TextLines(TextLines.Parent);
            RegExRules UsedRules = new RegExRules();
            foreach (var Line in TextLines) {
                int InLineMatch = 0;
                foreach (var Rule in HeaderRules) {
                    var Match = System.Text.RegularExpressions.Regex.Match(Line.NoSpaceText, Rule.Rule, System.Text.RegularExpressions.RegexOptions.ExplicitCapture | System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace);
                    if (Match.Success) {
                        InLineMatch++;
                        if (!UsedRules.Contains(Rule)) {
                            UsedRules.Add(Rule);
                        }
                    }
                }
                if (InLineMatch < 1) {
                    //MatchCount = 0;
                    UsedRules.Clear();
                    Ret.Clear();
                } else {
                    //MatchCount += InLineMatch;
                    Ret.Add(Line);
                }
                if (UsedRules.Count >= HeaderRules.Count) {
                    return Ret;
                }
            }
            return new TextLines(TextLines.Parent);
        }
        private static void AssignDataToTable(Table Table, TextLines HeaderLines, TextLines DataLines, PageTableSearchSettings Options)
        {
            Table.Data = new DataTable();
            DataTable Tab = Table.Data;
            int ColumnsCount = Table.VSeparators.Count + 1;
            // number of rows except for header row
            int RowsCount = Table.HSeparators.Count;

            bool NoRowLines = (RowsCount == 1);

            //fill column names
            double Left = Table.Bounds.Left;
            double Top = Table.Bounds.Top;
            double Bottom = Table.HSeparators[0].Top;
            double Right;
            List<Rect> RectToDelete = new List<Rect>();
            for (int i = 0; i < ColumnsCount; i++) {
                if (i < ColumnsCount - 1) {
                    Right = Table.VSeparators[i].Left;
                } else {
                    Right = Table.Bounds.Right;
                }
                string CellText;
                if (HeaderLines.Count < 1 && Options.CanHaveNoHeaders) {
                    CellText = "Column_" + i.ToString();
                } else {
                    Rect Cell = new Rect(Left, Top, Right - Left, Bottom - Top);
                    CellText = HeaderLines.GetTextInArea(Cell);
                }

                //in case of multi-line header we need this part to be checked!!!
                if (string.IsNullOrEmpty(CellText.Trim()) && Options.MergeUnnamedColumns) {
                    if (Options.UnnamedColumnsMergeDirection == E_ColumnsMergeDirection.Left) {
                        RectToDelete.Add(Table.VSeparators[i - 1]);
                    }
                    if (Options.UnnamedColumnsMergeDirection == E_ColumnsMergeDirection.Right) {
                        RectToDelete.Add(Table.VSeparators[i]);
                    }
                } else {
                    if (!Tab.Columns.Contains(CellText.Trim())) {
                        Tab.Columns.Add(CellText.Trim());
                    } else {
                        Tab.Columns.Add(CellText.Trim() + Guid.NewGuid().ToString());
                    }
                }
                if (i < ColumnsCount - 1) {
                    Left = Table.VSeparators[i].Right;
                }
            }
            for (int i = 0; i < RectToDelete.Count; i++) {
                Table.VSeparators.Remove(RectToDelete[i]);
            }

            // fill data rows
            int MinRowStrings;
            int MaxRowStrings;
            int[] RowStringsPerColumn;
            FillRowsData(Table, DataLines, out MinRowStrings, out MaxRowStrings, out RowStringsPerColumn);

            // search for data rows if table has no internal row line separators
            if (MaxRowStrings > 1 && NoRowLines) {
                Trace.Assert(Tab.Rows.Count == 1);
                if (Options.RowsSearchRule == E_TableSearch.MaxStringsCount) {
                    AddRowSeparators(Table, DataLines, MaxRowStrings);
                    FillRowsData(Table, DataLines);
                } else if (Options.RowsSearchRule == E_TableSearch.ByMasterColumn && Options.MasterColumnIndex != -1) {
                    Trace.Assert(Options.MasterColumnIndex >= 0);
                    Trace.Assert(Options.MasterColumnIndex < ColumnsCount);
                    AddRowSeparatorsBasedOnColumn(Table, DataLines, RowStringsPerColumn[Options.MasterColumnIndex], Options.MasterColumnIndex, Options.MasterColumnCellAlignment);
                    FillRowsData(Table, DataLines);
                } else if (Options.RowsSearchRule == E_TableSearch.MostUsedCount) {
                    int ColumnIndex;
                    int MostUsedRowsCount = GetMostUsed(RowStringsPerColumn, out ColumnIndex);
                    if (MostUsedRowsCount == -1) {
                        //do nothing. we didn't found most used column rows count
                        return; ;
                    }
                    if (MostUsedRowsCount == 1) {
                        //do nothing. we already found 1 row.
                        return;
                    } else {
                        Trace.Assert(MostUsedRowsCount > 1);
                        Trace.Assert(ColumnIndex >= 0);
                        Trace.Assert(ColumnIndex < ColumnsCount);
                        //search for additional rows separator base on found rows count and column index
                        AddRowSeparatorsBasedOnColumn(Table, DataLines, MostUsedRowsCount, ColumnIndex, Options.MasterColumnCellAlignment);
                        FillRowsData(Table, DataLines);
                    }
                }
            }
        }
        private static void FillRowsData(Table Table, TextLines DataLines)
        {
            int a, b;
            int[] c;
            FillRowsData(Table, DataLines, out a, out b, out c);
        }
        private static void AddRowSeparators(Table Table, TextLines DataLines, int MaxRowStrings)
        {
            Rect RowsRect = Table.RowsBounds;
            Trace.Assert(DataLines.Count == MaxRowStrings);
            for (int i = 0; i < DataLines.Count - 1; i++) {
                double TextBottom = 0.0;
                foreach (var Line in DataLines[i]) {
                    TextBottom = Math.Max(Line.Bound.Bottom, TextBottom);
                }
                Trace.Assert(TextBottom > 0.0);
                double NextTextTop = DataLines[i + 1][0].Bound.Top;
                foreach (var Line in DataLines[i + 1]) {
                    NextTextTop = Math.Min(Line.Bound.Top, NextTextTop);
                }
                Trace.Assert(NextTextTop > 0.0);
                double MidPoint = TextBottom + ((NextTextTop - TextBottom) / 2.0) - 0.5;
                Table.HSeparators.Add(new Rect(Table.Bounds.Left, MidPoint, Table.Bounds.Width, 1.0));
            }
        }
        private static void FillRowsData(Table Table, TextLines DataLines, out int MinRowStrings, out int MaxRowStrings, out int[] RowStringsPerColumn)
        {
            MaxRowStrings = 0;
            MinRowStrings = int.MaxValue;
            DataTable Tab = Table.Data;
            int RowsCount = Table.HSeparators.Count;
            int ColumnsCount = Table.VSeparators.Count + 1;
            RowStringsPerColumn = new int[ColumnsCount];
            Tab.Rows.Clear();
            Trace.Assert(Tab.Columns.Count > 0);
            if (DataLines.Count < 1 && RowsCount == 1) { //empty table
                return;
            }
            for (int i = 0; i < RowsCount; i++) {
                Tab.Rows.Add();
                double Top = Table.HSeparators[i].Bottom;
                double Bottom;
                if (i < RowsCount - 1) {
                    Bottom = Table.HSeparators[i + 1].Top;
                } else {
                    Bottom = Table.Bounds.Bottom;
                }
                double Left = Table.Bounds.Left;
                for (int j = 0; j < ColumnsCount; j++) {
                    double Right;
                    if (j < ColumnsCount - 1) {
                        Right = Table.VSeparators[j].Left;
                    } else {
                        Right = Table.Bounds.Right;
                    }
                    Rect Cell = new Rect(Left, Top, Right - Left, Bottom - Top);
                    string CellText = DataLines.GetTextInArea(Cell);
                    int StringCount = CellText.Split(new char[] { '\r' }).Length;
                    RowStringsPerColumn[j] = StringCount;
                    MaxRowStrings = Math.Max(StringCount, MaxRowStrings);
                    MinRowStrings = Math.Min(StringCount, MinRowStrings);
                    Tab.Rows[i][j] = CellText;
                    if (j < ColumnsCount - 1) {
                        Left = Table.VSeparators[j].Right;
                    }
                }
            }
        }
        private static int GetMostUsed(int[] RowStringsPerColumn, out int RowIndex)
        {
            RowIndex = -1;
            Dictionary<int, int> Counts = new Dictionary<int, int>();
            for (int i = 0; i < RowStringsPerColumn.Length; i++) {
                if (!Counts.ContainsKey(RowStringsPerColumn[i])) {
                    Counts.Add(RowStringsPerColumn[i], 1);
                } else {
                    Counts[RowStringsPerColumn[i]]++;
                }
            }
            Trace.Assert(Counts.Count > 0);
            if (Counts.Keys.Count == 1) {
                RowIndex = 0;
                return Counts.Keys.First();
            }
            int[] Values = Counts.Values.ToArray();
            Array.Sort(Values);
            //Get max
            int MaxVal = Values.Last();
            if (MaxVal > Values[Values.Length - 2]) {
                foreach (var item in Counts) {
                    if (item.Value == MaxVal) {
                        for (int i = 0; i < RowStringsPerColumn.Length; i++) {
                            if (RowStringsPerColumn[i] == item.Key) {
                                RowIndex = i;
                                break;
                            }
                        }
                        return item.Key;
                    }
                }
            }
            return -1;
        }
        private static void AddRowSeparatorsBasedOnColumn(Table Table, TextLines DataLines, int RowStrings, int ColumnIndex, E_VerticalTextCellAlignment Alignment)
        {
            Rect RowsRect = Table.RowsBounds;
            Rect ColumnRect = Table.GetDataColumnBoundsStrict(ColumnIndex);
            var TextLines = DataLines.GetLinesInArea(ColumnRect);
            if (TextLines.Count < 1) {
                return;
                throw new ATAPY.Common.Warning(MessagesID.ERR_NO_TEXT_FOR_MASTER_COLUMN, (DataLines.Parent.Index + 1).ToString());
            }
            var RowLine = DataLines.GetLinesIntersectingArea(TextLines[0].Bound);
            Rect CheckRect;
            Trace.Assert(TextLines.Count == RowStrings);
            if (Alignment == E_VerticalTextCellAlignment.Top) {
                TextLines TopLines;
                //check for extra lines above the top value in master column. If found we'll add an extra separator line
                if (RowLine.Bound.Top > RowsRect.Top) {
                    CheckRect = new Rect(RowsRect.Left, RowsRect.Top, RowsRect.Width, RowLine.Bound.Top - RowsRect.Top - 1.0);
                    TopLines = DataLines.GetLinesInArea(CheckRect);
                    if (TopLines.Count > 0) {
                        //add horizontal separator
                        double MidPoint = TopLines.Bound.Bottom + ((RowLine.Bound.Top - TopLines.Bound.Bottom) / 2.0) - 0.5;
                        Table.HSeparators.Add(new Rect(Table.Bounds.Left, MidPoint, Table.Bounds.Width, 1.0));
                    }
                }
                for (int i = 1; i < TextLines.Count; i++) {
                    try {
                        RowLine = DataLines.GetLinesIntersectingArea(TextLines[i].Bound);
                        var PrevLine = DataLines.GetLinesIntersectingArea(TextLines[i - 1].Bound);
                        CheckRect = new Rect(RowsRect.Left, PrevLine.Bound.Top, RowsRect.Width, RowLine.Bound.Top - PrevLine.Bound.Top - 1.0);
                        TopLines = DataLines.GetLinesInArea(CheckRect);
                        double MidPoint = TopLines.Last().Bound.Bottom + ((RowLine.Bound.Top - TopLines.Last().Bound.Bottom) / 2.0) - 0.5;
                        Table.HSeparators.Add(new Rect(Table.Bounds.Left, MidPoint, Table.Bounds.Width, 1.0));
                    } catch (Exception ex) {
                        var a = ex;
                        throw;
                    }
                }
            } else if (Alignment == E_VerticalTextCellAlignment.Bottom) {
                TextLines NextLine;
                TextLines BottomLines;
                for (int i = 0; i < TextLines.Count - 1; i++) {
                    RowLine = DataLines.GetLinesIntersectingArea(TextLines[i].Bound);
                    NextLine = DataLines.GetLinesIntersectingArea(TextLines[i + 1].Bound);
                    CheckRect = new Rect(RowsRect.Left, RowLine.Bound.Bottom + 1.0, RowsRect.Width, NextLine.Bound.Bottom - RowLine.Bound.Top);
                    BottomLines = DataLines.GetLinesInArea(CheckRect);
                    double MidPoint = RowLine.Last().Bound.Bottom + ((BottomLines.Bound.Top - RowLine.Last().Bound.Bottom) / 2.0) - 0.5;
                    Table.HSeparators.Add(new Rect(Table.Bounds.Left, MidPoint, Table.Bounds.Width, 1.0));
                }
                //check for extra lines below the bottom value in master column. If Found an extra separator will be added
                NextLine = DataLines.GetLinesIntersectingArea(TextLines.Last().Bound);
                if (NextLine.Last().Bound.Bottom < RowsRect.Bottom) {
                    CheckRect = new Rect(RowsRect.Left, NextLine.Last().Bound.Bottom + 1.0, RowsRect.Width, RowsRect.Bottom - NextLine.Last().Bound.Top);
                    BottomLines = DataLines.GetLinesInArea(CheckRect);
                    if (BottomLines.Count > 0) {
                        double MidPoint = NextLine.Last().Bound.Bottom + ((BottomLines.Bound.Top - NextLine.Last().Bound.Bottom) / 2.0) - 0.5;
                        Table.HSeparators.Add(new Rect(Table.Bounds.Left, MidPoint, Table.Bounds.Width, 1.0));
                    }
                }
            } else if (Alignment == E_VerticalTextCellAlignment.Medium) {
                TextLines NextLine;
                TextLines BottomLines;
                //TextLines TopLines;
                //check for extra lines above the top value in master column. If found we'll add an extra separator line
                /*if (RowLine.Bound.Top > RowsRect.Top) {
                    CheckRect = new Rect(RowsRect.Left, RowsRect.Top, RowsRect.Width, RowLine.Bound.Top - RowsRect.Top - 1.0);
                    TopLines = DataLines.GetLinesInArea(CheckRect);
                    if (TopLines.Count > 0) {
                        //add horizontal separator
                        double MidPoint = TopLines.Bound.Bottom + ((RowLine.Bound.Top - TopLines.Bound.Bottom) / 2.0) - 0.5;
                        Table.HSeparators.Add(new Rect(Table.Bounds.Left, MidPoint, Table.Bounds.Width, 1.0));
                    }
                }*/
                for (int i = 0; i < TextLines.Count - 1; i++) {
                    Rect LineBound = new Rect(RowsRect.Left, TextLines[i].Bound.Top, RowsRect.Width, TextLines[i].Bound.Height);
                    Rect NextLineBound = new Rect(RowsRect.Left, TextLines[i + 1].Bound.Top, RowsRect.Width, TextLines[i + 1].Bound.Height);
                    RowLine = DataLines.GetLinesTouchesArea(LineBound);
                    NextLine = DataLines.GetLinesTouchesArea(NextLineBound);
                    if (DataLines.IndexOf(RowLine.Last()) + 1 == DataLines.IndexOf(NextLine.First())) {
                        double MidPoint = RowLine.Last().Bound.Bottom + ((NextLine.First().Bound.Top - RowLine.Last().Bound.Bottom) / 2.0) - 0.5;
                        Table.HSeparators.Add(new Rect(Table.Bounds.Left, MidPoint, Table.Bounds.Width, 1.0));
                    }
                    /*CheckRect = new Rect(RowsRect.Left, RowLine.Bound.Bottom + 1.0, RowsRect.Width, NextLine.Bound.Bottom - RowLine.Bound.Top);
                    BottomLines = DataLines.GetLinesInArea(CheckRect);
                    double MidPoint = RowLine.Last().Bound.Bottom + ((BottomLines.Bound.Top - RowLine.Last().Bound.Bottom) / 2.0) - 0.5;
                    Table.HSeparators.Add(new Rect(Table.Bounds.Left, MidPoint, Table.Bounds.Width, 1.0));*/
                }
                //check for extra lines below the bottom value in master column. If Found an extra separator will be added
                /*NextLine = DataLines.GetLinesIntersectingArea(TextLines.Last().Bound);
                if (NextLine.Last().Bound.Bottom < RowsRect.Bottom) {
                    CheckRect = new Rect(RowsRect.Left, NextLine.Last().Bound.Bottom + 1.0, RowsRect.Width, RowsRect.Bottom - NextLine.Last().Bound.Top);
                    BottomLines = DataLines.GetLinesInArea(CheckRect);
                    if (BottomLines.Count > 0) {
                        double MidPoint = NextLine.Last().Bound.Bottom + ((BottomLines.Bound.Top - NextLine.Last().Bound.Bottom) / 2.0) - 0.5;
                        Table.HSeparators.Add(new Rect(Table.Bounds.Left, MidPoint, Table.Bounds.Width, 1.0));
                    }
                }*/
            }
        }
    }
}

