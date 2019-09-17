using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace WowItemCreator
{
    public class Table
    {
        private string _tableName;
        private string _id_field_name;
        private Field[] _fields;

        public string TableName
        {
            get
            {
                return _tableName;
            }

            set
            {
                _tableName = value;
            }
        }

        public string Id_field_name
        {
            get
            {
                return _id_field_name;
            }

            set
            {
                _id_field_name = value;
            }
        }



        public Field[] Fields
        {
            get
            {
                return _fields;
            }

            set
            {
                _fields = value;
            }
        }

        public Table(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode node = doc.SelectSingleNode("/table");
            if (node != null && node.Attributes["tablename"] != null)
            {
                _tableName = node.Attributes["tablename"].Value;
            }
            if (node != null && node.Attributes["id"] != null)
            {
                _id_field_name = node.Attributes["id"].Value;
            }
           
            ArrayList list = new ArrayList();
            XmlNodeList nodes = doc.SelectNodes("/table/field");
            foreach (XmlNode n in nodes)
            {
                Field pro = new Field();
                pro.Child = n.Attributes["child"].Value;
                pro.DisplayName = n.Attributes["displayname"].Value;
                pro.Name = n.Attributes["name"].Value;
                pro.Parent = n.Attributes["parent"].Value;
                pro.Regex = n.Attributes["regex"].Value;
                pro.Value = n.Attributes["default"].Value;
                pro.Data = n.Attributes["data"].Value;
                string typeStr = n.Attributes["type"].Value;
                if (typeStr != null && typeStr.Trim() != string.Empty)
                {
                    pro.ValueType = Type.GetType(typeStr);
                }
                pro.Tips = n.Attributes["tips"].Value;

                string showid = n.Attributes["show"].Value;
                if(showid!=null &&showid.Trim() != string.Empty)
                {
                    pro.ShowIndex = Convert.ToInt16(showid);
                }
                else
                {
                    pro.ShowIndex = -1;
                }

                list.Add(pro);
            }
            _fields= (Field[])list.ToArray(typeof(Field));
        }
    }
}
