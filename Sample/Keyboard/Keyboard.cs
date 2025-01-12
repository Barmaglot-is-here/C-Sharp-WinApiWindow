using static WinApiWindow.Sample.WinApi;

namespace WinApiWindow.Sample;

internal static class Keyboard
{
    public static Action<Key>? OnKeyDown { get; set; }

    internal static void Connect()
    {
        HookProcedure hook = Hook;
        SetWindowsHookEx(WH_KEYBOARD, hook, IntPtr.Zero, GetCurrentThreadId());
    }

    private static IntPtr Hook(int nCode, IntPtr wParam, IntPtr lParam)
    {
        var key     = (Key)wParam;
        var flag    = lParam.ToInt64() >> 31 & 0xffff;

        if (flag == 0)
            OnKeyDown?.Invoke(key);

        return CallNextHookEx(0, nCode, wParam, lParam);
    }
}