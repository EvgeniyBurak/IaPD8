using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace lab8
{
    class LogFilesRenameWatcher
    {
        public string DirrectoryPath { private get; set; }
        public string DestinationAddress { private get; set; }

        private FileSystemWatcher _watcher;

        private const string AuthorEmail = "burak.eauheni@gmail.com";
        private const string AuthorPassword = "";

        public void StartWatcher()
        {
            _watcher = new FileSystemWatcher
            {
                Path = DirrectoryPath,
                NotifyFilter = NotifyFilters.FileName,
                Filter = "*.txt"
            };
            ChangeWatcherStatus(true);
        }

        public void StopWatcher() => ChangeWatcherStatus(false);

        private void ChangeWatcherStatus(bool addEvent)
        {
            if (_watcher != null)
            {
                if (addEvent)
                {
                    _watcher.Renamed += OnRenamed;
                }
                else
                {
                    _watcher.Renamed -= OnRenamed;
                }
                _watcher.EnableRaisingEvents = addEvent;    // Enable events
            }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            Thread.Sleep(20);
            SendFile(e.FullPath);   // Send file after renaming
        }

        private void SendFile(string path)
        {
            try
            {
                var mail = new MailMessage
                {
                    From = new MailAddress(AuthorEmail),
                    Subject = "Log file!",
                    Body = "Sending log file!",
                };
                mail.To.Add(DestinationAddress);
                mail.Attachments.Add(new Attachment(path));

                var smtpServer = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Credentials = new NetworkCredential(AuthorEmail, AuthorPassword),
                    EnableSsl = true
                };

                smtpServer.SendCompleted += SmtpServer_SendCompleted;
                smtpServer.SendAsync(mail, mail);   // Mail as userToken to get file name in SendComplit event
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void SmtpServer_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var name = ((MailMessage)e.UserState).Attachments[0].Name;  // Get file name
            ((MailMessage)e.UserState).Dispose();
            FilesOperations.Delete(DirrectoryPath + name);  // Delete file after sending complite
        }
    }
}