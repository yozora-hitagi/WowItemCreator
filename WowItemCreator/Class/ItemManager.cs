using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using MySql.Data.MySqlClient;

namespace WowItemCreator
{
    public class ItemManager
    {
        private Logger log = new Logger(typeof(ItemManager));
        private string tableName;
        private string idField;
        private DBAccess _db;
        private Field[] itemProperties;

        public DBAccess Db
        {
            get { return _db; }
            set { _db = value; }
        }

        public ItemManager()
        {
            this._db = new DBAccess();
            this.tableName = Configuration.CURRENT_TABLE.TableName;
            this.idField = Configuration.CURRENT_TABLE.Id_field_name;
            this.itemProperties = Configuration.CURRENT_TABLE.Fields;
        }

        public void addItem(Record item)
        {
            if (item == null || item.Fields.Length == 0)
                throw new Exception("物品信息错误。");
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO " + this.tableName);
            string fieldStr = string.Empty;
            string valueStr = string.Empty;
            foreach (Field p in item.Fields)
            {
                fieldStr += "," + p.Name;
                valueStr += ",'" + p.Value.Replace("'", "''") + "'";
            }
            sb.Append("(" + fieldStr.Substring(1) + ") ");
            sb.Append("VALUES(" + valueStr.Substring(1) + ");");
            this._db.execute(sb.ToString());
        }

        public void updateItem(Record item)
        {
            if (item == null || item.Fields.Length == 0)
                throw new Exception("物品信息错误。");
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE " + this.tableName + " SET ");
            foreach (Field p in item.Fields)
            {
                if (p != null)
                    sb.Append(p.Name + "=" + (p.Value == null ? "null" : ("'" + p.Value.Replace("'", "''") + "'")) + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(" WHERE " + this.idField + "='" + item.getField(this.idField).Value.Replace("'", "''") + "';");
            this._db.execute(sb.ToString());
        }

        public void deleteItem(string itemId)
        {
            if (itemId == null || itemId.Length == 0)
                throw new Exception("物品信息错误。");
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM " + this.tableName);
            sb.Append(" WHERE " + this.idField + "='" + itemId.Replace("'", "''") + "';");
            this._db.execute(sb.ToString());
        }

        public string getMaxValue(string fieldName, string tableName)
        {
            string sql = "SELECT MAX(" + fieldName + ") FROM " + tableName;
            object obj = this._db.executeScalar(sql);
            return obj == null ? null : obj.ToString();
        }

        public Record getItem(string itemId)
        {
            Record item = null;
            string sql = "SELECT * FROM " + this.tableName + " WHERE " + this.idField + "='" + itemId.Replace("'", "''") + "'";
            DataTable dt = this._db.query(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                Field[] properties = this.getItemProperties(this.tableName);
                foreach (Field p in properties)
                {
                    foreach (Field pp in this.itemProperties)
                    {
                        if (pp.Name.ToUpper() == p.Name.ToUpper())
                        {
                            p.DisplayName = pp.DisplayName;
                            p.Child = pp.Child;
                            p.Parent = pp.Parent;
                            p.Regex = pp.Regex;
                            p.Data = pp.Data;
                            break;
                        }
                    }
                }
                item = this.dataTable2item(dt, properties)[0];
            }
            return item;
        }

        public Record[] getItems(string id,string type, string subType, string name, int start, int limit)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM " + this.tableName + " ");
            bool appendWhere = false;
            if (id != null)
            {
                if (appendWhere)
                    sb.Append("AND ");
                else
                {
                    appendWhere = true;
                    sb.Append("WHERE ");
                }
                sb.Append("entry='" + id + "' ");
            }
            if (type != null)
            {
                if (appendWhere)
                    sb.Append("AND ");
                else
                {
                    appendWhere = true;
                    sb.Append("WHERE ");
                }
                sb.Append("class='" + type + "' ");
            }
            if (subType != null)
            {
                if (appendWhere)
                    sb.Append("AND ");
                else
                {
                    appendWhere = true;
                    sb.Append("WHERE ");
                }
                sb.Append("subclass='" + subType + "' ");
            }
            if (name != null)
            {
                if (appendWhere)
                    sb.Append("AND ");
                else
                {
                    appendWhere = true;
                    sb.Append("WHERE ");
                }
                sb.Append("name LIKE '%" + name.Replace("'", "''") + "%' ");
            }
            sb.Append(" ORDER BY " + this.idField + " DESC LIMIT " + start + "," + limit);
            DataTable dt = this._db.query(sb.ToString());
            if (dt != null && dt.Rows.Count > 0)
            {
                Field[] properties = this.getItemProperties(this.tableName);
                foreach (Field p in properties)
                {
                    foreach (Field pp in this.itemProperties)
                    {
                        if (pp.Name.ToUpper() == p.Name.ToUpper())
                        {
                            p.DisplayName = pp.DisplayName;
                            p.Child = pp.Child;
                            p.Parent = pp.Parent;
                            p.Regex = pp.Regex;
                            p.Data = pp.Data;
                            break;
                        }
                    }
                }
                return this.dataTable2item(dt, properties);
            }
            else
                return new Record[0];
        }

        private Record[] dataTable2item(DataTable dt, Field[] properties)
        {
            ArrayList list = new ArrayList();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                Record item = new Record();
                foreach (Field p in properties)
                {
                    try
                    {
                        Field pro = p.Clone();
                        pro.Value = dr.IsNull(pro.Name) ? null : dr[pro.Name].ToString();
                        item.addField(pro);
                    }
                    catch (Exception err)
                    {
                        log.error("转换物品属性出错，属性名：" + p.Name);
                        log.error(err);
                    }
                }
                list.Add(item);
            }
            return (Record[])list.ToArray(typeof(Record));
        }

