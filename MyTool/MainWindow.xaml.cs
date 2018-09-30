using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using TopWindow;
using MyTool.Script;
using System.Windows.Media;

namespace MyTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //系统托盘
        public NotifyIcon notifyIcon;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            //注册快捷键设置
            AllControl.register = new Register(this);
            AllControl.ReadConfig();
            ShowWindow(false);
        }

        public MainWindow()
        {
            InitializeComponent();
            InitNotify();
            //窗体状态改变时候触发
            StateChanged += OnStateChanged;
            Closing += MainWindow_Closing;
            Closed += MainWindow_Closed;
            WindowStartupLocation = WindowStartupLocation.Manual;
            WindowCenter.MainWindow = this;
        }


        private void MainWindow_Closed(object sender, EventArgs e)
        {
            AllControl.SaveConfig();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 添加托盘图标
        /// </summary>
        private void InitNotify()
        {
            //设置托盘的各个属性
            notifyIcon = new NotifyIcon
            {
                Text = "工具",
                Icon = Properties.Resources.favicon,
                Visible = true
            };

            //退出菜单项
            MenuItem exit = new MenuItem("退出");
            exit.Click += new EventHandler(Exit);
            //设置菜单
            MenuItem setting = new MenuItem("设置");
            setting.Click += new EventHandler(Setting);

            //关联托盘控件
            MenuItem[] childen = new MenuItem[] { setting, exit };
            notifyIcon.ContextMenu = new ContextMenu(childen);
        }

        private void Setting(object sender, EventArgs e)
        {
            if(WindowCenter.SettingWindow == null)
            {
                WindowCenter.SettingWindow = new SettingWindow();
            }
            WindowCenter.SettingWindow.Show();
            

        }

        private void SetWindowPos()
        {
            //屏幕高度
            double ph = SystemParameters.PrimaryScreenHeight;
            //工作区高度
            double fh = SystemParameters.FullPrimaryScreenHeight;
            //任务栏高度
            double offset = ph - fh;

            //将窗口放在左下角
            this.Top = ph - this.Height - offset;
            this.Left = 0;
        }

        //窗口显示设置
        public void ShowWindow(bool isShow = true)
        {
            if (isShow)
            {
                Visibility = Visibility.Visible;
                TopManager.SetTop(this);
                SetWindowPos();
                this.txtInput.SelectAll();
                Activate();
            }
            else
            {
                Visibility = Visibility.Hidden;
            }

        }

        /// <summary>
        /// 窗口状态改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit(object sender, EventArgs e)
        {
            this.notifyIcon.Visible = false;
            System.Windows.Application.Current.Shutdown();
        }

        private void txtInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int num = 0;
                if (int.TryParse(txtInput.Text, out num))
                {
                    //判断是否存在
                    if (!AllControl.hotpathId.ContainsKey(txtInput.Text))
                    {
                        ShowMessage("ID所对应快捷路径不存在。");
                        return;
                    }
                    string path = AllControl.hotpathId[txtInput.Text].path;
                    MethodCenter.Methods[MethodName.OpenFile](path);
                }
                else
                {
                    if(txtInput.Text == "")
                    {
                        ShowMessage("不能为空。");
                        return;
                    }
                    //判断是否存在
                    if (!AllControl.hotpathName.ContainsKey(txtInput.Text))
                    {
                        ShowMessage("自定义名称所对应快捷路径不存在。");
                        return;
                    }
                    string path = AllControl.hotpathName[txtInput.Text].path;
                    MethodCenter.Methods[MethodName.OpenFile](path);
                }
                ShowWindow(false);
            }
            else if(e.Key == Key.Escape)
            {
                ShowWindow(false);
            }
        }

        //在方框显示消息
        private void ShowMessage(string msg)
        {
            txtInput.Text = msg;
        }

        public void ShowNotify(string msg)
        {
            notifyIcon.ShowBalloonTip(500, "热键工具", msg, ToolTipIcon.Error);
        }
    }
}
