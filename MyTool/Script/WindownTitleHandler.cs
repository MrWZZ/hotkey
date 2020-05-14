using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyTool.Script
{
    public class WindownTitleHandler
    {
        [DllImport("user32.dll")]
        private extern static int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public static string GetWindowTitle(IntPtr hld)
        {
            StringBuilder s = new StringBuilder(512);
            int i = GetWindowText(hld, s, s.Capacity); //把this.handle换成你需要的句柄  
            return s.ToString();
        }
            
    }
}
