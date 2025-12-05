// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using System.Collections.Generic;

namespace Mipser.Windows.Helpers;

public static class WindowHelper
{
    private readonly static HashSet<MipserWindow> _openWindows = [];

    /// <summary>
    /// Creates a new <see cref="MipserWindow"/>.
    /// </summary>
    /// <typeparam name="T">The specific type of window created</typeparam>
    /// <returns>The <see cref="MipserWindow"/> created.</returns>
    public static T CreateWindow<T>()
        where T : MipserWindow, new()
    {
        var window = new T();
        TrackWindow(window);
        return window;
    }

    public static MipserWindow? GetWindowForElement(UIElement element)
    {
        if (element.XamlRoot is null)
            return null;

        foreach (MipserWindow window in _openWindows)
        {
            if (element.XamlRoot == window.Content.XamlRoot)
                return window;
        }

        return null;
    }

    private static void TrackWindow(MipserWindow window)
    {
        window.Closed += (sender, args) =>
        {
            _openWindows.Remove(window);
        };

        _openWindows.Add(window);
    }
}
