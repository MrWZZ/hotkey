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

        //控件及对应的快捷键信息
        public static Dictionary<string, Item> items = new Dictionary<string, Item>();
        //热键ID及对应的快捷键信息
        public static Dictionary<uint, Item> keyIds = new Dictionary<uint, Item>();

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
            }
            return result;
        }

        /// <summary>
        /// 清除所有热键
        /// </summary>
        //public static void ClearHotKeyItem()
        //{
        //    List<uint> list = new List<uint>(hotkeyItems.Keys);
        //    foreach(uint l in list)
        //    {
        //        RemoveHotKeyItem(l);
        //    }
        //    register.Clear();
        //}

        /// <summary>
        /// 获取已注册热键的数据源
        /// </summary>
        //public static Item GetHotkeyItem(Control control)
        //{
        //    foreach (var item in hotkeyItems.Values)
        //    {
        //        if (item.control == control)
        //        {
        //            return item;
        //        }
        //    }
        //    return null;
        //}

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

        /// <summary>
        /// 重新注册所有热键
        /// </summary>
        //public static void ReregisterHotkey()
        //{
        //    foreach (Item i in hotkeyItems.Values)
        //    {
        //        uint result;
        //        uint keyid = register.RegisterHotkey(i.key, i.keyFlag, out result);
        //        ((TextBox)i.control).Text = i.keyName;
        //        i.keyId = keyid;
        //    }
        //}

        public static string configPath = @".\hotkey_config.txt";
        //读取配置文件
        public static void ReadConfig()
        {
            //判断文件是否存在
            if (File.Exists(configPath))
            {
                using(StreamReader sr = new StreamReader(configPath,Encoding.UTF8))
                {
                    string file = sr.ReadToEnd();
                    items = JsonConvert.DeserializeObject<Dictionary<string,Item>>(file);
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
                //手动给控件注册方法
                Item i0 = new Item("txtCmd_Open", MethodName.SetMainVisible, "true");
                items.Add(i0.controlName, i0);
                Item i1 = new Item("txtCmd_Close", MethodName.SetMainVisible, "false");
                items.Add(i1.controlName, i1);
                SaveConfig();
            }
        }

        //保存配置文件
        public static void SaveConfig()
        {
            string json = JsonConvert.SerializeObject(items,Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(configPath,false,Encoding.UTF8))
            {
                sw.Write(json);
            }
        }
    }
}
