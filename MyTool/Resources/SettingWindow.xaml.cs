﻿using System.Text;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MyTool.Script;
using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace MyTool
{
    /// <summary>
    /// WindowSetting.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        LockKeyManager LockKeyManager = new LockKeyManager();

        public SettingWindow()
        {
            InitializeComponent();
            LockKeyManager.GetCombineKeyEvent += LockKeyManager_GetCombineKeyEvent;
            SetControlName();
            AddAllItemListBox();
            Closed += SettingWindow_Closed;

            if (AllControl.isAutoStartUp)
            {
                cbAuto.IsChecked = true;
            }
            else
            {
                cbAuto.IsChecked = false;
            }
        }

        private void SettingWindow_Closed(object sender, EventArgs e)
        {
            WindowCenter.SettingWindow = null;
        }

        //监听组合键返回了什么信息
        private void LockKeyManager_GetCombineKeyEvent(KeysInfo keysInfo)
        {
            //按下ESC键注销该控件的快捷键
            if(keysInfo.key == 27)
            {
                uint removeResult = AllControl.RemoveHotKeyItem(AllControl.CurrControl.Name);
                if(removeResult == 1)
                {
                    ShowMassage("注销成功。");
                }
                else
                {
                    ShowMassage("注销失败。");
                }
                ((TextBox)AllControl.CurrControl).Text = "";
                return;
            }

            //准备注册热键
            if (keysInfo.key == 0 || keysInfo.lockKey.Count == 0)
            {
                ShowMassage("缺少实体键或功能键");
                return;
            }
            if (keysInfo.isWin)
            {
                ShowMassage("不能包含Win键。");
                return;
            }

            ((TextBox)AllControl.CurrControl).Text = keysInfo.keyName;
            // 注册热键
            uint result = AllControl.AddHotkeyItem(AllControl.CurrControl.Name,keysInfo.key,keysInfo.keyFlag,keysInfo.keyName);
            if(result == 0)
            {
                ShowMassage("注册失败。");
            }
            else
            {
                ShowMassage("注册成功");
            }

        }

        //在下面显示提示消息
        public void ShowMassage(string msg)
        {
            this.txtMsg.Text = msg;
        }
        //清除消息
        public void ClearMassage()
        {
            this.txtMsg.Text = "";
        }

        //显示当前控件的提示样式
        private void ShowFous(Control control,bool isShow)
        {
            if(isShow)
                control.Background = new SolidColorBrush(Colors.AliceBlue);
            else
                control.Background = new SolidColorBrush(Colors.White);
        }

        //给当前页面的控件显示已快捷键名
        private void SetControlName()
        {
            foreach(var name in AllControl.items.Keys)
            {
                var tb = (TextBox)FindName(name);
                if(tb != null)
                {
                    tb.Text = AllControl.items[name].keyName;
                }
            }
            
        }

        //在快捷列表中添加Item
        public void AddItemInListBox(HotPathInfo info)
        {
            lbItem.Items.Add(new HotPathItem(info));
        }

        //读取配置全部加载进去
        public void AddAllItemListBox()
        {
            foreach (var item in AllControl.hotpathId.Values)
            {
                AddItemInListBox(item);
            }
        }

        //移除控件
        public void removeListBoxItem(HotPathItem item)
        {
            lbItem.Items.Remove(item);
            AllControl.RemoveHotPathItem(item.txtID.Text);
        }

        private void Control_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowFous((Control)sender,true);
            AllControl.CurrControl = (Control)sender;
            ClearMassage();
            LockKeyManager.Hook_Start();
        }

        private void Control_MouseLeave(object sender, MouseEventArgs e)
        {
            ShowFous((Control)sender, false);
            AllControl.CurrControl = null;
            ClearMassage();
            LockKeyManager.Hook_Clear();
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            string fileName = "";
            try
            {
                fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            }
            catch
            {
                ShowMassage("无法识别该文件。");
                return;
            }
            
            //向listbox中添加
            //当前最大ID
            foreach (var item in AllControl.hotpathId.Keys)
            {
                int i = int.Parse(item);
                if(AllControl.canUseMinID <= i)
                {
                    AllControl.canUseMinID = i + 1;
                }
            }
            int index = AllControl.canUseMinID;
            AllControl.AddHotPathItem(index.ToString(), index + "-name", fileName);
            AddItemInListBox(AllControl.hotpathId[index.ToString()]);
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Link;
            else
                e.Effects = DragDropEffects.None;
        }

        private void cbAuto_Checked(object sender, RoutedEventArgs e)
        {
            bool isChecked = (bool)cbAuto.IsChecked;
            string name = Process.GetCurrentProcess().MainModule.FileName;
            bool isSuccess;
            if (isChecked) //设置开机自启动  
            {
                AllControl.isAutoStartUp = true;
                isSuccess = SetSelfStarting(true, name);
            }
            else //取消开机自启动  
            {
                AllControl.isAutoStartUp = false;
                isSuccess = SetSelfStarting(false, name);
            }

            if (isSuccess)
            {
                ShowMassage("设置成功。");
            }
            else
            {
                ShowMassage("设置失败");
            }
        }


        /// <summary>
        /// 开机自动启动
        /// </summary>
        /// <param name="started">设置开机启动，或取消开机启动</param>
        /// <param name="exeName">注册表中的名称</param>
        /// <returns>开启或停用是否成功</returns>
        public static bool SetSelfStarting(bool started, string exeName)
        {
            RegistryKey key = null;
            try
            {

                string exeDir = System.Windows.Forms.Application.ExecutablePath;
                key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//打开注册表子项

                if (key == null)//如果该项不存在的话，则创建该子项
                {
                    key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                }
                if (started)
                {
                    try
                    {
                        object ob = key.GetValue(exeName, -1);

                        if (!ob.ToString().Equals(exeDir))
                        {
                            if (!ob.ToString().Equals("-1"))
                            {
                                key.DeleteValue(exeName);//取消开机启动
                            }
                            key.SetValue(exeName, exeDir);//设置为开机启动
                        }
                        key.Close();

                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        key.DeleteValue(exeName);//取消开机启动
                        key.Close();
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                if (key != null)
                {
                    key.Close();
                }
                return false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbAuto.Click += cbAuto_Checked;
        }
    }
}
