using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATAPY.Common;
using ATAPY.Common.IO;
using ATAPY.Document.Data.Core;

using GdPicture9;
using System.Diagnostics;
using System.Windows;

namespace TextExtractor
{
    public class DigitalPdfExtractor: IExtractor
    {
        #region Declaration Area
        public class KeyVal<Key, Val>
        {
            public Key Id { get; set; }
            public Val Value { get; set; }

            public KeyVal() { }

            public KeyVal(Key key, Val val)
            {
                this.Id = key;
                this.Value = val;
            }
        }
        public const string EURO_CHAR = "€";
        private const string SEPARATOR = "|";
        private const double DEFAULT_RENDER_DPI = 300.0;
        private const double SOURCE_DPI = 72.0;
        //private const double COEFFICIENT = 300.0 / 72.0;
        //public static char[] GLYPH_SYMBOLS_WITH_NO_GEOMETRY = { ' ', ' ' };
        #endregion

        #region Constructors
        internal DigitalPdfExtractor()
        {
            LicenseManager _manager = new LicenseManager();
            _manager.RegisterKEY("411807659822389691114151231799986");
            _manager.RegisterKEY("912167958713519651119121587784646");
            RenderDPI = DEFAULT_RENDER_DPI;
            this.PageImagesFolder = new Directory(System.IO.Path.GetTempPath());
        }
        #endregion

        #region Public Area

        #region Properties
        public bool FixWordsAreas
        {
            get;
            set;
        }
        public bool ExtraTextCheck
        {
            get;
            set;
        }
        public double RenderDPI
        {
            get;
            set;
        }
        public double Scale
        {
            get
            {
                return 1.0 / SOURCE_DPI * RenderDPI;
            }
        }
        public Directory PageImagesFolder
        {
            get;
            set;
        }
        #endregion

