namespace StudentPerformanceApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell())
        {
            Title = "Лабораторна робота №2 — Варіант 6: ІС «Успішність студентів»",
            Width = 1280,
            Height = 820
        };

#if WINDOWS
        window.HandlerChanged += (s, e) =>
        {
            if (window.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWindow)
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
                var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

                if (appWindow != null)
                {
                    bool isClosingConfirmed = false;
                    appWindow.Closing += async (sender, args) =>
                    {
                        if (isClosingConfirmed)
                            return;

                        args.Cancel = true;

                        if (window.Page is AppShell shell && shell.CurrentPage is MainPage mainPage)
                        {
                            bool confirm = await mainPage.ConfirmExitAsync();
                            if (confirm)
                            {
                                isClosingConfirmed = true;
                                Application.Current?.Quit();
                            }
                        }
                    };
                }
            }
        };
#endif

        return window;
    }
}
