using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace WowItemCreator
{
    public class Record 
    {
        private ArrayList field_list;

        public Record()
        {
            field_list = new ArrayList();
        }

        public void addField(Field p)
        {
            field_list.Add(p);
        }

        public Field getField(string name)
        {
            Field res = null;
            if (name != null && this.field_list != null)
            {
                foreach (Field p in this.field_list)
                {
                    if (p.Name.Trim().ToUpper() == name.Trim().ToUpper())
                        res = p;
                }
            }
            return res;
        }

        public void removeField(string name)
        {
            Field p = getField(name);
            if (p != null)
                this.field_list.Remove(p);
        }

        public bool hasField(string name)
        {
            bool res = false;
            Field p = getField(name);
            if (p != null)
                res = true;
            return res;
        }

        public Field[] Fields
        {
            get { return (Field[])this.field_list.ToArray(typeof(Field)); }
        }
    }
}
