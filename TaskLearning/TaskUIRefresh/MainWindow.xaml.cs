using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace TaskUIRefresh
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            btn.Content = "Runing";

            ///Work Task
            Task workTask = new Task(() => {
                Thread.Sleep(1000);
            });

            ///Update Task
            Task updateTask = workTask.ContinueWith((argc) =>
            {
                btn.Content = "Run";
            },
            TaskScheduler.FromCurrentSynchronizationContext());

            ///Start Task
            workTask.Start();
        }
    }
}
