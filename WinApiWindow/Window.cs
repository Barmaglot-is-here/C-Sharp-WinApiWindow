using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static WinApiWindow.WinApi;

namespace WinApiWindow;

public class Window
{
    private string _title;

    private int _positionX;
    private int _positionY;

    private int _width;
    private int _height;

    private readonly WndProcDelegate _wndProc;

    private FullscreenInfo _fullscreenInfo;

    public IntPtr Handle { get; }

    public string Title 
    {
        get => _title;
        set => SetTitle(value);
    }

    public int PositionX
    {
        get => _positionX;
        set => SetPosition(value, _positionY);
    }

    public int PositionY
    {
        get => _positionX;
        set => SetPosition(_positionX, value);
    }

    public int Width 
    {
        get => _width;
        set => SetSize(value, _height);
    }
    public int Height
    {
        get => _height;
        set => SetSize(_width, value);
    }

    public Action<int, int>? OnSizeChanged { get; set; }
    public Action<int, int>? OnPositionChanged { get; set; }

    public Action<Key>? OnKeyDown { get; set; }
    public Action<Key>? OnKeyUp { get; set; }

    public bool FullscreenMode { get; private set; }

    public Window(string title = "Window", int width = 480, int height = 600, 
                  int positionX = 0, int positionY = 0)
    {
        IntPtr handle   = Process.GetCurrentProcess().Handle;
        _wndProc        = WndProc;

        WNDCLASSEXW wc      = new();
        wc.cbSize           = (uint)Marshal.SizeOf(typeof(WNDCLASSEXW));
        wc.style            = CS_HREDRAW | CS_VREDRAW;
        wc.hbrBackground    = (IntPtr)COLOR_BACKGROUND + 1;
        wc.cbClsExtra       = 0;
        wc.cbWndExtra       = 0;
        wc.hInstance        = handle;
        wc.hIcon            = IntPtr.Zero;
        wc.hCursor          = LoadCursorW(IntPtr.Zero, IDC_ARROW);
        wc.lpszMenuName     = null;
        wc.lpszClassName    = title;
        wc.lpfnWndProc      = _wndProc;
        wc.hIconSm          = IntPtr.Zero;

        ushort atom = RegisterClassExW(ref wc);

        if (atom == 0)
            throw new Win32Exception(Marshal.GetLastWin32Error());

        IntPtr hWnd = CreateWindowExA(
            WS_EX_APPWINDOW,
            atom,
            title, 
            WS_OVERLAPPEDWINDOW | WS_VISIBLE, 
            positionX, positionY,
            width, height, 
            IntPtr.Zero, 
            IntPtr.Zero, 
            wc.hInstance,
            IntPtr.Zero);

        if (hWnd == 0)
            throw new Win32Exception(Marshal.GetLastWin32Error());

        Handle          = hWnd;
        FullscreenMode  = false;
        _title          = title;

        _fullscreenInfo                     = new();
        _fullscreenInfo.WindowPlacement     = new();
        _fullscreenInfo.WindowStyle         = GetWindowLongPtrW(Handle, GWL_STYLE);

        GetWindowPlacement(Handle, ref _fullscreenInfo.WindowPlacement);
    }

    public void Update(out bool result)
    {
        result = (GetMessageW(out MSG msg, IntPtr.Zero, 0, 0) != 0);

        if (result)
            DispatchMessageW(ref msg);
    }

    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        switch (msg)
        {
            case WM_DESTROY:
                DestroyWindow(hWnd);

                PostQuitMessage(0);

                break;
            case WM_DISPLAYCHANGE:
            case WM_SIZE:
                _width     = (int)LOWORD(lParam);
                _height    = (int)HIWORD(lParam);

                OnSizeChanged?.Invoke(_width, _height);

                break;
            case WM_MOVE:
                _positionX = (int)LOWORD(lParam);
                _positionY = (int)HIWORD(lParam);

                OnPositionChanged?.Invoke(_positionX, _positionY);

                break;
            case WM_KEYDOWN:
                Key key = (Key)wParam;

                OnKeyDown?.Invoke(key);

                break;
            case WM_KEYUP:
                key = (Key)wParam;

                OnKeyUp?.Invoke(key);

                break;
            default:
                break;
        }

        return DefWindowProcW(hWnd, msg, wParam, lParam);
    }

    public void Maximize()  => ShowWindow(Handle, SW_MAXIMIZE);
    public void Minimize()  => ShowWindow(Handle, SW_MINIMIZE);
    public void Restore()   => ShowWindow(Handle, SW_RESTORE);
    public void Hide()      => ShowWindow(Handle, SW_HIDE);
    public void Show()      => ShowWindow(Handle, SW_NORMAL);
    public void Close()     => DestroyWindow(Handle);

    public void Fullscreen(bool mode = true)
    {
        if (mode)
        {
            uint gwlStyle = _fullscreenInfo.WindowStyle & ~(WS_CAPTION | WS_THICKFRAME);

            GetWindowPlacement(Handle, ref _fullscreenInfo.WindowPlacement);
            SetWindowLongPtrW(Handle, GWL_STYLE, gwlStyle);

            ToMonitorSize();
        }
        else
        {
            SetWindowPlacement(Handle, _fullscreenInfo.WindowPlacement);
            SetWindowLongPtrW(Handle, GWL_STYLE, _fullscreenInfo.WindowStyle);

            ShowWindow(Handle, _fullscreenInfo.WindowPlacement.showCmd);
        }

        FullscreenMode = mode;
    }

    private void ToMonitorSize()
    {
        IntPtr hmon     = MonitorFromWindow(Handle, MONITOR_DEFAULTTONEAREST);
        MONITORINFO mi  = new();
        mi.cbSize       = (uint)Marshal.SizeOf(mi);

        GetMonitorInfoW(hmon, ref mi);

        int x       = mi.rcMonitor.left;
        int y       = mi.rcMonitor.top;
        int nWidth  = mi.rcMonitor.right - mi.rcMonitor.left;
        int nHeight = mi.rcMonitor.bottom - mi.rcMonitor.top;

        MoveWindow(Handle, x, y, nWidth, nHeight, true);
    }

    public void SetPosition(int x, int y)
    {
        _positionX = x;
        _positionY = y;

        MoveWindow(Handle, _positionX, _positionY, _width, _height, true);

        OnPositionChanged?.Invoke(x, y);
    }

    public void SetSize(int width, int height)
    {
        _width  = width;
        _height = height;

        MoveWindow(Handle, PositionX, PositionY, width, height, true);

        OnSizeChanged?.Invoke(width, height);
    }

    public void SetTitle(string title)
    {
        _title = title;

        SetWindowTextA(Handle, title);
    }
}