using System;
using System.Windows.Forms;

namespace lab8
{
    class EventLogger
    {
        public TextBox EventTextBox { private get; set; }
        public TextBox TimeTextBox { private get; set; }
        public string DirrPath { private get; set; }
        public long MaxFileSize { private get; set; }
        protected virtual string FileName => "log";
        private object Lock { get; set; }
        private int FileNumber { get; set; }
        private string FullPathWithoutExtension => DirrPath + FileName;
        private string FullPath => FullPathWithoutExtension + ".txt";
        public string DestinationEmail { get; set; }

        protected void WriteLog(string message)
        {
            Lock = Lock ?? new object();
            lock (Lock)
            {
                try
                {
                    FilesOperations.FixOrCreate(FullPath);
                    if (FilesOperations.ContentLength(FullPath) > MaxFileSize) // Send by email is required
                    {
                        var newPath = $"{FullPathWithoutExtension}_File{FileNumber++}.txt";
                        FilesOperations.Move(FullPath, newPath);
                    }
                    FilesOperations.Write(FullPath, $"{DateTime.Now.ToString()} {message}", true);
                }
                catch (Exception)
                {
                    // ignored
                }

                EventTextBox.Text = message;
                TimeTextBox.Text = DateTime.Now.ToString();
            }
        }
    }
}
