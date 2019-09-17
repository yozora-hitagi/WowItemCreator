using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Collections;

using Lib_Creator;

namespace WowItemCreator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ItemManager manager;
        private Pager pager;
        public MainWindow()
        {
            InitializeComponent();

            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            this.manager = new ItemManager();
            this.pager = new Pager();


            //根据配置文件设置表格绑定字段名
            this.dataGrid1.SelectedValuePath = Configuration.CURRENT_TABLE.Id_field_name;

            ArrayList list = new ArrayList();
            foreach (Field ipt in Configuration.CURRENT_TABLE.Fields)
            {
                if (ipt.ShowIndex >= 0)
                {
                    list.Add(ipt);
                }
            }

            list.Sort(new ShowIndexComparer());
            foreach (Field ipt in list)
            {
                DataGridTextColumn col = new DataGridTextColumn();
                col.IsReadOnly = true;
                col.Header = ipt.DisplayName;
                col.Binding = new Binding(ipt.Name);

                this.dataGrid1.Columns.Insert(0, col);
            }



            //DataGridTextColumn col_id = (DataGridTextColumn)this.dataGrid1.Columns[0];
            //Binding binding_id = new Binding();
            //binding_id.Path = new PropertyPath(Configuration.table.Id_field_name);
            //col_id.Binding = binding_id;
            //DataGridTextColumn col_name = (DataGridTextColumn)this.dataGrid1.Columns[1];
            //Binding binding_name = new Binding();
            //binding_name.Path = new PropertyPath(Configuration.table.Name_field_name);
            //col_name.Binding = binding_name;
            //DataGridTextColumn col_class = (DataGridTextColumn)this.dataGrid1.Columns[2];
            //Binding binding_class = new Binding();
            //binding_class.Path = new PropertyPath(Configuration.table.Class_field_name);
            //col_class.Binding = binding_class;
            //DataGridTextColumn col_subclass = (DataGridTextColumn)this.dataGrid1.Columns[3];
            //Binding binding_subclass = new Binding();
            //binding_subclass.Path = new PropertyPath(Configuration.table.Subclass_field_name);
            //col_subclass.Binding = binding_subclass;

        }
        private class ShowIndexComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return -(((Field)x).ShowIndex - ((Field)y).ShowIndex);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Window_Conn win_conn = new Window_Conn();
            win_conn.OnServerConnected += new EventHandler(win_conn_OnServerConnected);
            win_conn.ShowDialog();
        }

        void win_conn_OnServerConnected(object sender, EventArgs e)
        {
            DBAccess db = (DBAccess)sender;
            this.manager.Db = db;
            showItems(1);
        }

        private void showItems(int page)
        {
            btn_first.IsEnabled = false;
            btn_last.IsEnabled = false;
            btn_next.IsEnabled = false;
            btn_prev.IsEnabled = false;
            LB_page.IsEnabled = false;
            IMG_loading.Visibility = System.Windows.Visibility.Visible;
            this.pager.CurrentPage = page;

            string id = null;
            string name = null;
            try
            {
                int.Parse(TB_ItemName.Text);
                id = TB_ItemName.Text;
            }
            catch
            {
                name = TB_ItemName.Text;
            }

            Thread t = new Thread(new ParameterizedThreadStart(doSearchItem));
            t.IsBackground = true;
            t.Start(new string[] { id, null, null, name });
        }

        void doSearchItem(object obj)
        {
            EventHandler handler = new EventHandler(invokeSearchItem);
            string id = null;
            string type = null;
            string subtype = null;
            string name = null;
            if (obj != null)
            {
                string[] condition = obj as string[];
                if (condition.Length == 4)
                {
                    id = condition[0];
                    type = condition[1];
                    subtype = condition[2];
                    name = condition[3];
                }
            }
            try
            {
                int totalCount = this.manager.getItmesCount(id, type, subtype, name);
                this.pager.TotalCount = totalCount;
                Record[] items = this.manager.getItems(id, type, subtype, name, this.pager.Start, this.pager.Limit);
                DataTable dt = ObjectBuilder.Items2DataTable(items);
                this.Dispatcher.Invoke(handler, new object[] { dt, null });
            }
            catch (Exception err)
            {
                this.Dispatcher.Invoke(handler, new object[] { err, null });
            }
        }

        void invokeSearchItem(object sender, EventArgs e)
        {
            btn_first.IsEnabled = this.pager.CanFirst;
            btn_last.IsEnabled = this.pager.CanLast;
            btn_next.IsEnabled = this.pager.CanNext;
            btn_prev.IsEnabled = this.pager.CanPrev;
            TB_page.IsEnabled = true;
            IMG_loading.Visibility = System.Windows.Visibility.Hidden;
            TB_page.Text = this.pager.CurrentPage.ToString();
            LB_page.Content = "/" + this.pager.TotalPage;
            if (sender.GetType() == typeof(DataTable))
            {
                DataTable dt = sender as DataTable;
                dataGrid1.ItemsSource = dt.DefaultView;
            }
            else if (sender.GetType().BaseType == typeof(SystemException))
            {
                Exception err = sender as Exception;
                MessageBox.Show("发生错误。\r\n" + err.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (sender.GetType().BaseType == typeof(DbException))
            {
                Exception err = sender as Exception;
                MessageBox.Show("数据库异常。\r\n" + err.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }



        private void btn_gendbc_Click(object sender, RoutedEventArgs e)
        {
            Window_dbcMaker win_mpq = new Window_dbcMaker(this.manager, "Item.dbc");
            win_mpq.Show();
        }



        private void menu_conn_Click(object sender, RoutedEventArgs e)
        {
            Window_Conn win_conn = new Window_Conn();
            win_conn.OnServerConnected += new EventHandler(win_conn_OnServerConnected);
            win_conn.ShowDialog();
        }


        private void DataGridHyperlinkColumn_Click(object sender, RoutedEventArgs e)
        {
            DataGrid g = sender as DataGrid;
            object value = g.SelectedValue;
            Hyperlink h = e.OriginalSource as Hyperlink;
            if (h == null || value == null)
                return;
            string oper = h.NavigateUri.OriginalString;
            string itemId = value.ToString();
            if (oper == "修改")
            {
                Window_ItemInfo win_iteminfo = new Window_ItemInfo(manager, itemId);
                win_iteminfo.OnItemChanged += new EventHandler(win_iteminfo_OnItemChanged);
                win_iteminfo.Show();
            }
            else if (oper == "删除")
            {
                MessageBoxResult r = MessageBox.Show("确认删除该物品？", this.Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (r == MessageBoxResult.Yes)
                {
                    IMG_loading.Visibility = System.Windows.Visibility.Visible;
                    string[] arr_itemId = new string[] { itemId };
                    Thread t = new Thread(new ParameterizedThreadStart(doDelItems));
                    t.IsBackground = true;
                    t.Start(arr_itemId);
                }
            }
        }

        private void btn_first_Click(object sender, RoutedEventArgs e)
        {
            showItems(1);
        }

        private void btn_prev_Click(object sender, RoutedEventArgs e)
        {
            showItems(this.pager.CurrentPage - 1);
        }

        private void btn_next_Click(object sender, RoutedEventArgs e)
        {
            showItems(this.pager.CurrentPage + 1);
        }

        private void btn_last_Click(object sender, RoutedEventArgs e)
        {
            showItems(this.pager.TotalPage);
        }

        private void TB_page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && IMG_loading.Visibility != System.Windows.Visibility.Visible)
            {
                string value = TB_page.Text;
                int page;
                if (int.TryParse(value, out page))
                    showItems(page);
            }
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            Window_ItemInfo win_iteminfo = new Window_ItemInfo(manager);
            win_iteminfo.OnItemChanged += new EventHandler(win_iteminfo_OnItemChanged);
            win_iteminfo.Show();
        }

        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            object obj = dataGrid1.SelectedValue;
            if (obj != null)
            {
                Window_ItemInfo win_iteminfo = new Window_ItemInfo(manager, obj.ToString());
                win_iteminfo.OnItemChanged += new EventHandler(win_iteminfo_OnItemChanged);
                win_iteminfo.Show();
            }
        }

        void win_iteminfo_OnItemChanged(object sender, EventArgs e)
        {
            showItems(this.pager.CurrentPage);
        }

        private void btn_del_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult r = MessageBox.Show("确认删除选中的物品？", this.Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                IMG_loading.Visibility = System.Windows.Visibility.Visible;
                ArrayList list = new ArrayList();
                string fieldName = Configuration.CURRENT_TABLE.Id_field_name;
                foreach (DataRowView drv in dataGrid1.SelectedItems)
                {
                    list.Add(drv[fieldName].ToString());
                }
                string[] arr_itemId = (String[])list.ToArray(typeof(String));
                Thread t = new Thread(new ParameterizedThreadStart(doDelItems));
                t.IsBackground = true;
                t.Start(arr_itemId);
            }
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int count = dataGrid1.SelectedItems.Count;
            btn_edit.IsEnabled = count == 1;
            btn_del.IsEnabled = count > 0;
        }

        void doDelItems(object obj)
        {
            if (obj == null)
                return;
            EventHandler handler = new EventHandler(invokeDelItems);
            string[] arr_itemId = obj as string[];
            try
            {
                foreach (string itemId in arr_itemId)
                {
                    this.manager.deleteItem(itemId);
                }
                this.Dispatcher.Invoke(handler, new object[] { obj, null });
            }
            catch (Exception err)
            {
                this.Dispatcher.Invoke(handler, new object[] { err, null });
            }
        }

        void invokeDelItems(object sender, EventArgs e)
        {
            IMG_loading.Visibility = System.Windows.Visibility.Hidden;
            if (sender.GetType() == typeof(string[]))
            {
                string fieldName = Configuration.CURRENT_TABLE.Id_field_name;
                string[] arr_itemId = sender as string[];
                DataView dv = dataGrid1.ItemsSource as DataView;
                foreach (string itemId in arr_itemId)
                {
                    for (int i = 0; i < dv.Count; i++)
                    {
                        DataRowView drv = dv[i];
                        if (drv[fieldName].ToString() == itemId)
                        {
                            dv.Delete(i);
                            break;
                        }
                    }
                }
            }
            else if (sender.GetType().BaseType == typeof(SystemException))
            {
                Exception err = sender as Exception;
                MessageBox.Show("发生错误。\r\n" + err.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (sender.GetType().BaseType == typeof(DbException))
            {
                Exception err = sender as Exception;
                MessageBox.Show("数据库异常。\r\n" + err.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            showItems(1);
        }

        private void btn_reset_Click(object sender, RoutedEventArgs e)
        {
            TB_ItemName.Text = string.Empty;
            showItems(1);
        }

        private void TB_ItemName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btn_search_Click(null, null);
            }
        }

        private void menu_color_tool(object sender, RoutedEventArgs e)
        {
            Window_GetColor wgc = new Window_GetColor();
            wgc.ShowDialog();

        }

        private void menu_ssvcp_Click(object sender, RoutedEventArgs e)
        {
            Window_ScalingStatValuesEntry w_ssv = new Window_ScalingStatValuesEntry();
            w_ssv.ShowDialog();
        }



        private void btn_dbc_open_click(object sender, RoutedEventArgs e)
        {
            string filename = Util.OpenFile("dbc文件|*.dbc|所有文件|*.*");

            Console.WriteLine(filename);

            dataGrid2.Columns.Clear();

            DataTable dt = DBCWriter.GetData(filename);

            int i = 0;
            foreach (DataColumn c in dt.Columns)
            {
                DataGridTextColumn col = new DataGridTextColumn();
                col.IsReadOnly = true;
                col.Header = c.ColumnName;
                col.Binding = new Binding(c.ColumnName);

                this.dataGrid2.Columns.Insert(i++, col);
            }



            dataGrid2.ItemsSource = dt.DefaultView;
        }



    }
}
