using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WowItemCreator
{
    public class Logger
    {
        private Type type;
        private static StreamWriter sw;
        public Logger(Type type)
        {
            this.type = type;
            string basePath = AppDomain.CurrentDomain.BaseDirectory ;
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);
            if (Logger.sw == null)
                Logger.sw = new StreamWriter(basePath + "app.log", true);
        }

        public void debug(object o)
        {
            StringBuilder sb = new StringBuilder();
            DateTime now = DateTime.Now;
            sb.Append(now.ToString("yyyy-MM-dd HH:mm:ss.fff DEBUG "));
            if(this.type != null)
                sb.Append(this.type.FullName + " ");
            if (o != null)
                sb.Append(o);
            sw.WriteLine(sb.ToString());
            sw.Flush();
        }

        public void info(object o)
        {
            StringBuilder sb = new StringBuilder();
            DateTime now = DateTime.Now;
            sb.Append(now.ToString("yyyy-MM-dd HH:mm:ss.fff INFO "));
            if (this.type != null)
                sb.Append(this.type.FullName + " ");
            if (o != null)
                sb.Append(o);
            sw.WriteLine(sb.ToString());
            sw.Flush();
        }

        public void error(object o)
        {
            StringBuilder sb = new StringBuilder();
            DateTime now = DateTime.Now;
            sb.Append(now.ToString("yyyy-MM-dd HH:mm:ss.fff ERROR "));
            if (this.type != null)
                sb.Append(this.type.FullName + " ");
            if (o != null)
                sb.Append(o);
            sw.WriteLine(sb.ToString());
            sw.Flush();
        }
    }
}
