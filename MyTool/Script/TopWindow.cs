using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace TopWindow
{
    class TopManager
    {
        public static int SW_SHOW = 5;
        public static int SW_NORMAL = 1;
        public static int SW_MAX = 3;
        public static int SW_HIDE = 0;
        public static readonly IntPtr HWND_TOP = new IntPtr(0);    //窗体置顶
        public const uint SWP_NOMOVE = 0x0002;    //不调整窗体位置
        public const uint SWP_NOSIZE = 0x0001;    //不调整窗体大小

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        public static extern bool ShowWindow(System.IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// 将置顶窗口置顶
        /// </summary>
        /// <param name="window"></param>
        public static void SetTop(Window window)
        {
            IntPtr handle = new WindowInteropHelper(window).Handle;
            if (handle != null)
            {
                SetWindowPos(handle, TopManager.HWND_TOP, 0, 0, 0, 0, TopManager.SWP_NOMOVE | TopManager.SWP_NOSIZE);
            }
        }
    }
}
