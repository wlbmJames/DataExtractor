using ATAPY.Common.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GdPicture9;


namespace TextExtractor
{
    internal static class Converter
    {
        public static File GetTiff(File file)
        {
            var filename = GetFileName();
            var compression = TiffCompression.TiffCompressionNONE;
            int dpi = 300;
            int tiffId = 0;
            var status = GdPictureStatus.OK;

            using (var pdf = new GdPicturePDF())
            {
                using (var imaging = new GdPictureImaging())
                {
                    if (pdf.LoadFromFile(file.FullPath, false) == GdPictureStatus.OK)
                    {
                        var pagesCount = pdf.GetPageCount();
                        for (int i = 1; i <= pagesCount; i++)
                        {
                            if (status == GdPictureStatus.OK)
                            {
                                pdf.SelectPage(i);
                                var pageId = pdf.RenderPageToGdPictureImageEx(dpi, true);
                                if (pageId == 0)
                                    throw new Exception($"Cannot converp page {i}");
                                if (i == 1)
                                {
                                    tiffId = pageId;
                                    status = imaging.TiffSaveAsMultiPageFile(tiffId, filename, compression);
                                }
                                else
                                {
                                    status = imaging.TiffAddToMultiPageFile(tiffId, pageId);
                                    imaging.ReleaseGdPictureImage(pageId);
                                }
                            }
                        }
                        if (imaging.TiffCloseMultiPageFile(tiffId) != GdPictureStatus.OK)
                            throw new Exception($"Cannot save file {filename}");
                        imaging.ReleaseGdPictureImage(tiffId);
                    }
                }
            }
            return new File(filename);
        }

        private static string GetFileName()
        {
            var tmpPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "P2PTemp");

            if (!System.IO.Directory.Exists(tmpPath))
                System.IO.Directory.CreateDirectory(tmpPath);
            var guid = Guid.NewGuid().ToString();
            var filename = guid + ".tif";
            return System.IO.Path.Combine(tmpPath, filename);
        }
    }
}
