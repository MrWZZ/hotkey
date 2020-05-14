using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyTool.Script
{
    public class MouseHookHandler
    {
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        static int hMouseHook = 0;
        private static HookProc MouseHookProcedure;//HookProc实例 

        //外部监听操作事件
        private static Action<MouseType> MouseEventHandler;
        
        //安装钩子
        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        // 卸载钩子 
        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(int idHook);
        // 继续下一个钩子 
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_MOUSEWHEEL = 0x020A;

        public enum MouseType
        {
            Move,
            LeftDown,
            RightDown,
            MiddleDown,
            LeftUp,
            RightUp,
            MiddleUp,
            Wheel
        }

        //钩子子程：就是钩子所要做的事情 
        private static int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSG m = (MSG)Marshal.PtrToStructure(lParam, typeof(MSG));//鼠标 
                MouseType type = MouseType.Move;
                switch(wParam)
                {
                    case WM_MOUSEMOVE:
                        type = MouseType.Move;
                        break;
                    case WM_LBUTTONDOWN:
                        type = MouseType.LeftDown;
                        break;
                    case WM_RBUTTONDOWN:
                        type = MouseType.RightDown;
                        break;
                    case WM_MBUTTONDOWN:
                        type = MouseType.MiddleDown;
                        break;
                    case WM_LBUTTONUP:
                        type = MouseType.LeftUp;
                        break;
                    case WM_RBUTTONUP:
                        type = MouseType.RightUp;
                        break;
                    case WM_MBUTTONUP:
                        type = MouseType.MiddleUp;
                        break;
                    case WM_MOUSEWHEEL:
                        type = MouseType.Wheel;
                        break;
                    default:
                        break;
                }

                MouseEventHandler?.Invoke(type);

                return 0;//如果返回1，则结束消息，这个消息到此为止，不再传递。如果返回0或调用CallNextHookEx函数则消息出了这个钩子继续往下传递，也就是传给消息真正的接受者 
            }
            return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
        }

        //鼠标结构 
        public struct MSG
        {
            public Point p; //鼠标坐标 
            public IntPtr HWnd;//鼠标点击的控件的句柄 
            public uint wHitTestCode;
            public int dwExtraInfo;
        }

        // 安装钩子(使用时只要调用此方法即可) 
        public static void HookStart(Action<MouseType> handler)
        {
            if (hMouseHook == 0)
            {
                // 创建HookProc实例 
                MouseHookProcedure = new HookProc(MouseHookProc);//参数为子程方法名 

                MouseEventHandler += handler;

                hMouseHook = SetWindowsHookEx(14, MouseHookProcedure, Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]), 0);// 鼠标全局钩子 

                // 如果设置钩子失败 
                if (hMouseHook == 0)
                {
                    HookStop();
                    throw new Exception("SetWindowsHookEx failed.");
                }
            }
        }

        // 卸载钩子(不需要时别忘了卸载啊,要不然系统速度会变慢哦) 
        public static void HookStop()
        {
            bool retMouse = true;
            if (hMouseHook != 0)
            {
                retMouse = UnhookWindowsHookEx(hMouseHook);
                MouseEventHandler -= MouseEventHandler;
                hMouseHook = 0;
            }
            if (!retMouse)
                throw new Exception("UnhookWindowsHookEx failed.");
        } 
    }
}
