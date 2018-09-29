using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using TopWindow;
using MyTool.Script;

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
        }

        public MainWindow()
        {
            InitializeComponent();
            InitNotify();
            Closing += MainWindow_Closing;
            Closed += MainWindow_Closed;
            WindowStartupLocation = WindowStartupLocation.Manual;
            SetWindowPos();
            TopManager.SetTop(this);
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
            MenuItem[] childen = new MenuItem[] { setting,exit };
            notifyIcon.ContextMenu = new ContextMenu(childen);

            //窗体状态改变时候触发
            this.StateChanged += new EventHandler(OnStateChanged);
        }

        private void Setting(object sender, EventArgs e)
        {
            new SettingWindow().Show();
        }

        private void SetWindowPos()
        {
            //屏幕高度
            double ph = SystemParameters.PrimaryScreenHeight;
            //工作区高度
            double fh = SystemParameters.FullPrimaryScreenHeight;
            //任务栏高度
            double offset = ph-fh;

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
            if(e.Key == Key.Enter)
            {
                Console.WriteLine(this.txtInput.Text);
                //TODO 回车后对指令执行方法
                ShowWindow(false);
            }
        }
    }
}
