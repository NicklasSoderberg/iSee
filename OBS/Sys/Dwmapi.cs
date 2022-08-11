using System;
using System.Runtime.InteropServices;
using iSee.External.Sys.Data;

namespace iSee.External.Sys
{
    public static class Dwmapi
    {
        /// <summary>
        /// Extends the window frame into the client area.
        /// </summary>
        [DllImport("dwmapi.dll", SetLastError = true)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);
    }
}
