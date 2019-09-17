using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WowItemCreator
{
    public class Field
    {
        private string _name;
        private string _displayName;

        private string _value;
        private Type _valueType;

        private string _parent;
        private string _child;

        private string _regex;
        private string _data;

        private string _tips;
        private int _show_index;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public Type ValueType
        {
            get { return _valueType; }
            set { _valueType = value; }
        }

        public string Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public string Child
        {
            get { return _child; }
            set { _child = value; }
        }

        public string Regex
        {
            get { return _regex; }
            set { _regex = value; }
        }

        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public string Tips
        {
            get
            {
                return _tips;
            }

            set
            {
                _tips = value;
            }
        }

        public int ShowIndex
        {
            get
            {
                return _show_index;
            }

            set
            {
                _show_index = value;
            }
        }

 

        public Field Clone()
        {
            Field p = new Field();
          
            p._displayName = this._displayName;
            p._name = this._name;

            p._parent = this._parent;
            p._child = this._child;
            p._regex = this._regex;
            p._value = this._value;
            p._valueType = this._valueType;
            p._data = this._data;
            p._show_index = this._show_index;
            p._tips = this._tips;
            return p;
        }
    }
}
