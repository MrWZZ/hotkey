using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyTool.Script
{
    [Serializable]
    public class Item
    {
        /// <summary>
        /// 热键的全局唯一标识
        /// </summary>
        public uint hotkeyid;
        /// <summary>
        /// 实体键码
        /// </summary>
        public uint key;
        /// <summary>
        /// 功能键码和
        /// </summary>
        public uint keyFlag;
        /// <summary>
        /// 显示组合键名
        /// </summary>
        public string keyName;
        /// <summary>
        /// 注册方法的参数
        /// </summary>
        public string arg;
        /// <summary>
        /// 注册的方法名
        /// </summary>
        public string methodName;
        /// <summary>
        /// 方法关联的控件名
        /// </summary>
        public string controlName;

        public Item(string controlName, MethodName methodName, string arg)
        {
            this.controlName = controlName;
            this.methodName = methodName.ToString();
            this.arg = arg;
        }
    }
}
