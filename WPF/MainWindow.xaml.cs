using ObjectRecognizerLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainVM mainVM;
        string folderName;
        public MainWindow()
        {
            InitializeComponent();

            mainVM = new MainVM(this.Dispatcher);
            this.DataContext = mainVM;
            Btn_Recognize.IsEnabled = Btn_Stop.IsEnabled = false;
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            var res = dialog.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
                folderName = dialog.SelectedPath;
            mainVM.SetDirectory(folderName);
            TB_FolderName.Text = folderName;
            Btn_Recognize.IsEnabled = Btn_Stop.IsEnabled = true;
        }
        private void Recognize_Click(object sender, RoutedEventArgs e)
        {
            mainVM.Start();
        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            mainVM.Stop();
        }
    }
}
