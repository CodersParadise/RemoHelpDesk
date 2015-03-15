
namespace ConsoleClient.Tools
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    public static class InputController
    {
        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(int xPoint, int yPoint);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(SystemMetric smIndex);

        private struct INPUT
        {
            public InputType Type;
            public MOUSEKEYBDHARDWAREINPUT Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;
        }

        private struct MOUSEINPUT
        {
            public Int32 X;
            public Int32 Y;
            public UInt32 MouseData;
            public MouseEventFlags Flags;
            public UInt32 Time;
            public IntPtr ExtraInfo;
        }

        private enum SystemMetric
        {
            SM_CXVIRTUALSCREEN = 78,
            SM_CYVIRTUALSCREEN = 79
        }

        private enum MouseEventFlags : uint
        {
            MOUSEEVENTF_MOVE = 0x0001,
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            MOUSEEVENTF_RIGHTUP = 0x0010,
            MOUSEEVENTF_MIDDLEDOWN = 0x0020,
            MOUSEEVENTF_MIDDLEUP = 0x0040,
            MOUSEEVENTF_XDOWN = 0x0080,
            MOUSEEVENTF_XUP = 0x0100,
            MOUSEEVENTF_WHEEL = 0x0800,
            MOUSEEVENTF_VIRTUALDESK = 0x4000,
            MOUSEEVENTF_ABSOLUTE = 0x8000
        }

        private enum InputType : uint
        {
            MOUSE = 0
        }

        private static int CalculateAbsoluteCoordinateX(int x)
        {
            return (x * 65536) / GetSystemMetrics(SystemMetric.SM_CXVIRTUALSCREEN);
        }

        private static int CalculateAbsoluteCoordinateY(int y)
        {
            return (y * 65536) / GetSystemMetrics(SystemMetric.SM_CYVIRTUALSCREEN);
        }

        public static void ClickOnPoint(int x, int y)
        {
            INPUT inputMouseMove = new INPUT();
            inputMouseMove.Type = InputType.MOUSE;
            inputMouseMove.Data.Mouse.Flags = MouseEventFlags.MOUSEEVENTF_MOVE | MouseEventFlags.MOUSEEVENTF_ABSOLUTE | MouseEventFlags.MOUSEEVENTF_VIRTUALDESK;
            inputMouseMove.Data.Mouse.X = CalculateAbsoluteCoordinateX(x);
            inputMouseMove.Data.Mouse.Y = CalculateAbsoluteCoordinateY(y);

            INPUT inputMouseDown = new INPUT();
            inputMouseDown.Type = InputType.MOUSE;
            inputMouseDown.Data.Mouse.Flags = MouseEventFlags.MOUSEEVENTF_LEFTDOWN | MouseEventFlags.MOUSEEVENTF_ABSOLUTE | MouseEventFlags.MOUSEEVENTF_VIRTUALDESK;

            INPUT inputMouseUp = new INPUT();
            inputMouseUp.Type = InputType.MOUSE;
            inputMouseUp.Data.Mouse.Flags = MouseEventFlags.MOUSEEVENTF_LEFTUP | MouseEventFlags.MOUSEEVENTF_ABSOLUTE | MouseEventFlags.MOUSEEVENTF_VIRTUALDESK;

            INPUT[] inputs = new INPUT[] { inputMouseMove, inputMouseDown, inputMouseUp };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

    }
}