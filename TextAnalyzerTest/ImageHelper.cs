using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.IO;

namespace TextAnalyzerTest
{
    public class ImageHelper: IDisposable
    {
        string _pathToImage;
        List<Bitmap> bitmaps;
        bool _bitmapsExtracted;
        public ImageHelper(string pathToimage)
        {
            _pathToImage = pathToimage;
            bitmaps = new List<Bitmap>();
            _bitmapsExtracted = false;
        }
        public void ExtractRegion(Rect rect)
        {
            ExtractBitmapsFromImage();
        }

        private void ExtractBitmapsFromImage()
        {
            using (var image = Image.FromFile(_pathToImage))
            {
                var guid = image.FrameDimensionsList[0];
                var dimension = new FrameDimension(guid);
                var pagesCount = image.GetFrameCount(dimension);
                for (int i = 0; i < pagesCount; i++)
                {
                    var ms = new MemoryStream();
                    image.SelectActiveFrame(dimension, i);
                    image.Save(ms, ImageFormat.Bmp);
                    var myBMP = new Bitmap(ms);
                    bitmaps.Add(myBMP);
                    ms.Close();
                }
                _bitmapsExtracted = true;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (bitmaps.Count != 0)
                        bitmaps.ForEach(b => b.Dispose());
                    bitmaps = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ImageHelper()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
