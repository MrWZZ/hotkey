using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LockKey;

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
        }

        public enum KeyFlag
        {
            None,
            Shift,
            Ctrl,
            Alt,
            Win
        }

        private void LockKeyManager_GetCombineKeyEvent(LockKeyManager.KeyCombine keyCombine)
        {
            this.txtCmd.Select(0, 0);
            if (keyCombine.commonKey == -1 || keyCombine.lockKey.Count == 0)
            {
                //todo 缺少实体键或功能键
                return;
            }
            
            KeyFlag[] keys = new KeyFlag[4];
            //判断组合键
            foreach(var key in keyCombine.lockKey)
            {
                switch (key)
                {
                    case 160:
                    case 161:
                        keys[0] = KeyFlag.Shift;
                        break;
                    case 162:
                    case 163:
                        keys[1] = KeyFlag.Ctrl;
                        break;
                    case 164:
                    case 165:
                        keys[2] = KeyFlag.Alt;
                        break;
                    case 91:
                    case 92:
                        keys[3] = KeyFlag.Win;
                        break;
                }
            }
            //组合键组装
            StringBuilder sb = new StringBuilder();
            foreach (var k in keys)
            {
                if (k != KeyFlag.None)
                {
                    sb.Append(k.ToString() + "+");
                }
            }
            //普通键组装
            var comkey = (System.Windows.Forms.Keys)keyCombine.commonKey;
            sb.Append(comkey.ToString());
            this.txtCmd.Text = sb.ToString();
        }

        private void txtCmd_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ((TextBox)sender).SelectAll();
            LockKeyManager.Hook_Start();
        }

        private void txtCmd_MouseLeave(object sender, MouseEventArgs e)
        {
            ((TextBox)sender).Select(0, 0);
            LockKeyManager.Hook_Clear();
        }
    }
}
