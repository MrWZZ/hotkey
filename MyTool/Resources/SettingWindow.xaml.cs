using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MyTool.Script;

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
            WindowCenter.SettingWindow = this;
            LockKeyManager.GetCombineKeyEvent += LockKeyManager_GetCombineKeyEvent;
            SetControlName();
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
                control.Background = new SolidColorBrush(Colors.Red);
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

    }
}
