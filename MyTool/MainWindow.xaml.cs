using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Interop;
using TopWindow;
using HotKey;

namespace MyTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //系统托盘
        public NotifyIcon notifyIcon;
        //快捷键管理
        public HotKeyManager hotKeyManager;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            //注册快捷键设置
            hotKeyManager = new HotKeyManager(this);
            hotKeyManager.RegisterHotkey(Keys.Z, HotKeyManager.KeyFlags.MOD_ALT, ()=>ShowWindow());
        }

        public MainWindow()
        {
            InitializeComponent();
            InitNotify();

            this.Closing += MainWindow_Closing;
            WindowStartupLocation = WindowStartupLocation.Manual;
            SetWindowPos();
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
            notifyIcon = new NotifyIcon();
            notifyIcon.Text = "工具";

            notifyIcon.Icon = Properties.Resources.favicon;
            notifyIcon.Visible = true;

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
            //todo 设置窗口
            
        }

        private void SetWindowPos()
        {
            double w = SystemParameters.PrimaryScreenWidth;
            double h = SystemParameters.PrimaryScreenHeight;

            //将窗口放在右下角
            this.Top = h - this.Height;
            this.Left = w - this.Width;
        }

        //窗口显示设置
        private void ShowWindow(bool isShow = true)
        {
            if (isShow)
            {
                Visibility = Visibility.Visible;
                TopManager.SetTop(this);
                SetWindowPos();
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
            hotKeyManager.Clear();
            System.Windows.Application.Current.Shutdown();
        }

        private void txtInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Console.WriteLine(this.txtInput.Text);
                ShowWindow(false);
            }
        }
    }
}
