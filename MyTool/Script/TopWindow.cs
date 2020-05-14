using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace TopWindow
{
    class TopManager
    {
        //hWndInsertAfter参数可选值:
        public static readonly IntPtr HWND_TOP = new IntPtr(0); //将窗口置于Z序的顶部。
        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1); //将窗口置于Z序的底部。如果参数hWnd标识了一个顶层窗口，则窗口失去顶级位置，并且被置在其他窗口的底部。
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1); //将窗口置于所有非顶层窗口之上。即使窗口未被激活窗口也将保持顶级位置。
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2); //将窗口置于所有非顶层窗口之上（即在所有顶层窗口之后）。如果窗口已经是非顶层窗口则该标志不起作用。
        
        //uFlags 参数可选值:
        private const uint SWP_NOSIZE = 0x0001;    //不调整窗体大小
        private const uint SWP_NOMOVE = 0x0002;    //不调整窗体位置
        //SWP_NOZORDER = 4; {忽略 hWndInsertAfter, 保持 Z 顺序}
        //SWP_NOREDRAW = 8; {不重绘}
        //SWP_NOACTIVATE = $10; {不激活}
        //SWP_FRAMECHANGED = $20; {强制发送 WM_NCCALCSIZE 消息, 一般只是在改变大小时才发送此消息}
        //SWP_SHOWWINDOW = $40; {显示窗口}
        //SWP_HIDEWINDOW = $80; {隐藏窗口}
        //SWP_NOCOPYBITS = $100; {丢弃客户区}
        //SWP_NOOWNERZORDER = $200; {忽略 hWndInsertAfter, 不改变 Z 序列的所有者}
        //SWP_NOSENDCHANGING = $400; {不发出 WM_WINDOWPOSCHANGING 消息}
        //SWP_DRAWFRAME = SWP_FRAMECHANGED; {画边框}
        //SWP_NOREPOSITION = SWP_NOOWNERZORDER;{}
        //SWP_DEFERERASE = $2000; {防止产生 WM_SYNCPAINT 消息}
        //SWP_ASYNCWINDOWPOS = $4000; {若调用进程不拥有窗口, 系统会向拥有窗口的线程发出需求}

        /// <summary>
        /// 设置窗口位置
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="hWndInsertAfter">窗口的 Z 顺序</param>
        /// <param name="x">{位置}</param>
        /// <param name="y">{位置}</param>
        /// <param name="cx">{大小}</param>
        /// <param name="cy">{大小}</param>
        /// <param name="uFlags">参数可选值</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

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

        /// <summary>
        /// 设置窗口层级的位置
        /// </summary>
        /// <param name="handle"></param>
        public static bool SetZPos(IntPtr handle,IntPtr topType)
        {
            if (handle != null)
            {
                return SetWindowPos(handle, topType, 0, 0, 0, 0, TopManager.SWP_NOMOVE | TopManager.SWP_NOSIZE);
            }
            return false;
        }
    }
}
