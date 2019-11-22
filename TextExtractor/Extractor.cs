using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATAPY.Common.IO;
using ATAPY.Document.Data.Core;
using GdPicture9;

namespace TextExtractor
{
    public class Extractor : IExtractor
    {
        bool withTL;
        private List<string> AllowedImageFormats = new List<string>();

        public Extractor()
        {
            AllowedImageFormats.Add(".jpg");
            AllowedImageFormats.Add(".jpeg");
            AllowedImageFormats.Add(".gif");
            AllowedImageFormats.Add(".png");
            AllowedImageFormats.Add(".bmp");
            AllowedImageFormats.Add(".tif");
            AllowedImageFormats.Add(".tiff");

            RegisterGdp();
        }

        private void RegisterGdp()
        {
            LicenseManager _manager = new LicenseManager();
            _manager.RegisterKEY("411807659822389691114151231799986");
            _manager.RegisterKEY("912167958713519651119121587784646");

        }

        private void CheckFormat(File file)
        {
            var extension = System.IO.Path.GetExtension(file.FullPath);
            if (AllowedImageFormats.Contains(extension))
            {
                withTL = false;
                return;
            }
        }

        public Document GetDocument(File file)
        {
            return GetDocument(file, "eng");
        }

        public Document GetDocument(File file, string language)
        {
            CheckFormat(file);
            var extension = System.IO.Path.GetExtension(file.FullPath);
            if (AllowedImageFormats.Contains(extension))
            {
                var extractor = new ImagesExtractor();
                return extractor.GetDocument(file, language);
            }
            else if (extension == ".pdf")
            {
                
                if (IsDigitalDocument(file))
                {
                    var extractor = new DigitalPdfExtractor();
                    return extractor.GetDocument(file);
                }
                else
                {
                    var tiff = Converter.GetTiff(file);
                    var extractor = new ImagesExtractor();
                    return extractor.GetDocument(tiff, language);
                }
            }
            else
                throw new ArgumentException("Unsupported image format!");
        }
        private bool IsDigitalDocument(File file)
        {
            using (var pdf = new GdPicturePDF())
            {
                var status = pdf.LoadFromFile(file.FullPath, false);
                if (status != GdPictureStatus.OK)
                    throw new ArgumentException(status.ToString());
                status = pdf.SelectPage(1);
                if (status != GdPictureStatus.OK)
                    throw new ArgumentException(status.ToString());
                return pdf.PageHasText();
            }
        }

    }
}
