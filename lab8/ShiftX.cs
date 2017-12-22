using EventHook;

namespace lab8
{
    class ShiftX
    {
        private bool _shiftPressed;
        private bool _somethingElse;

        private const string LShiftKeyName = "LeftShift";
        private const string RShiftKeyName = "RightShift";
        private const string X = "X";

        public void AnalyseEvent(KeyData keyData, Form1 form1)
        {
            var isKeyDownEvent = keyData.EventType.ToString().Equals("down");

            switch (keyData.Keyname)
            {
                case RShiftKeyName:
                case LShiftKeyName:
                    _somethingElse = false;
                    _shiftPressed = isKeyDownEvent;
                    break;

                case X:
                    if (_shiftPressed && isKeyDownEvent && !_somethingElse)
                    {
                        _shiftPressed = false;
                        form1.IsVisible = !form1.IsVisible;
                    }
                    break;

                default:
                    _somethingElse = true;
                    break;
            }
        }
    }
}
