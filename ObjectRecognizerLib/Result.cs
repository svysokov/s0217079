using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ObjectRecognizerLib
{
    public class Result : INotifyPropertyChanged
    {
        string path;
        string label;
        public event PropertyChangedEventHandler PropertyChanged;
        public string Path 
        { 
            get
            {
                return path;
            }
            set
            {
                path = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Path"));
            }
        }
        public string Label 
        { 
            get
            {
                return label; 
            }
            set
            {
                label = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
            }
        }
        public float Confidence { get; set; }
        public Result(string path, string label, float confidence)
        {
            Path = path; Label = label; Confidence = confidence;
        }
        public override string ToString()
        {
            return $"Path = {Path}\nLabel = {Label}\nConfidence = {Confidence}";
        }
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
