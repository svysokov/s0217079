using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace WPF
{
    public class MainVM : INotifyPropertyChanged, ObjectRecognizerLib.IViewResult
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Dispatcher cDispatcher;

        public ObjectRecognizerLib.ObjectRecognizer objectRecognizer;
        public ObservableCollection<Tuple<string, int>> Labels { get; set; }
        public ObservableCollection<ObjectRecognizerLib.Result> Result { get; set; }
        public ObservableCollection<ObjectRecognizerLib.Result> SelectedResult { get; set; }
        private Tuple<string, int> selectedItem;
        public Tuple<string, int> SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;

                if (value != null)
                {
                    SelectedResult =
                        new ObservableCollection<ObjectRecognizerLib.Result>(Result.Where(x => x.Label == selectedItem.Item1));
                    OnPropertyChanged("SelectedResult");
                }

            }
        }
        public MainVM(Dispatcher mainDispatcher)
        {
            cDispatcher = mainDispatcher;
            Labels = new ObservableCollection<Tuple<string, int>>();
            Result = new ObservableCollection<ObjectRecognizerLib.Result>();
            SelectedResult = new ObservableCollection<ObjectRecognizerLib.Result>();
        }
        public void SetDirectory(string directory)
        {
            objectRecognizer = new ObjectRecognizerLib.ObjectRecognizer(directory, this);
        }
        public void Start()
        {
            Result.Clear();
            Labels.Clear();
            objectRecognizer = new ObjectRecognizerLib.ObjectRecognizer(objectRecognizer.ImageDirectory, this);
            objectRecognizer.isStopped.Reset();
            Thread thread = new Thread(() => objectRecognizer.StartThreads());
            thread.Start();
        }
        public void Stop()
        {
            objectRecognizer.isStopped.Set();
        }
        int Check(string lable)
        {
            int index = -1;
            foreach (var item in Labels)
            {

                if (item.Item1.Equals(lable))
                {
                    return Labels.IndexOf(item);
                }
            }
            return index;
        }
        public void GetResult(ObjectRecognizerLib.Result result)
        {
            cDispatcher.Invoke(() =>
            {
                Result.Add(result);
                int index = Check(result.Label);
                if (index == -1)
                    Labels.Add(new Tuple<string, int>(result.Label, 1));
                else
                    Labels[index] = new Tuple<string, int>(Labels[index].Item1, Labels[index].Item2 + 1);
            });
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
