using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyTool.Script;
using Microsoft.Win32;
using System.Windows.Forms;
using System.IO;

namespace MyTool
{
    /// <summary>
    /// HotPathItem.xaml 的交互逻辑
    /// </summary>
    public partial class HotPathItem : System.Windows.Controls.UserControl
    {
        //当前控件的ID
        private HotPathInfo info;

        public HotPathItem(HotPathInfo info)
        {
            InitializeComponent();
            this.info = info;
            txtID.Text = info.ID;
            txtName.Text = info.fastName;
            txtPath.Text = info.path;
            //判断路径是否存在
            if (!File.Exists(info.path) && !Directory.Exists(info.path))
            {
                txtPath.Foreground = new SolidColorBrush(Colors.Red);
            }
            
        }

        private void btnDelet_Click(object sender, RoutedEventArgs e)
        {
            WindowCenter.SettingWindow.removeListBoxItem(this);
        }

        private void btnSelectPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = dialog.SelectedPath;
                txtPath.Foreground = new SolidColorBrush(Colors.Black);
                AllControl.hotpathId[txtID.Text].path = txtPath.Text;
            }
        }
        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                txtPath.Text = dialog.FileName;
                txtPath.Foreground = new SolidColorBrush(Colors.Black);
                AllControl.hotpathId[txtID.Text].path = txtPath.Text;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtID.TextChanged += TxtID_TextChanged;
            txtName.TextChanged += TxtName_TextChanged;
        }

        private void TxtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = txtName.Text.Trim();
            //名字不能是数字
            int num;
            if (int.TryParse(text, out num))
            {
                WindowCenter.SettingWindow.ShowMassage("自定义名称不能是数字。");
                return;
            }
            //不能为空
            if(text == "")
            {
                WindowCenter.SettingWindow.ShowMassage("不能为空。");
                return;
            }
            //键是否重复
            if (AllControl.hotpathName.ContainsKey(text))
            {
                if(text != info.fastName)
                {
                    WindowCenter.SettingWindow.ShowMassage("自定义名称重复。");
                }
                else
                {
                    WindowCenter.SettingWindow.ClearMassage();
                }
                return;
            }
            WindowCenter.SettingWindow.ClearMassage();
            string fn = info.fastName;
            //原来的键的名字
            info.fastName = text;
            AllControl.hotpathName.Remove(fn);
            AllControl.hotpathName.Add(text, info);
            e.Handled = false;
        }

        private void TxtID_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = txtID.Text.Trim();
            //名字必须是数字
            int num;
            if (!int.TryParse(text, out num))
            {
                WindowCenter.SettingWindow.ShowMassage("ID必须是数字。");
                return;
            }
            //键是否重复
            if (AllControl.hotpathId.ContainsKey(text))
            {
                //如果和原来的一样就不显示
                if(text != info.ID)
                {
                    WindowCenter.SettingWindow.ShowMassage("ID名称重复。");
                }
                else
                {
                    WindowCenter.SettingWindow.ClearMassage();
                }
                return;
            }
            WindowCenter.SettingWindow.ClearMassage();
            //原来的键的名字
            string keyId = this.info.ID;
            info.ID = text;
            AllControl.hotpathId.Remove(keyId);
            AllControl.hotpathId.Add(info.ID, info);
            e.Handled = false;
        }

        private void txt_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((System.Windows.Controls.TextBox)sender).SelectAll();
        }
       
    }
}
