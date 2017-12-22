using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace lab8
{
    class Locker
    {
        private bool Disabled = false;
        private Timer timer;

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }

        // System level functions to be used for hook and unhook keyboard input
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern short GetAsyncKeyState(Keys key);

        private IntPtr ptrHook = IntPtr.Zero;
        private LowLevelKeyboardProc objKeyboardProcess = null;
        private Keys prevKey = 0;

        public void StartLocker()
        {
            // Get Current Module
            ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
            // Assign callback function each time keyboard process
            objKeyboardProcess = new LowLevelKeyboardProc(CaptureKey);
            // Setting Hook of Keyboard Process for current module  
            ptrHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);
        }

        public void StopLocker()
        {
            if (ptrHook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(ptrHook);
                ptrHook = IntPtr.Zero;
            }
        }

        private IntPtr CaptureKey(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));

                if (objKeyInfo.flags != 128 && objKeyInfo.flags != 129) // Key Down
                {
                    if (objKeyInfo.key == Keys.F12 && !Disabled)  // Key Space
                    {
                        Disabled = true;
                        timer = new Timer
                        {
                            Interval = 2 * 1000 // 2 Sec
                        };
                        timer.Tick += timer_Tick;
                        timer.Start();
                    }
                }
            }

            return Disabled ? (IntPtr)1 : CallNextHookEx(ptrHook, nCode, wp, lp);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Disabled = false;
            timer.Stop();
        }
    }
}
