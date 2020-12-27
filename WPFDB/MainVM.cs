using ObjectRecognizerLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using WPFDB;

namespace WPF
{
    public class MainVM : INotifyPropertyChanged, ObjectRecognizerLib.IViewResult, ObjectRecognizerLib.IViewResultDB
    {
        ApplicationContext db;
        public event PropertyChangedEventHandler PropertyChanged;

        public Dispatcher cDispatcher;

        public ObjectRecognizerLib.ObjectRecognizer objectRecognizer = null;
        public ObservableCollection<Tuple<string, int>> Labels { get; set; }
        public ObservableCollection<ObjectRecognizerLib.Result> Result { get; set; }
        public ObservableCollection<ObjectRecognizerLib.Result> SelectedResult { get; set; }
        public ObservableCollection<Tuple<string, int>> Stats { get; set; }
        Tuple<string, int> selectedItem;
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
            Stats = new ObservableCollection<Tuple<string, int>>();
            db = new ApplicationContext();
        }
        public void SetDirectory(string directory)
        {
            objectRecognizer = new ObjectRecognizerLib.ObjectRecognizer(directory, this);
        }
        // Взаимодействие на форме
        public void Start()
        {
            Result.Clear();
            Labels.Clear();

            ConcurrentQueue<String> fileNamesQueue = new ConcurrentQueue<string>();
            List<string> paths = new List<string>(Directory.GetFiles(objectRecognizer.ImageDirectory));
            Thread thread = new Thread(() =>
            {
                foreach(string path in paths)
                {
                    ObjectRecognizerLib.Result resultFromDB = CheckDB(path);
                    if (resultFromDB != null)
                        GetResult(resultFromDB);
                    else
                        fileNamesQueue.Enqueue(path);
                }
                if(fileNamesQueue.Count != 0)
                {
                    objectRecognizer = new ObjectRecognizerLib.ObjectRecognizer(fileNamesQueue, objectRecognizer.ImageDirectory, this);
                    objectRecognizer.isStopped.Reset();
                    objectRecognizer.StartThreads();
                }
            });
            thread.Start();
        }
        public void Stop()
        {
            objectRecognizer.isStopped.Set();
        }
        // Вспомогательные функции/процедуры
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
        // Реализация интерфейсов
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
        public void GetResultDB(ObjectRecognizerLib.Result result)
        {
            AddToDB(result);
            GetResult(result);
        }
        // Работа с базой данных
        void AddToDB(ObjectRecognizerLib.Result result)
        {
            lock(db)
            {
                ImageInfo newImage = new ImageInfo
                {
                    Path = result.Path,
                    Label = result.Label,
                    Confidence = result.Confidence,
                    Image = ObjectRecognizerLib.ObjectRecognizer.ImageToByteArray(result.Path)
                };
                db.Images.Add(newImage);
                db.SaveChanges();
            }
        }
        ObjectRecognizerLib.Result CheckDB(string path)
        {
            byte[] blob = ObjectRecognizerLib.ObjectRecognizer.ImageToByteArray(path);
            lock(db)
            {
                var images = db.Images.Where(p => p.Path == path).Select(p => p).ToList();
                if(images.Count != 0)
                {
                    foreach(var image in images)
                    {
                        if(blob.SequenceEqual(image.Image))
                        {
                            return new ObjectRecognizerLib.Result(image.Path, image.Label, image.Confidence);
                        }
                    }
                }
            }
            return null;
        }
        public void Statistics()
        {
            Stats.Clear();
            lock(db)
            {
                var images = db.Images.GroupBy(p => p.Label).Select(x => new
                {
                    Label = x.Key,
                    Count = x.Count()
                });
                foreach (var image in images)
                    Stats.Add(new Tuple<string, int>(image.Label, image.Count));
            }

        }
        public void ClearDB()
        {
            db.Clear();
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
