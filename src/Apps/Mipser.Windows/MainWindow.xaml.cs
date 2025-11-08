// Adam Dernis 2024

using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Mipser.Services.Localization;
using Mipser.ViewModels;
using System;
using System.Runtime.InteropServices;
using Windows.Storage;

namespace Mipser.Windows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        this.InitializeComponent();

        ViewModel = App.Current.Services.GetRequiredService<WindowViewModel>();

        // Set the app flow direction
        var localization = Ioc.Default.GetRequiredService<ILocalizationService>();
        if (localization.IsRightToLeftLanguage && Content is FrameworkElement fe)
        {
            // Set app content flow direction to RTL
            fe.FlowDirection = FlowDirection.RightToLeft;

            // Set window style to RTL
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            if (hwnd is 0)
                return;

            var exStyle = GetWindowLongPtr(hwnd, -20);
            exStyle |= 0x00400000;
            SetWindowLongPtr(hwnd, -20, exStyle);
        }
    }

    private WindowViewModel ViewModel { get; }

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = false)]
    private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = false)]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
}
