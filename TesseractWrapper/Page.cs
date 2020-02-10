﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TesseractWrapper.Internal;

namespace TesseractWrapper
{
    public sealed class Page : DisposableBase
    {
        private static readonly TraceSource trace = new TraceSource("Tesseract");

        private bool runRecognitionPhase;
        private Rect regionOfInterest;

        public TesseractEngine Engine { get; private set; }

        /// <summary>
        /// Gets the <see cref="Pix"/> that is being ocr'd.
        /// </summary>
        public Pix Image { get; private set; }

        /// <summary>
        /// Gets the name of the image being ocr'd.
        /// </summary>
        /// <remarks>
        /// This is also used for some of the more advanced functionality such as identifying the associated UZN file if present.
        /// </remarks>
        public string ImageName { get; private set; }

        /// <summary>
        /// Gets the page segmentation mode used to OCR the specified image.
        /// </summary>
        public PageSegMode PageSegmentMode { get; private set; }

        internal Page(TesseractEngine engine, Pix image, string imageName, Rect regionOfInterest, PageSegMode pageSegmentMode)
        {
            Engine = engine;
            Image = image;
            ImageName = imageName;
            RegionOfInterest = regionOfInterest;
            PageSegmentMode = pageSegmentMode;
        }

        /// <summary>
        /// The current region of interest being parsed.
        /// </summary>
        public Rect RegionOfInterest
        {
            get
            {
                return regionOfInterest;
            }
            set
            {
                if (value.X1 < 0 || value.Y1 < 0 || value.X2 > Image.Width || value.Y2 > Image.Height)
                    throw new ArgumentException("The region of interest to be processed must be within the image bounds.", "value");

                if (regionOfInterest != value) {
                    regionOfInterest = value;

                    // update region of interest in image
                    Interop.TessApi.BaseApiSetRectangle(Engine.Handle, regionOfInterest.X1, regionOfInterest.Y1, regionOfInterest.Width, regionOfInterest.Height);

                    // request rerun of recognition on the next call that requires recognition
                    runRecognitionPhase = false;
                }
            }
        }

        /// <summary>
        /// Gets the thresholded image that was OCR'd.
        /// </summary>
        /// <returns></returns>
        public Pix GetThresholdedImage()
        {
            Recognize();

            var pixHandle = Interop.TessApi.BaseAPIGetThresholdedImage(Engine.Handle);
            if (pixHandle == IntPtr.Zero) {
                throw new TesseractException("Failed to get thresholded image.");
            }

            return Pix.Create(pixHandle);
        }

        /// <summary>
        /// Creates a <see cref="PageIterator"/> object that is used to iterate over the page's layout as defined by the current <see cref="Page.RegionOfInterest"/>.
        /// </summary>
        /// <returns></returns>
        public PageIterator AnalyseLayout()
        {
            Guard.Verify(PageSegmentMode != PageSegMode.OsdOnly, "Cannot analyse image layout when using OSD only page segmentation, please use DetectBestOrientation instead.");

            var resultIteratorHandle = Interop.TessApi.BaseAPIAnalyseLayout(Engine.Handle);
            return new PageIterator(this, resultIteratorHandle);
        }

        /// <summary>
        /// Creates a <see cref="ResultIterator"/> object that is used to iterate over the page as defined by the current <see cref="Page.RegionOfInterest"/>.
        /// </summary>
        /// <returns></returns>
        public ResultIterator GetIterator()
        {
            Recognize();
            var resultIteratorHandle = Interop.TessApi.BaseApiGetIterator(Engine.Handle);
            return new ResultIterator(this, resultIteratorHandle);
        }

        /// <summary>
        /// Gets the page's content as plain text.
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            Recognize();
            return Interop.MarshalHelper.PtrToString(Interop.TessApi.BaseAPIGetUTF8Text(Engine.Handle), Encoding.UTF8);
        }

        /// <summary>
        /// Gets the page's content as a HOCR text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <param name="useXHtml">True to use XHTML Output, False to HTML Output</param>
        /// <returns>The OCR'd output as a HOCR text string.</returns>
        public string GetHOCRText(int pageNum, bool useXHtml = false)
        {
            //Why Not Use 'nameof(pageNum)' instead of '"pageNum"'
            Guard.Require("pageNum", pageNum >= 0, "Page number must be greater than or equal to zero (0).");
            Recognize();
            /*if(useXHtml)
                return Interop.TessApi.BaseAPIGetHOCRText2(Engine.Handle, pageNum);
            else*/
            return Interop.MarshalHelper.PtrToString(Interop.TessApi.BaseAPIGetHOCRText(Engine.Handle, pageNum), Encoding.UTF8);
        }

        /// <summary>
        /// Get's the mean confidence that as a percentage of the recognized text.
        /// </summary>
        /// <returns></returns>
        public float GetMeanConfidence()
        {
            Recognize();
            return Interop.TessApi.BaseApiMeanTextConf(Engine.Handle) / 100.0f;
        }

        /// <summary>
        /// Detects the page orientation, with corresponding confidence when using <see cref="PageSegMode.OsdOnly"/>.
        /// </summary>
        /// <remarks>
        /// If using full page segmentation mode (i.e. AutoOsd) then consider using <see cref="AnalyseLayout"/> instead as this also provides a
        /// deskew angle which isn't available when just performing orientation detection.
        /// </remarks>
        /// <param name="orientation">The page orientation.</param>
        /// <param name="confidence">The confidence level of the orientation (15 is reasonably confident).</param>
        /// 
        public void DetectBestOrientation(out Orientation orientation, out float confidence)
        {
            Interop.OSResult result = new Interop.OSResult();
            result.Init();
            if (Interop.TessApi.BaseAPIDetectOS(Engine.Handle, ref result) != 0)
            {
                result.GetBestOrientation(out orientation, out confidence);
            }
            else
            {
                throw new TesseractException("Failed to detect image orientation.");
            }
        }
        public void Recognize()
        {
            Guard.Verify(PageSegmentMode != PageSegMode.OsdOnly, "Cannot OCR image when using OSD only page segmentation, please use DetectBestOrientation instead.");
            if (!runRecognitionPhase) {
                if (Interop.TessApi.BaseApiRecognize(Engine.Handle, new HandleRef(this, IntPtr.Zero)) != 0) {
                    throw new InvalidOperationException("Recognition of image failed.");
                }

                runRecognitionPhase = true;

                // now write out the thresholded image if required to do so
                bool tesseditWriteImages;
                if (Engine.TryGetBoolVariable("tessedit_write_images", out tesseditWriteImages) && tesseditWriteImages) {
                    using (Pix thresholdedImage = GetThresholdedImage()) {
                        string filePath = Path.Combine(Environment.CurrentDirectory, "tessinput.tif");
                        try {
                            thresholdedImage.Save(filePath, ImageFormat.TiffG4);
                            trace.TraceEvent(TraceEventType.Information, 2, "Successfully saved the thresholded image to '{0}'", filePath);
                        } catch (Exception error) {
                            trace.TraceEvent(TraceEventType.Error, 2, "Failed to save the thresholded image to '{0}'.\nError: {1}", filePath, error.Message);
                        }
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                Interop.TessApi.BaseAPIClear(Engine.Handle);
            }
        }
    }
}