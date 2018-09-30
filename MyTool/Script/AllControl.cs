using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
                using (StreamReader sr = new StreamReader(configPath,Encoding.UTF8))
                {
                    string file = sr.ReadToEnd();
                    Config config = JsonConvert.DeserializeObject<Config>(file);
                    canUseMinID = config.canUseMinID;
                    items = config.items;
                    hotpathName = config.hotpathName;
                    foreach (var item in hotpathName.Values)
                    {
                        hotpathId.Add(item.ID, item);
                    }

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
            }
            else
            {
                MethodCenter.ResetConfig();
                SaveConfig();
            }
        }

        //保存配置文件
        public static void SaveConfig()
        {
            Config config = new Config();
            config.hotpathName = hotpathName;
            config.items = items;
            config.canUseMinID = canUseMinID;

            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(configPath,false,Encoding.UTF8))
            {
                sw.Write(json);
            }
        }
    }
}
