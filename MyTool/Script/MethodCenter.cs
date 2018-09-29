using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTool.Script
{
    public enum MethodName
    {
        OpenFile,
        OpenWindow,
        SetMainVisible
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
        }

        #region 方法实现
        // 打开指定文件
        public static void OpenFile(string arg = "")
        {

        }

        // 打开指定窗口
        public static void OpenWindow(string arg = "")
        {

        }

        //打开关闭主窗口
        public static void SetMainVisible(string arg = "")
        {
            WindowCenter.MainWindow.ShowWindow(bool.Parse(arg));
        }

        #endregion
    }
}
