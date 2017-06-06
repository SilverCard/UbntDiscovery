using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UbntDiscovery
{
    public partial class MainWindow : Window
    {
        public MainWindowModel Model;

        public MainWindow()
        {
            Model = new MainWindowModel(this);
            this.DataContext = Model;

            InitializeComponent();

            this.MinHeight = this.Height;
            this.MinWidth = this.Width;
            this.Title =  String.Format("{0} v{1}", this.Title, Assembly.GetEntryAssembly().GetName().Version);
        }


        private void ExitClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            this.Model.ClearDevices();
        }

        private void ScanClick(object sender, RoutedEventArgs e)
        {
            this.Model.Scan();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.Model.Scan();
        }
    }
}
