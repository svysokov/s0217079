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
        public static RoutedCommand StopCommand = new RoutedCommand("Stop", typeof(WPF.MainWindow));
        public static RoutedCommand RecognizeCommand = new RoutedCommand("Recognize", typeof(WPF.MainWindow));
        string folderName;
        public MainWindow()
        {
            InitializeComponent();
            mainVM = new MainVM(this.Dispatcher);
            this.DataContext = mainVM;
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            var res = dialog.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                folderName = dialog.SelectedPath;
                mainVM.SetDirectory(folderName);
                TB_FolderName.Text = folderName;
            }
        }
        private void Recognize_CommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            mainVM.Start();
        }
        private void Stop_CommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            mainVM.Stop();
        }
        private void Recognize_CanExecuteCommandHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (mainVM is null || mainVM.objectRecognizer is null)
                e.CanExecute = false;
            else
                e.CanExecute = true;
        }
        private void Stop_CanExecuteCommandHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (mainVM is null || mainVM.objectRecognizer is null)
                e.CanExecute = false;
            else
                e.CanExecute = true;
        }
        private void Staticstics_Click(object sender, RoutedEventArgs e)
        {
            mainVM.Statistics();
        }
        private void ClearDB_Click(object sender, RoutedEventArgs e)
        {
            mainVM.ClearDB();
        }
    }
}