        public int getItmesCount(string id,string type, string subType, string name)
        {
            int r = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT COUNT(*) FROM " + this.tableName + " ");
            bool appendWhere = false;
            if (id != null)
            {
                if (appendWhere)
                    sb.Append("AND ");
                else
                {
                    appendWhere = true;
                    sb.Append("WHERE ");
                }
                sb.Append("entry='" + id + "' ");
            }
            if (type != null)
            {
                if (appendWhere)
                    sb.Append("AND ");
                else
                {
                    appendWhere = true;
                    sb.Append("WHERE ");
                }
                sb.Append("class='" + type + "' ");
            }
            if (subType != null)
            {
                if (appendWhere)
                    sb.Append("AND ");
                else
                {
                    appendWhere = true;
                    sb.Append("WHERE ");
                }
                sb.Append("subclass='" + subType + "' ");
            }
            if (name != null)
            {
                if (appendWhere)
                    sb.Append("AND ");
                else
                {
                    appendWhere = true;
                    sb.Append("WHERE ");
                }
                sb.Append("name LIKE '%" + name + "%' ");
            }
            object obj = this._db.executeScalar(sb.ToString());
            if (obj != null)
            {
                r = int.Parse(obj.ToString());
            }
            return r;
        }

        public Field[] getItemProperties(string tableName)
        {
            ArrayList list_res = new ArrayList();
            DataTable dt = Db.query("DESC " + tableName);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Field p = new Field();
                    p.Name = dr.IsNull("Field") ? string.Empty : dr["Field"].ToString();
                    p.Value = dr.IsNull("Default") ? string.Empty : dr["Default"].ToString();
                    p.ValueType = dr.IsNull("Type") ? null : dbType2type(dr["Type"]);
                    list_res.Add(p);
                }
            }
            return (Field[])list_res.ToArray(typeof(Field));
        }

        public Type dbType2type(object dbType)
        {
            Type res = typeof(String);
            if (dbType == null)
                return res;
            string typeStr = dbType.ToString();
            int pos = Math.Min(typeStr.IndexOf('('), typeStr.IndexOf(' '));
            if (pos > -1)
                typeStr = typeStr.Substring(0, pos);
            switch (typeStr)
            {
                case "binary":
                case "varbinary": 
                    res = typeof(Byte[]);
                    break;
                case "bit":
                    res = typeof(Byte);
                    break;
                case "date":
                case "time":
                case "datetime":
                case "timestamp": 
                    res = typeof(DateTime);
                    break;
                case "decimal":
                    res = typeof(Decimal);
                    break;
                case "double":
                    res = typeof(Double);
                    break;
                case "enum":
                    res = typeof(Enum);
                    break;
                case "float":
                    res = typeof(float);
                    break;
                case "mediumint":
                case "tinyint":
                case "smallint":
                case "int":
                    res = typeof(int);
                    break;
                case "bigint":
                    res = typeof(Int64);
                    break;
            }
            return res;
        }

    }
}
