using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ATAPY.Document.Data.Core
{
    public enum E_TextOrientation
    {
        Unknown,
        LeftRight,
        RightLeft,
        BottomTop,
        TopBottom
    }
    public class BasicElement
    {
        public Rect Bound
        {
            get;
            set;
        }
        public double Width
        {
            get
            {
                if (IsVertical) {
                    return this.Bound.Height;
                }
                return this.Bound.Width;
            }
        }
        public double Height
        {
            get
            {
                if (IsVertical) {
                    return this.Bound.Width;
                }
                return this.Bound.Height;
            }
        }
        public E_TextOrientation Orientation
        {
            get;
            set;
        }
        public bool IsVertical
        {
            get
            {
                return (this.Orientation == E_TextOrientation.BottomTop || this.Orientation == E_TextOrientation.TopBottom);
            }
        }

    }
}
