using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Collections.Generic;

namespace HotKey
{
    public class HotKeyManager
    {
        //快捷键方法
        private Dictionary<uint, Action> hotkeyMethods;
        //窗口句柄
        private IntPtr hWnd;
        private HwndSource source;

        public enum KeyFlags
        {
            MOD_ALT = 0x1,
            MOD_CONTROL = 0x2,
            MOD_SHIFT = 0x4,
            MOD_WIN = 0x8
        }

        [DllImport("user32.dll")]
        public static extern uint RegisterHotKey(IntPtr hWnd, uint id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern uint UnregisterHotKey(IntPtr hWnd, uint id);

        [DllImport("kernel32.dll")]
        public static extern uint GlobalAddAtom(String lpString);

        [DllImport("kernel32.dll")]
        public static extern uint GlobalDeleteAtom(uint nAtom);

        public HotKeyManager(System.Windows.Window window)
        {
            hWnd = new WindowInteropHelper(window).Handle;
            hotkeyMethods = new Dictionary<uint, Action>();
            source = System.Windows.PresentationSource.FromVisual(window) as HwndSource;
            source.AddHook(Listener);
        }

        /// <summary>
        /// 注册快捷键事件
        /// </summary>
        /// <param name="Key">键盘字符</param>
        /// <param name="keyflags">组合键标志</param>
        /// <param name="hotkeyMethod">快捷键事件</param>
        public uint RegisterHotkey(Keys Key, KeyFlags keyflags, Action hotkeyMethod)
        {
            uint hotkeyid = GlobalAddAtom(Guid.NewGuid().ToString());
            RegisterHotKey(hWnd, hotkeyid, (uint)keyflags, (uint)Key);
            hotkeyMethods.Add(hotkeyid, hotkeyMethod);
            return hotkeyid;
        }

        /// <summary>
        /// 注销指定事件
        /// </summary>
        /// <param name="key">事件的key</param>
        public void UnregisterHotkey(uint key)
        {
            if(hotkeyMethods.ContainsKey(key))
            {
                hotkeyMethods.Remove(key);
                UnregisterHotKey(hWnd, key);
                GlobalDeleteAtom(key);
            }
        }

        /// <summary>
        /// 注销所有事件
        /// </summary>
        public void Clear()
        {
            List<uint> keys = new List<uint>();
            keys.AddRange(hotkeyMethods.Keys);
            foreach (uint key in keys)
            {
                hotkeyMethods.Remove(key);
                UnregisterHotKey(hWnd, key);
                GlobalDeleteAtom(key);
            }
            source.RemoveHook(Listener);
        }

        /// <summary>
        /// 该窗口绑定的快捷键触发后的系统响应
        /// </summary>
        public IntPtr Listener(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handle)
        {
            uint key = (uint)wParam.ToInt32();
            if (hotkeyMethods.ContainsKey(key))
            {
                hotkeyMethods[key]();
            }
            return IntPtr.Zero;
        }
    }
}

