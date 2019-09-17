using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Xml;

namespace WowItemCreator
{
    public static class Configuration
    {
        private static Logger log;

        public static Table CURRENT_TABLE;

        static Configuration()
        {
            log = new Logger(typeof(Configuration));
            // table = new Table(AppDomain.CurrentDomain.BaseDirectory + "conf\\item_template.xml");
            CURRENT_TABLE = new Table(Properties.Resources.item_template);
        }

        public static int getPageSize()
        {
            int r = 50;
            return r;
        }



        public static ItemFieldData[] getItemFieldData(string fieldName)
        {
            return getItemFieldData(fieldName, string.Empty, string.Empty);
        }

        public static ItemFieldData[] getItemFieldData(string fieldName, string child, string parent)
        {
            ArrayList list = new ArrayList();
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Data\\" + fieldName + child + ".txt";
            if (File.Exists(filePath))
            {
                try
                {
                    StreamReader sr = new StreamReader(filePath);
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        int pos = line.IndexOf(',');
                        if (pos > 0)
                        {
                            string value = line.Substring(0, pos);
                            string name = line.Substring(pos + 1, line.Length - pos - 1);
                            ItemFieldData data = new ItemFieldData();
                            data.Name = value+" - "+name;
                            data.Value = value;
                            data.Parent = parent;
                            list.Add(data);
                        }
                    }
                }
                catch (Exception e)
                {
                    log.error("读取配置文件出错" + filePath);
                    log.error(e);
                }
            }
            return (ItemFieldData[])list.ToArray(typeof(ItemFieldData));
        }
    }
}
