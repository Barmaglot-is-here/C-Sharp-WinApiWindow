using System.Runtime.InteropServices;

namespace WinApiWindow.Sample;
internal static class WinApi
{
    public delegate IntPtr HookProcedure(int nCode, IntPtr wParam, IntPtr lParam);

    public const int WH_KEYBOARD = 2;

    [DllImport("user32.dll", CharSet = CharSet.Auto,
    CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern IntPtr SetWindowsHookEx(int idHook, HookProcedure lpfn, 
        IntPtr hMod, int dwThreadId);
    
    [DllImport("user32.dll", CharSet = CharSet.Auto,
    CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern IntPtr CallNextHookEx(int hhk, int nCode, IntPtr wParam, 
        IntPtr lParam);

    [DllImport("kernel32")]
    public static extern int GetCurrentThreadId();
}