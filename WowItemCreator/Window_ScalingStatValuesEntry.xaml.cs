using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WowItemCreator
{
    /// <summary>
    /// Window_ScalingStatValuesEntry.xaml 的交互逻辑
    /// </summary>
    public partial class Window_ScalingStatValuesEntry : Window
    {
        public Window_ScalingStatValuesEntry()
        {
            InitializeComponent();
            dg_ssv.ItemsSource = getdata().DefaultView;
        }


        DataTable getdata()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("description", typeof(string));
            dt.Columns.Add("value", typeof(string));


            //dt.Rows.Add(new object[] {0, "Id", "" });
            //dt.Rows.Add(new object[] {1, "Level", "" });


            //dt.Rows.Add(new object[] { "ssdMultiplier" });
            dt.Rows.Add(new object[] { null });
            dt.Rows.Add(new object[] { null, "属性值" });
            dt.Rows.Add(new object[] { 2, "Shoulder", "0x00000001" });
            dt.Rows.Add(new object[] { 3, "Trinket (饰品)", "0x00000002" });
            dt.Rows.Add(new object[] { 4, "Weapon1H", "0x00000004" });
            dt.Rows.Add(new object[] { 17, "there's data from 3.1 dbc ssdMultiplier[3]", "0x00000008" });
            dt.Rows.Add(new object[] { 5, "Ranged", "0x00000010" });
            dt.Rows.Add(new object[] { 18, "3.3", "0x00040000" });

            dt.Rows.Add(new object[] { null });
            dt.Rows.Add(new object[] { null, "装备护甲" });
            dt.Rows.Add(new object[] { 6, "Cloth shoulder", "0x00000020" });
            dt.Rows.Add(new object[] { 7, "Leather shoulder", "0x00000040" });
            dt.Rows.Add(new object[] { 8, "Mail shoulder", "0x00000080" });
            dt.Rows.Add(new object[] { 9, "Plate shoulder", "0x00000100" });
            dt.Rows.Add(new object[] { 19, "cloak (披风)", "0x00080000" });
            dt.Rows.Add(new object[] { 20, "cloth", "0x00100000" });
            dt.Rows.Add(new object[] { 21, "leather", "0x00200000" });
            dt.Rows.Add(new object[] { 22, "mail", "0x00400000" });
            dt.Rows.Add(new object[] { 23, "plate", "0x00800000" });

            dt.Rows.Add(new object[] { null });
            dt.Rows.Add(new object[] { null, "武器伤害" });
            dt.Rows.Add(new object[] { 10, "Weapon 1h", "0x00000200" });
            dt.Rows.Add(new object[] { 11, "Weapon 2h", "0x00000400" });
            dt.Rows.Add(new object[] { 12, "Caster dps 1h", "0x00000800" });
            dt.Rows.Add(new object[] { 13, "Caster dps 2h", "0x00001000" });
            dt.Rows.Add(new object[] { 14, "Ranged (远程)", "0x00002000" });
            dt.Rows.Add(new object[] { 15, "Wand (魔棒)", "0x00004000" });

            dt.Rows.Add(new object[] { null });
            dt.Rows.Add(new object[] { 16, "spell power for level", "0x00008000" });


            return dt;
        }


        bool flag = false;
        private void dg_ssv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (flag) return;

            flag = true;

            long ssv = 0;
            foreach (DataRowView drv in dg_ssv.SelectedItems)
            {
                //int id = (int)drv["id"];
                if (drv["value"].ToString() != "")
                {
                    string value = (string)drv["value"];
                    ssv += Convert.ToInt64(value, 16);
                }
            }
            this.tb_ssv.Text = ssv.ToString();

            flag = false;
        }



        private void tb_ssv_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (flag) return;

            flag = true;

            this.dg_ssv.UnselectAll();

            string t = this.tb_ssv.Text;
            if (t != null && t != string.Empty)
            {

                UInt32 mask = Convert.ToUInt32(t);

                if ((mask & 0x4001F) != 0)
                {
                    if ((mask & 0x00000001) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(0)).IsSelected = true; // Shoulder
                    else if ((mask & 0x00000002) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(1)).IsSelected = true; // Trinket
                    else if ((mask & 0x00000004) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(2)).IsSelected = true; // Weapon1H
                    else if ((mask & 0x00000008) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(3)).IsSelected = true;
                    else if ((mask & 0x00000010) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(4)).IsSelected = true; // Ranged
                    else if ((mask & 0x00040000) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(5)).IsSelected = true;
                }

                if ((mask & 0x00F001E0) != 0)
                {
                    if ((mask & 0x00000020) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(6)).IsSelected = true;      // Cloth shoulder
                    else if ((mask & 0x00000040) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(7)).IsSelected = true;      // Leather shoulder
                    else if ((mask & 0x00000080) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(8)).IsSelected = true;      // Mail shoulder
                    else if ((mask & 0x00000100) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(9)).IsSelected = true;      // Plate shoulder

                    else if ((mask & 0x00080000) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(10)).IsSelected = true;      // cloak
                    else if ((mask & 0x00100000) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(11)).IsSelected = true;      // cloth
                    else if ((mask & 0x00200000) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(12)).IsSelected = true;      // leather
                    else if ((mask & 0x00400000) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(13)).IsSelected = true;     // mail
                    else if ((mask & 0x00800000) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(14)).IsSelected = true;      // plate
                }

                if ((mask & 0x7E00) != 0)
                {
                    if ((mask & 0x00000200) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(15)).IsSelected = true;       // Weapon 1h
                    else if ((mask & 0x00000400) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(16)).IsSelected = true;       // Weapon 2h
                    else if ((mask & 0x00000800) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(17)).IsSelected = true;        // Caster dps 1h
                    else if ((mask & 0x00001000) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(18)).IsSelected = true;       // Caster dps 2h
                    else if ((mask & 0x00002000) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(19)).IsSelected = true;     // Ranged
                    else if ((mask & 0x00004000) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(20)).IsSelected = true;       // Wand
                }



                if ((mask & 0x00008000) != 0) ((DataGridRow)this.dg_ssv.ItemContainerGenerator.ContainerFromIndex(21)).IsSelected = true;
            }

            flag = false;
        }
    }
}