        #region Methods
        public Document GetDocument(File SourceFile)
        {
            return GetDocument(SourceFile, "eng");
        }
        public Document GetDocument(File SourceFile, string language)
        {
            Trace.Assert(SourceFile != null);
            try
            {
                if (!SourceFile.Exists)
                {
                    throw new Warning("Source file does not exist.\r\nSource file - " + SourceFile.FullPath);
                }
                Document Doc = new Document();
                Doc.SourceFormat = E_SourceFormat.PDF;
                Doc.SourceFile = SourceFile.Clone();
                using (GdPicturePDF SourcePDF = new GdPicturePDF())
                {
                    //var PassRes = SourcePDF.SetPassword("test");
                    var Result = SourcePDF.LoadFromFile(SourceFile.FullPath, false);
                    if (Result != GdPictureStatus.OK)
                    {
                        throw new Warning("Cannot open source file.\r\nSource file -" + SourceFile.FullPath);
                    }
                    if (SourcePDF.IsEncrypted())
                    {
                        throw new Warning("Source PDF file is encrypted.");
                    }
                    int PagesCount = SourcePDF.GetPageCount();
                    //var PdfVersion = SourcePDF.GetVersion();
                    for (int i = 1; i <= PagesCount; i++)
                    {
                        SourcePDF.SelectPage(i);
                        //string aaa = SourcePDF.GetPageMetadata();
                        //string PageText = SourcePDF.GetPageText();
                        //System.IO.File.WriteAllText(ResFile.FullPath + i.ToString() + ".txt", PageText);
                        string PageTextWCoords = SourcePDF.GetPageTextWithCoords(SEPARATOR);
                        Page Page = new Page();
                        int Width = (int)(SourcePDF.GetPageWidth() * Scale);
                        int Height = (int)(SourcePDF.GetPageHeight() * Scale);

                        /*double WidthInCm = SourcePDF.GetPageWidth() / 72;
                        double HeughtInCm = SourcePDF.GetPageHeight() / 72;
                        int ImagesCount = SourcePDF.GetPageImageCount();
                        for (int j = 0; j < ImagesCount; j++) {
                            float HDPI = 0;
                            float VDPI = 0;
                            var Stat = SourcePDF.GetPageImageResolution(j, ref HDPI, ref VDPI);
                        }*/
                        Page.Bound = new Rect(0, 0, Width, Height);
                        Doc.Pages.Add(Page);
                        FillPageText(PageTextWCoords, SourcePDF, Page);
                        if (FixWordsAreas || ExtraTextCheck)
                        {
                            using (GdPictureImaging Imaging = new GdPictureImaging())
                            {
                                CleanPage(SourcePDF);
                                int RenderedPageId = SourcePDF.RenderPageToGdPictureImageEx((float)DEFAULT_RENDER_DPI, true, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
                                if (FixWordsAreas)
                                {
                                    CorrectTextCoordinates(Page, RenderedPageId);
                                }
                                if (ExtraTextCheck)
                                {
                                    CheckTextWithRecognition(Page, RenderedPageId);
                                }
                                Imaging.ReleaseGdPictureImage(RenderedPageId);
                            }
                        }
                        Page.TextAreas.RemoveClones();
                        Page.AnalyzeData();

                        /*File PNGFile = new File(OutputFolder, SourceFile.NameWithoutExtension + "_Page_" + i.ToString() + ".png");
                        File CSVFile = new File(OutputFolder, SourceFile.NameWithoutExtension + "_Page_" + i.ToString() + ".csv");
                        TablesFound += DrawPageText(PageText, Width, Height, PNGFile.FullPath, WriteText, "Helvetica");
                        PageText.SaveTablesToCSV(CSVFile);*/

                    }
                    //var res = SourcePDF.SaveToFile(SourceFile.FullPath);
                    SourcePDF.CloseDocument();
                }
                return Doc;
            }
            catch (Exception ex)
            {
                GlobalObjects.Log.WriteError(ex);
                throw;
            }

        }
        public ATAPY.Common.IO.Files CreatePageFiles(ATAPY.Common.IO.File PdfFile)
        {
            Trace.Assert(PdfFile != null);
            ATAPY.Common.IO.Files Ret = new ATAPY.Common.IO.Files();
            try {
                using (GdPicturePDF SourcePDF = new GdPicturePDF()) {
                    if (SourcePDF.LoadFromFile(PdfFile.FullPath, true) != GdPictureStatus.OK) {
                        throw new Warning("Cannot open source file.\r\nSource file -" + PdfFile.FullPath);
                    }
                    if (SourcePDF.IsEncrypted()) {
                        throw new Warning("Source PDF file is encrypted.");
                    }
                    int PagesCount = SourcePDF.GetPageCount();
                    using (GdPictureImaging Imaging = new GdPictureImaging()) {
                        for (int i = 1; i <= PagesCount; i++) {
                            SourcePDF.SelectPage(i);
                            int RenderedPageId = SourcePDF.RenderPageToGdPictureImageEx((float)DEFAULT_RENDER_DPI, true, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                            ATAPY.Common.IO.File RetFile = new ATAPY.Common.IO.File(PageImagesFolder, PdfFile.NameWithoutExtension + "_" + i.ToString("D3") + ".png");
                            Imaging.SaveAsPNG(RenderedPageId, RetFile.FullPath);
                            Imaging.ReleaseGdPictureImage(RenderedPageId);
                            Ret.Add(RetFile);
                        }
                    }
                    SourcePDF.CloseDocument();
                }
            } catch (Exception ex) {
                GlobalObjects.Log.WriteError(ex);
                throw;
            }
            return Ret;
        }
        #endregion

        #endregion

        #region Private Area
        private void CleanPage(GdPicturePDF SourcePDF)
        {
            int ImagesCount = SourcePDF.GetPageImageCount();
            List<string> ResNames = new List<string>();
            for (int i = 0; i < ImagesCount; i++) {
                string Name = SourcePDF.GetPageImageResName(i);
                if (!ResNames.Contains(Name)) {
                    ResNames.Add(Name);
                }
            }
            foreach (var item in ResNames) {
                SourcePDF.DeleteImage(item);
            }
        }
        private void CheckTextWithRecognition(Page Page, int PageImageID)
        {
            using (GdPictureImaging Imaging = new GdPictureImaging()) {
                Imaging.OCRTesseractSetOCRContext(OCRContext.OCRContextSingleBlock);
                Imaging.OCRTesseractSetPassCount(2);
                for (int i = 0; i < Page.TextAreas.Count; i++) {
                    var Area = Page.TextAreas[i];
                    if (!IsEven(Area.Text.Length)) {
                        continue;
                    }
                    var Left = Area.Text.Substring(0, Area.Text.Length / 2);
                    var Right = Area.Text.Substring(Area.Text.Length / 2);
                    if (Left != Right) {
                        continue;
                    }

                    int Width = (int)Area.Width + 2;
                    int Height = (int)Area.Height + 2;
                    int TempId = Imaging.CreateNewGdPictureImage(Width, Height, 1, System.Drawing.Color.Transparent);
                    Imaging.DrawGdPictureImageRect(PageImageID, TempId, 1, 1, Width, Height, (int)Area.Bound.Left, (int)Area.Bound.Top,
                            Width, Height, System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor);

                    //Imaging.SetROI((int)Area.Bound.Left - 10, (int)Area.Bound.Top - 1, (int)Area.Width + 2, (int)Area.Height + 20);
                    string RecognizedValue = Imaging.OCRTesseractDoOCR(TempId, "nld", ATAPY.Common.Application.Path, string.Empty);
                    Imaging.OCRTesseractClear();

                    //Imaging.SaveAsPNG(TempId, @"d:\Temp\P2P\" + string.Format("Page_{0}_Word_{1}_Cropped.png", Page.Index, i.ToString("D3")));
                    Imaging.ReleaseGdPictureImage(TempId);
                    //check for words duplication
                    if (RecognizedValue.Length * 2 == Area.Text.Length) {
                        if (Area.Text == RecognizedValue + RecognizedValue) {
                            Area.Text = RecognizedValue;
                            FillCharParams(Area);
                        }
                    } else if (ATAPY.Common.String.MatchRegularExpression(Left, @"^EUR|€|$\d+\s?[.,]?\s?\d*\s?[.,]?\s?\d{2}$")) {
                        Area.Text = Left;
                        FillCharParams(Area);
                    }
                    /*else if (IsEven(Area.Text.Length)) {
                        var Left = Area.Text.Substring(0, Area.Text.Length / 2);
                        var Right = Area.Text.Substring(Area.Text.Length / 2);
                        if (Left == Right && ATAPY.Common.String.MatchRegularExpression(Left, @"^EUR|€|$\d+\s?[.,]?\s?\d*\s?[.,]?\s?\d{2}$")) {
                            Area.Text = Left;
                            FillCharParams(Area);
                        }
                    }*/
                }
            }
        }
        private static bool IsEven(int value)
        {
            return value % 2 == 0;
        }
        private void CorrectTextCoordinates(Page Page, int PageImageID)
        {
            using (GdPictureImaging Imaging = new GdPictureImaging()) {
                var Bd = (short)Imaging.GetBitDepth(PageImageID);
                Dictionary<double, double> AverageNewHeight = new Dictionary<double, double>();
                Dictionary<double, int> AverageNewHeightAreas = new Dictionary<double, int>();
                for (int i = 0; i < Page.TextAreas.Count; i++) {
                    var Area = Page.TextAreas[i];
                    int Width = (int)Area.Width + 1;
                    int Height = (int)Area.Height + 1;
                    int TempId = Imaging.CreateNewGdPictureImage(Width, Height, 1, System.Drawing.Color.Transparent);
                    //var ExpectedHeight = Imaging.GetTextHeight(TempId, "W", "Arial", (float)Area.FontSize, GdPicture9.FontStyle.FontStyleRegular);
                    Imaging.DrawGdPictureImageRect(PageImageID, TempId, 0, 0, Width, Height, (int)Area.Bound.Left, (int)Area.Bound.Top,
                            Width, Height, System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor);
                    if (Imaging.GetAverageColor(TempId).Name == "ffffffff") {
                        //transparent or white text printed.
                        if (!AverageNewHeight.ContainsKey(Area.FontSize)) {
                            AverageNewHeight.Add(Area.FontSize, Area.Height);
                            AverageNewHeightAreas.Add(Area.FontSize, 1);
                        } else {
                            AverageNewHeight[Area.FontSize] += Area.Height;
                            AverageNewHeightAreas[Area.FontSize]++;
                        }
                        continue;
                    }
                    bool[] VerticalLines;
                    var LinesSpread = GetAreaLines(TempId, Imaging, Width, Height, out VerticalLines);
                    int TopOffset;
                    int BottomOffset;
                    int LeftOfffset;
                    int RightOffset;
                    bool Is2Dots = (Area.Text == ":" || Area.Text == ";");
                    /*if (Area.Text == "-") {
                        Imaging.ReleaseGdPictureImage(TempId);
                        continue;
                    }*/
                    AnalyzeLines(LinesSpread, out TopOffset, out BottomOffset, Is2Dots);
                    AnalyzeVerticalLines(VerticalLines, out LeftOfffset, out RightOffset);
                    /*Imaging.SaveAsPNG(TempId, @"d:\Temp\P2P\" + string.Format("Page_{0}_Word_{1}.png", Page.Index, i.ToString("D3")));
                    var stat = Imaging.Crop(TempId, LeftOfffset, TopOffset + 1, Width - LeftOfffset - RightOffset, Height - BottomOffset - TopOffset - 1);
                    Imaging.SaveAsPNG(TempId, @"d:\Temp\P2P\" + string.Format("Page_{0}_Word_{1}_Cropped.png", Page.Index, i.ToString("D3")));*/
                    /*if (Area.Width - LeftOfffset - RightOffset <= 0) {
                        //var res = Imaging.SaveAsPNG(TempId, @"d:\Test.png");
                        int a = 1;
                    }
                    if (Area.Height - TopOffset - BottomOffset <= 0) {
                        int b = 1;
                    }*/
                    Imaging.ReleaseGdPictureImage(TempId);
                    Area.Bound = new Rect(Area.Bound.Left + LeftOfffset, Area.Bound.Top + TopOffset, Area.Width - LeftOfffset - RightOffset, Area.Height - TopOffset - BottomOffset);
                    //FillCharParams(Area);
                    if (!AverageNewHeight.ContainsKey(Area.FontSize)) {
                        AverageNewHeight.Add(Area.FontSize, Area.Height - TopOffset - BottomOffset);
                        AverageNewHeightAreas.Add(Area.FontSize, 1);
                    } else {
                        AverageNewHeight[Area.FontSize] += Area.Height - TopOffset - BottomOffset;
                        AverageNewHeightAreas[Area.FontSize]++;
                    }
                }
                Dictionary<double, double> AverageNewFontSize = new Dictionary<double, double>();
                foreach (var item in AverageNewHeight) {
                    AverageNewFontSize.Add(item.Key, item.Value / AverageNewHeightAreas[item.Key]);
                }
                foreach (var Area in Page.TextAreas) {
                    Area.FontSize = AverageNewFontSize[Area.FontSize];
                    if (Area.Text == "-") {
                        int TempId = Imaging.CreateNewGdPictureImage((int)Area.Width, (int)Area.Width, 1, System.Drawing.Color.Transparent);
                        var ExpectedHeight = Imaging.GetTextHeight(TempId, "W", "Arial", (float)Area.FontSize, GdPicture9.FontStyle.FontStyleRegular);
                        int Offset = (int)((ExpectedHeight - Area.Height) / 2.0) + 1;
                        Imaging.ReleaseGdPictureImage(TempId);
                        Area.Bound = new Rect(Area.Bound.Left, Area.Bound.Top - Offset, Area.Width, Area.Height + 2 * Offset);
                    }
                    FillCharParams(Area);
                }
            }
        }
        private bool[] GetAreaLines(int AreaId, GdPictureImaging Imaging, int Width, int Height, out bool[] VerticalLines)
        {
            int[] Data = new int[Width * Height];
            Imaging.GetPixelArrayInteger(AreaId, ref Data, 0, 0, Width, Height);
            bool[] Ret = new bool[Height];
            VerticalLines = new bool[Width];
            for (int i = 0; i < Height; i++) {
                Ret[i] = true;
                for (int j = 0; j < Width; j++) {
                    if (Data[i * Width + j] != -1) {
                        Ret[i] = false;
                        break;
                    }
                }
            }
            for (int i = 0; i < Width; i++) {
                VerticalLines[i] = true;
                for (int j = 0; j < Height; j++) {
                    if (Data[i + j * Width] != -1) {
                        VerticalLines[i] = false;
                        break;
                    }
                }
            }
            return Ret;
        }
        private void AnalyzeLines(bool[] LinesBW, out int TopOffset, out int BottomOffset, bool Is2Dots)
        {
            TopOffset = 0;
            BottomOffset = 0;
            int Height = LinesBW.Length;
            List<KeyVal<int, int>> BlackAreas = new List<KeyVal<int, int>>();
            bool Added = false;

            for (int i = 0; i < LinesBW.Length; i++) {
                if (!LinesBW[i]) {
                    if (!Added) {
                        KeyVal<int, int> BlackArea = new KeyVal<int, int>(i, 1);
                        BlackAreas.Add(BlackArea);
                        Added = true;
                    } else {
                        BlackAreas.Last().Value++;
                    }
                } else {
                    Added = false;
                }
            }
            if (BlackAreas.Count < 1) {
                return;
            }

            TopOffset = BlackAreas[0].Id - 1;
            BottomOffset = Height - (BlackAreas.Last().Id + BlackAreas.Last().Value);
            if (Is2Dots) {
                return;
            }
            if (BlackAreas.Count > 1) {
                //BlackAreas.Sort((x, y) => y.Value.CompareTo(x.Value));
                var Sorted = BlackAreas.OrderByDescending(x => x.Value).ToList();
                var BiggestAreaIndex = BlackAreas.IndexOf(Sorted.First());

                if (IsSameHeight(Sorted.First().Value, Sorted[1].Value, 1.5)) {
                    /*TopOffset = Sorted.First().Id - 1;
                    BottomOffset = Height - (Sorted.First().Id + Sorted.First().Value);*/
                    var SecondAreaIndex = BlackAreas.IndexOf(Sorted[1]);
                    var Lowest = Math.Max(BiggestAreaIndex, SecondAreaIndex);
                    TopOffset = BlackAreas[Lowest].Id - 1;
                    BottomOffset = Height - (BlackAreas[Lowest].Id + BlackAreas[Lowest].Value);
                    /*}
                    if (Sorted.First().Value > Sorted[1].Value * 1.35) {
                        TopOffset = Sorted.First().Id - 1;
                        BottomOffset = Height - (Sorted.First().Id + Sorted.First().Value);
                        if (BiggestAreaIndex > 0) {
                            var PrevAreaHigh = BlackAreas[BiggestAreaIndex - 1].Value;
                            if (Sorted.First().Id - (BlackAreas[BiggestAreaIndex - 1].Id + BlackAreas[BiggestAreaIndex - 1].Value) <= PrevAreaHigh) {
                                TopOffset = BlackAreas[BiggestAreaIndex - 1].Id - 1;
                            }
                        }*/
                } else {
                    TopOffset = Sorted.First().Id - 1;
                    BottomOffset = Height - (Sorted.First().Id + Sorted.First().Value);
                    //pick bottom line
                    /*var SecondAreaIndex = BlackAreas.IndexOf(Sorted[1]);
                    var Lowest = Math.Max(BiggestAreaIndex, SecondAreaIndex);
                    TopOffset = BlackAreas[Lowest].Id - 1;
                    BottomOffset = Height - (BlackAreas[Lowest].Id + BlackAreas[Lowest].Value);*/
                }
            }
        }
        private void AnalyzeVerticalLines(bool[] LinesVertical, out int LeftOffset, out int RightOffset)
        {
            LeftOffset = 0;
            RightOffset = 0;
            for (int i = 0; i < LinesVertical.Length; i++) {
                if (!LinesVertical[i]) {
                    break;
                }
                LeftOffset++;
            }
            for (int i = LinesVertical.Length - 1; i >= 0; i--) {
                if (!LinesVertical[i]) {
                    break;
                }
                RightOffset++;
            }
        }
        private bool IsSameHeight(int Height1, int Height2, double AllowedMaxRatio)
        {
            double Ratio1 = (double)Height1 / (double)Height2;
            double Ratio2 = 1 / Ratio1;
            double Ratio = Math.Max(Ratio1, Ratio2);
            return (Ratio <= AllowedMaxRatio);
        }
        private void FillPageText(string PageText, GdPicturePDF SourcePDF, Page Page)
        {
            GdPictureImaging GdPictureImaging = new GdPictureImaging();
            var Words = PageText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            SourcePDF.SetOrigin(PdfOrigin.PdfOriginTopLeft);
            SourcePDF.SetMeasurementUnit(PdfMeasurementUnit.PdfMeasurementUnitPoint);
            for (int i = 0; i < Words.Length; i++) {
                string WordSet = Words[i];
                try {
                    var Coords = WordSet.Split(SEPARATOR[0]);
                    var Word = CorrectWord(Coords[8]);
                    if (!string.IsNullOrEmpty(Word) && !string.IsNullOrWhiteSpace(Word)) {
                        E_TextOrientation Orientation;
                        System.Windows.Rect Rect = GetRect(Coords, out Orientation);
                        if (Rect.IsEmpty) {
                            continue;
                        }
                        //var asdf = SourcePDF.GetPageTextArea(0, 0, 500, 500);
                        //var AltText = SourcePDF.GetPageTextArea((float)(Rect.Left / Scale / 72.0), (float)(Rect.Top / Scale / 72.0), (float)(Rect.Width / Scale / 72.0), (float)(Rect.Height / Scale / 72.0));
                        TextArea Area = new TextArea(Rect, Word, Page);
                        Area.Orientation = Orientation;
                        //SetPDFFontSize(SourcePDF, Area, Fonts);
                        if (Orientation == E_TextOrientation.LeftRight) {
                            Page.TextAreas.Add(Area);
                            FillCharParams(Area);
                        }
                    }
                } catch (Exception ex) {
                    throw ex;
                }
            }
            int ID = GdPictureImaging.CreateNewGdPictureImage((int)Page.Width, (int)Page.Height, 24, System.Drawing.Color.White);
            foreach (var item in Page.TextAreas) {
                //calc sizes
                var FontSize = GetFontSize(GdPictureImaging, ID, item, "Arial");
                /*if (item.Text == "-") {
                    var a = item.FontSize;
                }*/
            }
            GdPictureImaging.ReleaseGdPictureImage(ID);
        }
        private string CorrectWord(string Word)
        {
            Word = Word.Trim();
            StringBuilder Sb = new StringBuilder(Word.Length);
            for (int i = 0; i < Word.Length; i++) {
                if (Word[i] > 255) {
                    //need some correction
                    switch (Word[i]) {
                        case (char)65533:
                            //Sb.Append("#");
                            break;
                        case (char)63710:
                        case (char)10095:
                            Sb.Append(">");
                            break;
                        case (char)63281:
                            Sb.Append("1");
                            break;
                        case (char)63282:
                            Sb.Append("2");
                            break;
                        case (char)63283:
                            Sb.Append("3");
                            break;
                        case (char)63284:
                            Sb.Append("4");
                            break;
                        case (char)63285:
                            Sb.Append("5");
                            break;
                        case (char)63286:
                            Sb.Append("6");
                            break;
                        case (char)63287:
                            Sb.Append("7");
                            break;
                        case (char)64256:
                            Sb.Append("ff");
                            break;
                        case (char)64257:
                            Sb.Append("fi");
                            break;
                        case (char)64258:
                            Sb.Append("fl");
                            break;
                        case (char)8364:
                            //Sb.Append("EUR");
                            Sb.Append(EURO_CHAR);
                            break;
                        case (char)8208:
                        case (char)8209:
                        case (char)8211:
                        case (char)8212:
                        case (char)8722:
                            Sb.Append("-");
                            break;
                        case (char)8216:
                        case (char)8217:
                            Sb.Append("'");
                            break;
                        case (char)8220:
                        case (char)8221:
                            Sb.Append("\"");
                            break;
                        case (char)8226:
                        case (char)61623:
                            Sb.Append("*");
                            break;
                        case (char)8230:
                            Sb.Append("...");
                            break;
                        case (char)8194:
                        case (char)8195:
                        case (char)9632:
                        case (char)65535:
                            Sb.Append(" ");
                            break;
                        default:
                            break;
                    }
                } else if (Word[i] == 164) { //euro
                    //Sb.Append(EURO_CHAR"EUR");
                    Sb.Append(EURO_CHAR);
                } else if (Word[i] == 173) { //minus sign
                    Sb.Append("-");
                } else {
                    Sb.Append(Word[i]);
                }
            }
            //here we drop out words that is basicly lines made of undescore set of symbols.
            if (Word.Length > 3 && Word.Contains("_") && string.IsNullOrEmpty(Word.Replace("_", ""))) {
                return null;
            }
            return Sb.ToString();
        }
        private System.Windows.Rect GetRect(string[] Coords, out E_TextOrientation Orientation)
        {
            Orientation = E_TextOrientation.Unknown;
            double TopLeftX = double.Parse(Coords[0]);
            double TopLeftY = double.Parse(Coords[1]);
            double TopRightX = double.Parse(Coords[2]);
            double TopRightY = double.Parse(Coords[3]);
            double BottomRightX = double.Parse(Coords[4]);
            double BottomRightY = double.Parse(Coords[5]);
            double BottomLeftX = double.Parse(Coords[6]);
            double BottomLeftY = double.Parse(Coords[7]);

            //vertical
            if (TopLeftX == TopRightX && BottomLeftX == BottomRightX && TopLeftY == BottomLeftY && TopRightY == BottomRightY) {
                if (TopRightX < BottomRightX && TopLeftX < BottomLeftX) {
                    Orientation = E_TextOrientation.BottomTop;
                } else {
                    Orientation = E_TextOrientation.TopBottom;
                }
            }
            //horizontal
            if (TopLeftY == TopRightY && BottomLeftY == BottomRightY) {// && TopLeftX == BottomLeftX && TopRightX == BottomRightX) {
                if (TopLeftY < BottomLeftY && TopRightY < BottomRightY) {
                    Orientation = E_TextOrientation.LeftRight;
                } else {
                    Orientation = E_TextOrientation.RightLeft;
                }
            }

            System.Windows.Rect Rect = new System.Windows.Rect();
            Rect.X = Math.Min(Math.Min(TopLeftX, TopRightX), Math.Min(BottomLeftX, BottomRightX)) * Scale;
            Rect.Y = Math.Min(Math.Min(TopLeftY, TopRightY), Math.Min(BottomLeftY, BottomRightY)) * Scale;
            Rect.Width = Math.Max(Math.Max(TopLeftX, TopRightX), Math.Max(BottomLeftX, BottomRightX)) * Scale - Rect.X;
            Rect.Height = Math.Max(Math.Max(TopLeftY, TopRightY), Math.Max(BottomLeftY, BottomRightY)) * Scale - Rect.Y;
            if (Rect.X < 0 || Rect.Y < 0) {
                return System.Windows.Rect.Empty;
            }
            return Rect;
        }
        private float GetFontSize(GdPictureImaging GdPictureImaging, int ID, TextArea Text, string FontName)
        {
            var Height = GdPictureImaging.GetTextHeight(ID, Text.Text, FontName, 50, GdPicture9.FontStyle.FontStyleRegular);
            var Width = GdPictureImaging.GetTextWidth(ID, Text.Text, FontName, 50, GdPicture9.FontStyle.FontStyleRegular);

            var Ratio1 = Height / Text.Height;
            Text.FontSize = 50 / Ratio1;
            var Ratio2 = Width / Text.Width;
            return (float)(50 / Math.Max(Ratio1, Ratio2));
        }
        private void FillCharParams(TextArea Area)
        {
            Rect[] Chars = new System.Windows.Rect[Area.Text.Length];
            double SymbolWidth = Area.Width / Area.Text.Length;
            double Offset = 0;
            for (int i = 0; i < Chars.Length; i++) {
                Offset = i * SymbolWidth;
                Chars[i] = new System.Windows.Rect(
                        Area.Bound.Left + Offset,
                        Area.Bound.Top,
                        SymbolWidth,
                        Area.Bound.Height);
            }
            Area.SetCharProperties(Chars);
        }
        #endregion
    }
}
