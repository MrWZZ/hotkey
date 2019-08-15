using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyTool.Script
{
    public class SelectWindowHandler
    {
        public struct POINT
        {
            int x;
            int y;
        }

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(int xPoint, int yPoint);

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        static extern bool SetWindowText(IntPtr hWnd, string lpString);

        public static POINT GetCursorPos()
        {
            POINT p;
            if (GetCursorPos(out p))
            {
                return p;
            }
            throw new Exception();
        }

        public static IntPtr WindowFromPoint()
        {
            POINT p = GetCursorPos();
            return WindowFromPoint(p);
        }
    }
}
