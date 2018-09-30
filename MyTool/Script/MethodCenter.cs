using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MyTool.Script
{
    public enum MethodName
    {
        OpenFile,
        OpenWindow,
        SetMainVisible,
        OpenDesk
    }

    public static class MethodCenter
    {
        public static Dictionary<MethodName, Action<string>> Methods;

        static MethodCenter()
        {
            Methods = new Dictionary<MethodName, Action<string>>();
            Methods.Add(MethodName.OpenFile, OpenFile);
            Methods.Add(MethodName.OpenWindow, OpenWindow);
            Methods.Add(MethodName.SetMainVisible, SetMainVisible);
            Methods.Add(MethodName.OpenDesk, OpenDesk);
        }

        //重置配置表
        public static void ResetConfig()
        {
            //手动给控件注册方法
            Item i0 = new Item("txtCmd_Open", MethodName.SetMainVisible, "true");
            AllControl.items.Add(i0.controlName, i0);
        }

        #region 方法实现
        // 打开指定文件
        public static void OpenFile(string arg)
        {
            if (File.Exists(arg))
            {
                System.Diagnostics.Process.Start(arg);
            }
            else if(Directory.Exists(arg))
            {
                System.Diagnostics.Process.Start("explorer.exe", arg);
            }
            else
            {
                WindowCenter.MainWindow.ShowNotify("路径未找到。");
            }
        }

        // 打开控制面板显示
        public static void OpenDesk(string arg)
        {
            System.Diagnostics.Process.Start("desk.cpl");
        }

        // 打开指定窗口
        public static void OpenWindow(string arg)
        {

        }

        //打开关闭主窗口
        public static void SetMainVisible(string arg)
        {
            WindowCenter.MainWindow.ShowWindow(bool.Parse(arg));
        }

        #endregion
    }
}
