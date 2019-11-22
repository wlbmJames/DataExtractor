using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using ATAPY.Common.IO;
using ATAPY.Document.Data.Core;
using TesseractWrapper;

namespace TextExtractor
{
    public class ImagesExtractor : IExtractor
    {
        public const double DEFAULT_RENDER_DPI = 300.0;
        private readonly List<string> AllowedFormats = new List<string>();
        private readonly List<string> AllowedMultiPageFormats = new List<string>();
        private const double SOURCE_DPI = 72.0;
        private const string ENGINE_DATAPATH = "tessdata";
        internal ImagesExtractor()
        {
            RenderDPI = DEFAULT_RENDER_DPI;
            this.PageImagesFolder = new ATAPY.Common.IO.Directory(System.IO.Path.GetTempPath());
            AllowedFormats.Add(".jpg");
            AllowedFormats.Add(".jpeg");
            AllowedFormats.Add(".gif");
            AllowedFormats.Add(".png");
            AllowedFormats.Add(".bmp");
            AllowedMultiPageFormats.Add(".tif");
            AllowedMultiPageFormats.Add(".tiff");
        }

        #region Public Area

        #region Properties
        public double RenderDPI
        {
            get;
            set;
        }
        public double Scale
        {
            get
            {
                return 1 / SOURCE_DPI * RenderDPI;
            }
        }
        public ATAPY.Common.IO.Directory PageImagesFolder
        {
            get;
            set;
        }
        #endregion

        #region Methods
        public Document GetDocument(ATAPY.Common.IO.File image, string language)
        {
            if (string.IsNullOrEmpty(language))
                throw new ArgumentException("No Languages specified!");

            var document = new ATAPY.Document.Data.Core.Document();
            document.Scale = Scale;
            document.SourceFormat = E_SourceFormat.HOCR;
            document.SourceFile = image.Clone();

            FillPagesData(document, image.FullPath, language);

            return document;
        }
        public Document GetDocument(ATAPY.Common.IO.File file)
        {
            return GetDocument(file, "eng");
        }

        #endregion

        #endregion

        #region Private Area
        private void FillPagesData(ATAPY.Document.Data.Core.Document document, string pathToImage, string language)
        {
            var extension = Path.GetExtension(pathToImage);
            using (var engine = new TesseractEngine(ENGINE_DATAPATH, language))
            {
                if (IsSinglePageImage(extension))
                {
                    Pix pageData = null;
                    try
                    {
                        pageData = Pix.LoadFromFile(pathToImage);
                        FillDocumentPage(document, language, engine, pageData);
                    }
                    finally
                    {
                        pageData?.Dispose();
                    }
                }
                else if (IsMultiPageImage(extension))
                {
                    PixArray pixes = null;
                    try
                    {
                        pixes = PixArray.LoadMultiPageTiffFromFile(pathToImage);
                        foreach (Pix pageData in pixes)
                        {
                            FillDocumentPage(document, language, engine, pageData);
                        }
                    }
                    finally
                    {
                        pixes?.Dispose();
                    }
                }
                else
                {
                    throw new FormatException("Please specify path to the image file");
                }
            }
        }

        private void FillDocumentPage(ATAPY.Document.Data.Core.Document document, string language, TesseractEngine engine, Pix pageData)
        {
            var page = new ATAPY.Document.Data.Core.Page();
            page.Bound = new System.Windows.Rect(0, 0, pageData.Width, pageData.Height);
            document.Pages.Add(page);
            GetPageData(engine, pageData, language, page);
            page.AnalyzeData();
        }

        private void GetPageData(TesseractEngine engine, Pix pageData, string language, ATAPY.Document.Data.Core.Page page)
        {
            ResultIterator resultIterator = null;
            try
            {

                using (var tessPage = engine.Process(pageData))
                {
                    tessPage.Recognize();
                    resultIterator = tessPage.GetIterator();
                    resultIterator.Begin();

                    do
                    {
                        var text = resultIterator.GetText(PageIteratorLevel.Word);
                        if (TextIsValid(text) && resultIterator.TryGetBoundingBox(PageIteratorLevel.Word, out var rect))
                        {
                            var rectW = GetRect(rect);
                            var area = new TextArea(rectW, text, page);
                            page.TextAreas.Add(area);
                            var chars = new System.Windows.Rect[text.Length];
                            int charIter = 0;
                            do
                            {
                                if (resultIterator.TryGetBoundingBox(PageIteratorLevel.Symbol, out var sRect)) ;
                                chars[charIter] = GetRect(sRect);
                                charIter++;
                            } while (resultIterator.Next(PageIteratorLevel.Word, PageIteratorLevel.Symbol));
                            area.SetCharProperties(chars);
                        }

                    } while (resultIterator.Next(PageIteratorLevel.Word));

                }
            }
            finally
            {
                resultIterator?.Dispose();
            }
            //return page;
        }

        private System.Windows.Rect GetRect(TesseractWrapper.Rect rect)
        {
            return new System.Windows.Rect(rect.X1, rect.Y1, rect.Width, rect.Height);
        }

        private bool TextIsValid(string text)
        {
            text = text.Trim();
            if (string.IsNullOrEmpty(text))
                return false;
            if (text == "\n" || text == "\r\n")
                return false;
            return true;
        }

        private bool IsSinglePageImage(string extension)
        {
            return AllowedFormats.Contains(extension);
        }
        private bool IsMultiPageImage(string extension)
        {
            return AllowedMultiPageFormats.Contains(extension);
        }

        #endregion Private Area

    }
}
