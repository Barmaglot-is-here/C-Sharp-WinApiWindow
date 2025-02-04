using WinApiWindow;

Window window = new("TestWindow", 600, 600, 200);

window.OnPositionChanged    += (x, y) => Console.WriteLine($"x: {x}, y: {y}");
window.OnSizeChanged        += (x, y) => Console.WriteLine($"width: {x}, height: {y}");

Console.WriteLine(Convert.ToString(0xffff, 2));

window.OnKeyDown += key =>
{
    switch (key)
    {
        case Key.Q:
            window.Close();

            break;
        case Key.F:
            window.Fullscreen(!window.FullscreenMode);

            break;
        case Key.T:
            window.SetTitle("Title2");

            break;
        case Key.M:
            window.Minimize();

            break;
        case Key.R:
            window.Show();

            break;
        default:
            Console.WriteLine(key);

            break;
    }
};

bool update = true;
while (update)
    window.Update(out update);