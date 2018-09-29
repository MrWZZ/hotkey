using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows;

namespace MyTool.Script
{
    public class Register
    {
        private IntPtr hWnd;
        private HwndSource source;

        [DllImport("user32.dll")]
        public static extern uint RegisterHotKey(IntPtr hWnd, uint id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern uint UnregisterHotKey(IntPtr hWnd, uint id);

        [DllImport("kernel32.dll")]
        public static extern uint GlobalAddAtom(String lpString);

        [DllImport("kernel32.dll")]
        public static extern uint GlobalDeleteAtom(uint nAtom);

        public Register(Window window)
        {
            hWnd = new WindowInteropHelper(window).Handle;
            source = PresentationSource.FromVisual(window) as HwndSource;
            source.AddHook(Listener);
        }

        /// <summary>
        /// 注册快捷键
        /// </summary>
        /// <param name="key">实体键</param>
        /// <param name="keyflag">功能键</param>
        /// <param name="hotkeyid">该快捷键对应的全局ID，用于判断用户当前按下的是什么组合键</param>
        /// <returns>是否注册成功</returns>
        public uint RegisterHotkey(uint key, uint keyflag,out uint hotkeyid)
        {
            hotkeyid = GlobalAddAtom(Guid.NewGuid().ToString());
            return RegisterHotKey(hWnd, hotkeyid, keyflag, key);
        }

        /// <summary>
        /// 注销快捷键
        /// </summary>
        /// <param name="key">快捷键对应的唯一ID</param>
        /// <returns>是否删除成功</returns>
        public uint UnregisterHotkey(uint hotkeyid)
        {
            GlobalDeleteAtom(hotkeyid);
            return UnregisterHotKey(hWnd, hotkeyid);
        }

        public void Destory()
        {
            source.RemoveHook(Listener);
        }

        /// <summary>
        /// 该窗口绑定的快捷键触发后的系统响应
        /// </summary>
        public IntPtr Listener(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handle)
        {
            uint hotkeyid = (uint)wParam.ToInt32();
            AllControl.CallFunction(hotkeyid);
            return IntPtr.Zero;
        }
    }
}

