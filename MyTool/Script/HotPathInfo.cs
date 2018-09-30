using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTool.Script
{
    public class HotPathInfo
    {   
        //分配的ID
        public string ID;
        //自定义名字
        public string fastName;
        //路径
        public string path;

        public HotPathInfo(string ID, string fastName, string path)
        {
            this.ID = ID;
            this.fastName = fastName;
            this.path = path;
        }
    }
}
