using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Kanbanned.UserControlsHelpers
{
    /// <summary>
    /// Interaction logic for BiranjeJezika.xaml
    /// </summary>
    public partial class BiranjeJezika : MetroWindow
    {
        public BiranjeJezika()
        {
            InitializeComponent();
        }

        public String Jezik { get; set; }
        private void English_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Jezik = "EN";
        }

        private void Serbian_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Jezik = "RS";
        }
    }
}
