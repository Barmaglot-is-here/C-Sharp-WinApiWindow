using System.Runtime.InteropServices;

namespace WinApiWindow;

internal static class WinApi
{
    public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    #region Constants
    public const uint WS_OVERLAPPEDWINDOW   = 0xcf0000;
    public const uint WS_VISIBLE            = 0x10000000;
    public const uint WS_CAPTION            = 0x00C00000;
    public const uint WS_THICKFRAME         = 0x00040000;
    public const uint WS_EX_APPWINDOW       = 0x00040000;

    public const int MONITOR_DEFAULTTONEAREST = 0x00000002;

    public const int CS_VREDRAW             = 1;
    public const int CS_HREDRAW             = 2;

    public const int COLOR_BACKGROUND       = 1;

    public const int IDC_ARROW              = 32512;

    public const int WM_DESTROY             = 2;
    public const int WM_SIZE                = 0x0005;
    public const int WM_MOVE                = 0x0003;
    public const int WM_DISPLAYCHANGE       = 0x007E;
    public const int WM_KEYDOWN             = 0x0100;
    public const int WM_KEYUP               = 0x0101;

    public const int SW_HIDE                = 0;
    public const int SW_NORMAL              = 1;
    public const int SW_MAXIMIZE            = 3;
    public const int SW_MINIMIZE            = 6;
    public const int SW_RESTORE             = 9;
    
    public const int GWL_STYLE              = -16;
    #endregion

    #region Structs
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WNDCLASSEXW
    {
        public uint cbSize;
        public uint style;
        public WndProcDelegate lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public string? lpszMenuName;
        public string lpszClassName;
        public IntPtr hIconSm;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public UIntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public POINT pt;
        public uint lPrivate;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    public struct WINDOWPLACEMENT
    {
        public uint length;
        public uint flags;
        public uint showCmd;
        public POINT ptMinPosition;
        public POINT ptMaxPosition;
        public RECT rcNormalPosition;
        public RECT rcDevice;
    }

    public struct MONITORINFO
    {
        public uint cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }
    #endregion

    #region Functions
    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern UInt16 RegisterClassExW([In] ref WNDCLASSEXW lpWndClass);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern IntPtr LoadCursorW(IntPtr hInstance, uint lpCursorName);

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern IntPtr CreateWindowExA(
       uint dwExStyle,
       IntPtr lpClassName,
       string lpWindowName,
       uint dwStyle,
       int x,
       int y,
       int nWidth,
       int nHeight,
       IntPtr hWndParent,
       IntPtr hMenu,
       IntPtr hInstance,
       IntPtr lpParam);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern sbyte GetMessageW(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin,
    uint wMsgFilterMax);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern IntPtr DispatchMessageW([In] ref MSG lpmsg);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern IntPtr DefWindowProcW(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern void PostQuitMessage(int nExitCode);

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern bool DestroyWindow(IntPtr hWnd);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern uint GetWindowLongPtrW(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern int SetWindowLongPtrW(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, 
        int nWidth, int nHeight, bool bRepaint);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
    
    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern bool SetWindowPlacement(IntPtr hWnd, WINDOWPLACEMENT lpwndpl);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern bool SetWindowTextA(IntPtr hwnd, string lpString);
    
    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);
    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern bool GetMonitorInfoW(IntPtr hMonitor, ref MONITORINFO lpmi);

    #endregion

    public static IntPtr LOWORD(IntPtr lParam) => lParam & 0xffff;
    public static IntPtr HIWORD(IntPtr lParam) => (lParam >> 16) & 0xffff;
}