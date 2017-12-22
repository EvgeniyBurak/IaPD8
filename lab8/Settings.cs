using System;
using System.Text.RegularExpressions;

namespace lab8
{
    class Settings
    {
        public string DestinationEmail { get; private set; }
        public long MaxFileSize { get; private set; }
        public bool StartInNormalMode { get; private set; }
        private string FullPath { get; }
        private StringCrypter Crypter { get; }

        private const string Key = "key";

        // Settings file decrypted content
        private string FileContent =>
            $"{DestinationEmail} {MaxFileSize} {StartInNormalMode}";

        public Settings(string path)
        {
            FullPath = path;
            Crypter = new StringCrypter
            {
                CryptKey = Key
            };
            if (!SetSettingsFromFile())
            {
                SetDefaultSettings();
            }
        }

        private bool SetSettingsFromFile()
        {
            try
            {
                var settings = Crypter.Decrypt(FilesOperations.Read(FullPath)).Split(' ');
                if (settings.Length == 3) // Settings file isn`t broken
                {
                    var email = settings[0];
                    if (VerifyEmail(email))
                    {
                        DestinationEmail = email;
                        MaxFileSize = Convert.ToInt64(settings[1]);
                        StartInNormalMode = Convert.ToBoolean(settings[2]);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return false;
        }

        private void SetDefaultSettings()
        {
            DestinationEmail = "yauheni.burak@gmail.com";
            MaxFileSize = 2048;
            StartInNormalMode = true;
            FilesOperations.Write(FullPath, Crypter.Encrypt(FileContent), false);
        }

        public bool ChangeSettings(string emailAddress, string maxFileSize, bool startNormal)
        {
            var successFlag = true;
            try
            {
                if (VerifyEmail(emailAddress))
                {
                    DestinationEmail = emailAddress;
                }
                else
                {
                    successFlag = false;
                }
                MaxFileSize = Convert.ToInt64(maxFileSize);
                StartInNormalMode = Convert.ToBoolean(startNormal);
            }
            catch (Exception)
            {
                successFlag = false;
            }

            FilesOperations.Write(FullPath, Crypter.Encrypt(FileContent), false);
            return successFlag;
        }

        private bool VerifyEmail(string emailAddress)
        {
            try
            {
                return Regex.IsMatch(emailAddress, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
