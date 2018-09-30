using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTool.Script
{
    [Serializable]
    public class Config
    {
        //是否开机启动
        public bool isAutoStartUp;
        //当前最大可用listbox的ID
        public int canUseMinID;
        //控件及对应的快捷键信息
        public Dictionary<string, Item> items;
        //列表路径快捷方式,name
        public Dictionary<string, HotPathInfo> hotpathName;
    }
}
