using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace MyTool.Script
{
    public class AllControl
    {
        /// <summary>
        /// 全局热键注册器
        /// </summary>
        public static Register register;

        /// <summary>
        /// 当前所操作的控件
        /// </summary>
        public static Control CurrControl { get; set; }

        //是否开机启动
        public static bool isAutoStartUp = false;
        //当前最大listbox的ID
        public static int canUseMinID = 0;
        //控件及对应的快捷键信息
        public static Dictionary<string, Item> items = new Dictionary<string, Item>();
        //列表路径快捷方式,id
        public static Dictionary<string, HotPathInfo> hotpathId = new Dictionary<string, HotPathInfo>();

        //热键ID及对应的快捷键信息
        public static Dictionary<uint, Item> keyIds = new Dictionary<uint, Item>();
        //列表路径快捷方式,name
        public static Dictionary<string, HotPathInfo> hotpathName = new Dictionary<string, HotPathInfo>();


        public static void AddHotPathItem(string ID,string fastName,string path)
        {
            HotPathInfo info = new HotPathInfo(ID, fastName, path);
            hotpathId.Add(info.ID, info);
            hotpathName.Add(info.fastName, info);
        }

        public static void RemoveHotPathItem(string ID)
        {
            hotpathName.Remove(hotpathId[ID].fastName);
            hotpathId.Remove(ID);
        }

        /// <summary>
        /// 添加已注册热键的控件
        /// </summary>
        public static uint AddHotkeyItem(string controlName,uint key,uint keyFlag,string keyName)
        {
            ////移除原有注册
            Item item = items[controlName];
            RemoveHotKeyItem(item.controlName);
            //重新注册热键
            uint result = register.RegisterHotkey(key, keyFlag, out item.hotkeyid);
            if(result == 1)
            {
                item.key = key;
                item.keyFlag = keyFlag;
                item.keyName = keyName;
                keyIds.Add(item.hotkeyid, item);
            }
            return result;
        }

        /// <summary>
        /// 移除已注册热键的控件
        /// </summary>
        public static uint RemoveHotKeyItem(string name)
        {
            Item item = items[name];
            uint result = 0;
            if (item.hotkeyid != 0)
            {
                result = register.UnregisterHotkey(item.hotkeyid);
                keyIds.Remove(item.hotkeyid);
            }
            return result;
        }

        //用户按下快捷键，判断程序是否有对该快捷键对方法
        public static void CallFunction(uint keyId)
        {
            if (keyIds.ContainsKey(keyId))
            {
                Item item = keyIds[keyId];
                MethodName mn = (MethodName)Enum.Parse(typeof(MethodName), item.methodName);
                MethodCenter.Methods[mn](item.arg);
            }
        }

        public static string configPath = @".\hotkey_config.txt";
        //读取配置文件
        public static void ReadConfig()
        {
            //判断文件是否存在
            if (File.Exists(configPath))
            {
                Config config;
                using (StreamReader sr = new StreamReader(configPath,Encoding.UTF8))
                {
                    string file = sr.ReadToEnd();
                    config = JsonConvert.DeserializeObject<Config>(file);
                }

                canUseMinID = config.canUseMinID;
                items = config.items;
                hotpathName = config.hotpathName;
                isAutoStartUp = config.isAutoStartUp;
                //判断是否文件路径发生改变
                bool isChange = false;
                foreach (var item in hotpathName.Values)
                {
                    hotpathId.Add(item.ID, item);
                    //判断路径是否存在
                    if (!File.Exists(item.path) && !Directory.Exists(item.path))
                    {
                        isChange = true;
                    }
                }
                if (isChange)
                {
                    WindowCenter.MainWindow.ShowNotify("有文件的路径发生改变，请打开设置面板重新设置确定路径。");
                }

                //注册热键
                foreach (var itemName in items.Keys)
                {
                    var item = items[itemName];
                    //该控件拥有热键
                    if(item.hotkeyid != 0)
                    {
                        item.hotkeyid = 0;
                        AddHotkeyItem(item.controlName,item.key, item.keyFlag, item.keyName);
                    }
                }

                //开启启动
                string name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToString();
                string directory = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string shortcutPath = Path.Combine(directory, string.Format("{0}.lnk", name));
                if (isAutoStartUp)
                {
                    //文件不存在则重新创建
                    if (!File.Exists(shortcutPath))
                    {
                        string path = Process.GetCurrentProcess().MainModule.FileName;
                        bool isSuccess = CreateStartup(name, path);
                        if (!isSuccess)
                        {
                            WindowCenter.MainWindow.ShowNotify("开机启动设置失败。");
                            isAutoStartUp = false;
                        }
                    }

                }
                else
                {
                    DestoryFile(shortcutPath);
                }
            }
            else
            {
                MethodCenter.ResetConfig();
                OpenSettingWindow();
            }
        }

        //保存配置文件
        public static void SaveConfig()
        {
            Config config = new Config();
            config.hotpathName = hotpathName;
            config.items = items;
            config.canUseMinID = canUseMinID;
            config.isAutoStartUp = isAutoStartUp;

            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(configPath,false,Encoding.UTF8))
            {
                sw.Write(json);
            }
        }

        /// <summary>
        /// 注册表：开机自动启动（在window10启动失败)
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
                    catch
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                if (key != null)
                {
                    key.Close();
                }
                return false;
            }
        }

        //在开始菜单创建快捷方式启动
        public static bool CreateStartup(string shortcutName, string targetPath, string description = null, string iconLocation = null)
        {
            // 获取全局 开始 文件夹位置
            //string directory = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
            // 获取当前登录用户的 开始 文件夹位置
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                //IWshRuntimeLibrary:添加引用 Com 中搜索 Windows Script Host Object Model
                string shortcutPath = Path.Combine(directory, string.Format("{0}.lnk", shortcutName));
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);//创建快捷方式对象
                shortcut.TargetPath = targetPath;//指定目标路径
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);//设置起始位置
                shortcut.WindowStyle = 1;//设置运行方式，默认为常规窗口
                shortcut.Description = description;//设置备注
                shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation;//设置图标路径
                shortcut.Save();//保存快捷方式

                return true;
            }
            catch
            { }
            return false;
        }

        //打开设置页面
        public static void OpenSettingWindow()
        {
            if (WindowCenter.SettingWindow == null)
            {
                WindowCenter.SettingWindow = new SettingWindow();
            }
            WindowCenter.SettingWindow.Show();
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <returns></returns>
        public static bool DestoryFile(string path)
        {
            try
            {
                File.Delete(path);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
