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
using System.Windows.Shapes;

namespace WowItemCreator
{
    /// <summary>
    /// Windows_GetColor.xaml 的交互逻辑
    /// </summary>
    public partial class Window_GetColor : Window
    {
        public Window_GetColor()
        {
            InitializeComponent();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.ColorDialog dialog = new System.Windows.Forms.ColorDialog();
            System.Windows.Forms.DialogResult r = dialog.ShowDialog();
            string colorstr = "|CFF";
            if (r == System.Windows.Forms.DialogResult.OK)
            {
                colorstr += to16(dialog.Color);
            }
            this.color_text.Text = colorstr;
            Color color=new Color();
            color.R = dialog.Color.R;
            color.G = dialog.Color.G;
            color.B = dialog.Color.B;
            color.A = dialog.Color.A;
            // Brush brush =(Brush) new BrushConverter().ConvertFrom("#"+ to16(dialog.Color));
            Brush brush = new SolidColorBrush(color);
            //brush.Freeze();
            this.color_label.Background = brush;
           
            dialog.Dispose();
            
        }
  
        private string to16(System.Drawing.Color c)
        {
            return to16(c.R) + to16(c.G) + to16(c.B);
        }

        private string to16(int i)
        {
            string r = Convert.ToString(i, 16);
            if (r.Length == 1)
                r = "0" + r;
            return r.ToUpper();
        }
    }
}
