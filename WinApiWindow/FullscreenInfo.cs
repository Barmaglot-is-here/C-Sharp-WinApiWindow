using static WinApiWindow.WinApi;

namespace WinApiWindow;
internal struct FullscreenInfo
{
    public WINDOWPLACEMENT WindowPlacement;
    public uint WindowStyle;
}