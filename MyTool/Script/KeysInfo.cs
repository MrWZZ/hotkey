using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTool.Script
{
    public class KeysInfo
    {
        public List<uint> lockKey;
        //实体键
        public uint key;
        //功能键值之和
        public uint keyFlag = 0;
        //组合键名称
        public string keyName;
        //是否包含win键
        public bool isWin = false;

        public enum KeyFlag
        {
            None, Shift, Ctrl, Alt, Win
        }

        public KeysInfo(List<uint> lockKey, uint commonKey)
        {
            this.lockKey = lockKey;
            this.key = commonKey;
            CombineKey();
        }

        public void CombineKey()
        {
            KeyFlag[] keys = new KeyFlag[4];
            //判断组合键
            foreach (var key in lockKey)
            {
                switch (key)
                {
                    case 160:
                    case 161:
                        if(keys[0] != KeyFlag.Shift)
                        {
                            keys[0] = KeyFlag.Shift;
                            keyFlag += 4;
                        }
                        break;
                    case 162:
                    case 163:
                        if(keys[1] != KeyFlag.Ctrl)
                        {
                            keys[1] = KeyFlag.Ctrl;
                            keyFlag += 2;                            
                        }
                        break;
                    case 164:
                    case 165:
                        if(keys[2] != KeyFlag.Alt)
                        {
                            keys[2] = KeyFlag.Alt;
                            keyFlag += 1;
                        }
                        break;
                    case 91:
                    case 92:
                        if (keys[3] != KeyFlag.Win)
                        {
                            keys[3] = KeyFlag.Win;
                            isWin = true;
                            keyFlag += 8;
                        }
                        break;
                }
            }

            //名字固定顺序
            List<string> ls = new List<string>();
            foreach (var k in keys)
            {
                if (k != KeyFlag.None)
                {
                    ls.Add(k.ToString());
                }
            }
            keyName = string.Join("+", ls);
            var comkey = (System.Windows.Forms.Keys)key;
            keyName += "+" + comkey.ToString();
        }
    }
}
