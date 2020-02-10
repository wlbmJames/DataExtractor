using System;
using System.Collections.Generic;
using System.Text;

namespace TesseractWrapper.Helpers
{
    internal static class BoolExtension
    {
        internal static Int32 ToInt32(this bool value)
        {
            return value ? 1 : 0;
        }
    }
}
