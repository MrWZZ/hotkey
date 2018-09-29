using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MyTool.Script
{
    class LockKeyManager
    {
        public delegate int HookProc(int nCode, int wParam, IntPtr lParam);
        public delegate void GetCombineKeyHandle(KeysInfo keyCombine);
        /// <summary>
        /// 得到组合键
        /// </summary>
        public event GetCombineKeyHandle GetCombineKeyEvent;

        static int hHook = 0;
        public const int WH_KEYBOARD_LL = 13;

        //功能键值列表
        private List<uint> lockKey = new List<uint>();
        //普通键值
        private uint commonKey = 0;

        //LowLevel键盘截获，如果是WH_KEYBOARD＝2，并不能对系统键盘截取，Acrobat Reader会在你截取之前获得键盘。  
        HookProc KeyBoardHookProcedure;

        //键盘Hook结构函数  
        [StructLayout(LayoutKind.Sequential)]
        public class KeyBoardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        //设置钩子  
        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //抽掉钩子  
        public static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32.dll")]
        //调用下一个钩子  
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        public void Hook_Start()
        {
            // 安装键盘钩子  
            if (hHook == 0)
            {
                KeyBoardHookProcedure = new HookProc(KeyBoardHookProc);

                hHook = SetWindowsHookEx(WH_KEYBOARD_LL,
                KeyBoardHookProcedure,
                GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);

                //如果设置钩子失败.  
                if (hHook == 0)
                {
                    Hook_Clear();
                }
            }
        }

        //取消钩子事件  
        public void Hook_Clear()
        {
            lockKey.Clear();
            commonKey = 0;

            bool retKeyboard = true;
            if (hHook != 0)
            {
                retKeyboard = UnhookWindowsHookEx(hHook);
                hHook = 0;
            }
            //如果去掉钩子失败.  
            if (!retKeyboard) throw new Exception("UnhookWindowsHookEx failed.");
        }

        public int KeyBoardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                //键盘弹起取消钩子
                if (wParam == 257)
                {
                    GetCombineKeyEvent(new KeysInfo(this.lockKey,this.commonKey));
                    Hook_Clear();
                }
                else
                {
                    KeyBoardHookStruct kbh = (KeyBoardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyBoardHookStruct));
                    switch (kbh.vkCode)
                    {
                        case 160://LeftShift
                        case 161://RightShift
                        case 162://LeftCtrl
                        case 163://RightCtrl
                        case 164://LeftAlt
                        case 165://RightAlt
                        case 91://LeftWin
                        case 92://RightWin
                            if (!lockKey.Contains((uint)kbh.vkCode))
                            {
                                lockKey.Add((uint)kbh.vkCode);
                            }
                            return 1;
                        default:
                            commonKey = (uint)kbh.vkCode;
                            break;
                    }
                }
            }
            return CallNextHookEx(hHook, nCode, wParam, lParam);
        }
    }
}
