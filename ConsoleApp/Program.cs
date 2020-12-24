using ObjectRecognizerLib;
using System;
using System.IO;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        public class ConsoleGetResult : IViewResult
        {
            public void GetResult(Result result)
            {
                Console.WriteLine(result);
            }
        }
        static void Main(string[] args)
        {

            DirectoryInfo myDirectory = new DirectoryInfo(
                Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName);
            ObjectRecognizer objectRecognizer = new ObjectRecognizer(myDirectory, "Images", "ObjectRecognizerLib", new ConsoleGetResult());
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                objectRecognizer.isStopped.Set();
                eArgs.Cancel = true;
            };
            objectRecognizer.StartThreads();
        }
    }
}
