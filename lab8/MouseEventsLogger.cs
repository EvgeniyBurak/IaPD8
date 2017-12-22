using EventHook;

namespace lab8
{
    class MouseEventsLogger : EventLogger
    {
        private const string LeftButtonDown = "WM_LBUTTONDOWN";
        private const string RightButtonDown = "WM_RBUTTONDOWN";

        protected override string FileName => "MouseEventsLogFile";

        public void StartLogger()
        {
            MouseWatcher.OnMouseInput += (s, e) =>
            {
                if (CheckEvent(e.Message.ToString())) // Button down event 
                {
                    WriteLog($"Mouse: {e.Message.ToString()} at point {e.Point.x},{e.Point.y}");
                }
            };
            MouseWatcher.Start();
        }

        public void StopLogger() => MouseWatcher.Stop();

        private bool CheckEvent(string message) =>
            message.Equals(LeftButtonDown) || message.Equals(RightButtonDown);
    }
}
