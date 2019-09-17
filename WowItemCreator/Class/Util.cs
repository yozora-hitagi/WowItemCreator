using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;

namespace WowItemCreator
{
    public class Util
    {
        public static string GetCpuID()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + HardwareName.Win32_Processor.ToString());
            foreach (ManagementObject obj in searcher.Get())
            {
                if (obj.GetPropertyValue("ProcessorId") != null)
                    return obj.GetPropertyValue("ProcessorId").ToString();
            }
            return null;
        }

        public static string OpenFile(string filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
            openFileDialog.Filter = filter;
            openFileDialog.Multiselect = false;
            DialogResult r = openFileDialog.ShowDialog();
            if (r == System.Windows.Forms.DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return null;
        }
    }
}
