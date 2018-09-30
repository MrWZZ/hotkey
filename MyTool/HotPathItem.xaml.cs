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

namespace MyTool
{
    /// <summary>
    /// HotPathItem.xaml 的交互逻辑
    /// </summary>
    public partial class HotPathItem : System.Windows.Controls.UserControl
    {
        //当前控件的ID
        private int ID;

        public HotPathItem(HotPathInfo info)
        {
            InitializeComponent();
            txtID.Text = info.ID;
            txtName.Text = info.fastName;
            txtPath.Text = info.path;
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
                AllControl.hotpathId[txtID.Text].path = txtPath.Text;
            }
        }
        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                txtPath.Text = dialog.FileName;
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
            //名字不能是数字
            int num;
            if (int.TryParse(txtName.Text, out num))
            {
                WindowCenter.SettingWindow.ShowMassage("自定义名称不能是数字。");
                return;
            }
            //不能为空
            if(txtName.Text == "")
            {
                WindowCenter.SettingWindow.ShowMassage("不能为空。");
                return;
            }
            //键是否重复
            if (AllControl.hotpathName.ContainsKey(txtName.Text))
            {
                WindowCenter.SettingWindow.ShowMassage("自定义名称重复。");
                return;
            }
            WindowCenter.SettingWindow.ClearMassage( );
            //原来的键的名字
            HotPathInfo info = AllControl.hotpathId[ID.ToString()];
            string fn = info.fastName;
            info.fastName = txtName.Text;
            AllControl.hotpathName.Remove(fn);
            AllControl.hotpathName.Add(txtName.Text, info);
        }

        private void TxtID_TextChanged(object sender, TextChangedEventArgs e)
        {
            //名字必须是数字
            int num;
            if (!int.TryParse(txtID.Text, out num))
            {
                WindowCenter.SettingWindow.ShowMassage("ID必须是数字。");
                return;
            }
            //键是否重复
            if (AllControl.hotpathId.ContainsKey(txtID.Text))
            {
                WindowCenter.SettingWindow.ShowMassage("ID名称重复。");
                return;
            }
            WindowCenter.SettingWindow.ClearMassage();
            AllControl.hotpathId[ID.ToString()].ID = txtID.Text;
        }

        private void txt_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int.TryParse(txtID.Text, out ID);
            ((System.Windows.Controls.TextBox)sender).SelectAll();
        }
    }
}
