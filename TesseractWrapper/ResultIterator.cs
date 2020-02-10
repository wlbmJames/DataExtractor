﻿using System;
using System.Collections.Generic;
using System.Text;

using TesseractWrapper.Interop;

namespace TesseractWrapper
{
    public sealed class ResultIterator : PageIterator
    {
        internal ResultIterator(Page page, IntPtr handle)
            : base(page, handle)
        {
        }

        public float GetConfidence(PageIteratorLevel level)
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero)
                return 0f;

            return Interop.TessApi.ResultIteratorGetConfidence(handle, level);
        }

        public string GetText(PageIteratorLevel level)
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return String.Empty;
            }

            return MarshalHelper.PtrToString( TessApi.ResultIteratorGetUTF8Text(handle, level), Encoding.UTF8);       
        }
        
        private Dictionary<int, FontInfo> _fontInfoCache = new Dictionary<int, FontInfo>();
        //public FontAttributes GetWordFontAttributes() {
        //    VerifyNotDisposed();
        //    if (handle.Handle == IntPtr.Zero) {
        //        return null;
        //    }

        //    bool isBold, isItalic, isUnderlined, isMonospace, isSerif, isSmallCaps;
        //    int pointSize, fontId;

        //    // per docs (ltrresultiterator.h:104 as of 4897796 in github:tesseract-ocr/tesseract)
        //    // this return value points to an internal table and should not be deleted.
        //    IntPtr nameHandle =
        //        Interop.TessApi.ResultIteratorWordFontAttributes(
        //            handle,
        //            out isBold, out isItalic, out isUnderlined,
        //            out isMonospace, out isSerif, out isSmallCaps,
        //            out pointSize, out fontId);

        //    // This can happen in certain error conditions
        //    if (nameHandle == IntPtr.Zero) {
        //        return null;
        //    }

        //    FontInfo fontInfo;
        //    if (!_fontInfoCache.TryGetValue(fontId, out fontInfo)) {
        //        string fontName = MarshalHelper.PtrToString(nameHandle, Encoding.UTF8);
        //        fontInfo = new FontInfo(fontName, fontId, isItalic, isBold, isMonospace, isSerif);
        //        _fontInfoCache.Add(fontId, fontInfo);
        //    }

        //    return new FontAttributes(fontInfo, isUnderlined, isSmallCaps, pointSize);
        //}

        public string GetWordRecognitionLanguage()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return null;
            }

            return MarshalHelper.PtrToString(TessApi.ResultIteratorWordRecognitionLanguage(handle), Encoding.UTF8);
        }

        public bool GetWordIsFromDictionary()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }

            return Interop.TessApi.ResultIteratorWordIsFromDictionary(handle) == 0 ? false : true;
        }

        public bool GetWordIsNumeric()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }

            return Interop.TessApi.ResultIteratorWordIsNumeric(handle) == 0 ? false : true;
        }

        public bool GetSymbolIsSuperscript()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }

            return Interop.TessApi.ResultIteratorSymbolIsSuperscript(handle) == 0? false : true;
        }

        public bool GetSymbolIsSubscript()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }

            return Interop.TessApi.ResultIteratorSymbolIsSubscript(handle) == 0? false : true;
        }

        public bool GetSymbolIsDropcap()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }

            return Interop.TessApi.ResultIteratorSymbolIsDropcap(handle) == 0? false : true;
        }

        /// <summary>
        /// Gets an instance of a choice iterator using the current symbol of interest. The ChoiceIterator allows a one-shot iteration over the
        /// choices for this symbol and after that is is useless.
        /// </summary>
        /// <returns>an instance of a Choice Iterator</returns>
        public ChoiceIterator GetChoiceIterator()
        {
            var choiceIteratorHandle = Interop.TessApi.ResultIteratorGetChoiceIterator(this.handle);
            if (choiceIteratorHandle == IntPtr.Zero)
                return null;
            return new ChoiceIterator(choiceIteratorHandle);
        }
    }
}
