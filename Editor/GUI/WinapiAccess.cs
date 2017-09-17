using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Editor.GUI.Controls;

namespace Editor.GUI
{
    //TODO: refactor
    /// <summary>
    /// Partial implementation of winapi mouse and keyboard events.
    /// </summary>
    class WinapiAccess
    {
        public delegate void GlobalKeyUpEvent(object sender, KeyEventArgs e);
        public static event GlobalKeyUpEvent GlobalKeyUp;

        public delegate void GlobalKeyDownEvent(object sender, KeyEventArgs e);
        public static event GlobalKeyDownEvent GlobalKeyDown;

        public delegate void GlobalMouseMoveEvent(object sender, MouseEventArgs e);
        public static event GlobalMouseMoveEvent GlobalMouseMove;

        private delegate IntPtr LowLevelevent(int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;
        
        private static IntPtr _keyboardHookId = IntPtr.Zero;
        private static IntPtr _mouseHookId = IntPtr.Zero;

        //Theese references must be kept so that the managed GC does not delete delegates
        //that were sent to the low-level unmanaged winapi code
        private static readonly LowLevelevent KeyboardHookKeepAliveReference = KeyboardHookCallback;
        private static readonly LowLevelevent MouseHookKeepAliveReference = MouseHookCallback;

        public static void Activate()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                _keyboardHookId = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookKeepAliveReference, GetModuleHandle(curModule.ModuleName), 0);
                _mouseHookId = SetWindowsHookEx(WH_MOUSE_LL, MouseHookKeepAliveReference, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public static void Suspend()
        {
            UnhookWindowsHookEx(_keyboardHookId);
            UnhookWindowsHookEx(_mouseHookId);
        }

        private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                MouseEventArgs ma;
                switch ((MouseMessages) wParam)
                {
                    case MouseMessages.WM_MOUSEMOVE:
                        ma = new MouseEventArgs(
                            MouseButtons.None,
                            0,
                            hookStruct.coords.X, hookStruct.coords.Y,
                            0);

                        GlobalMouseMove?.Invoke(null, ma);
                        break;
                }
            }

            return CallNextHookEx(_mouseHookId, nCode, wParam, lParam);
        }

        private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT hookStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));

                Keys keys = (Keys) hookStruct.vkCode;
                if ((GetKeyState(0x10) & 0x8000) != 0) keys |= Keys.Shift;
                if ((GetKeyState(0x11) & 0x8000) != 0) keys |= Keys.Control;
                if ((GetKeyState(0x12) & 0x8000) != 0) keys |= Keys.Alt;

                if ((KeyboardMessages)wParam == KeyboardMessages.WM_KEYUP)
                    GlobalKeyUp?.Invoke(null, new KeyEventArgs(keys));

                if ((KeyboardMessages)wParam == KeyboardMessages.WM_KEYDOWN)
                    GlobalKeyDown?.Invoke(null, new KeyEventArgs(keys));
            }

            return CallNextHookEx(_keyboardHookId, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelevent lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT coords;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        private enum KeyboardMessages
        {
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101
        }

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }
    }
}
