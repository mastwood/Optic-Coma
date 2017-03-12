using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.InteropServices;

namespace Level_Editor
{
    class Win32ExternalInfrastructure
    {
        #region win32 externs and infrastructure
        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        /// <summary>
        /// Windows defined method that has been extracted from a system32 file
        /// </summary>
        /// <param name="hIcon"></param>
        /// <param name="pIconInfo"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        /// <summary>
        /// Windows defined method that has been extracted from a system32 file
        /// </summary>
        /// <param name="hIcon"></param>
        /// <param name="pIconInfo"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        /// <summary>
        /// Windows defined method that has been extracted from a system32 file
        /// </summary>
        /// <param name="hIcon"></param>
        /// <param name="pIconInfo"></param>
        /// <returns></returns>
        public static Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            IntPtr ptr = bmp.GetHicon();
            IconInfo tmp = new IconInfo();
            GetIconInfo(ptr, ref tmp);
            tmp.xHotspot = xHotSpot;
            tmp.yHotspot = yHotSpot;
            tmp.fIcon = false;
            ptr = CreateIconIndirect(ref tmp);
            return new Cursor(ptr);
        }
        #endregion
    }
}
