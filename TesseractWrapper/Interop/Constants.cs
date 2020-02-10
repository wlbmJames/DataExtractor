using System;
using System.Runtime.InteropServices;

namespace TesseractWrapper.Interop
{
    /// <summary>
    /// Description of Constants.
    /// </summary>
    internal static class Constants
    {
        //public const string LeptonicaDllName = "leptonica-1.74.4";
        public const string LeptonicaDllName = "leptonica174";
        //public const string TesseractDllName = "libtesseract-5";
        public const string TesseractDllName = "tesseract50";
        //public readonly static string LeptonicaDllName;
        //public readonly static string TesseractDllName;
        //static Constants()
        //{
        //    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        //    {
        //        TesseractDllName = "tesseract50";
        //    }
        //    else 
        //    {
        //        TesseractDllName = "libtesseract";
        //    }
        //    LeptonicaDllName = "leptonica174";
        //}
        // tesseract uses an int to represent true false values.
        public const int TRUE = 1;
        public const int FALSE = 0;
    }
}