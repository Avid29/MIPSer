// Avishai Dernis 2025

using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.Foundation;

namespace Mipser.Editors.AssemblyEditBox;

public partial class AssemblyEditBox
{
    private void AssemblyEditBox_Loaded(object sender, RoutedEventArgs e)
    {
        // While loaded, detach the loaded event and attach unloaded event
        this.Loaded -= AssemblyEditBox_Loaded;
        this.Unloaded += AssemblyEditBox_Unloaded;

        TextChanging += AssemblyEditBox_TextChanging;
        TextChanged += AssemblyEditBox_TextChanged;
        SelectionChanging += AssemblyEditBox_SelectionChanging;
        SelectionChanged += AssemblyEditBox_SelectionChanged;
    }

    private void AssemblyEditBox_Unloaded(object sender, RoutedEventArgs e)
    {
        // Restore the loaded event and detach unloaded event
        this.Loaded += AssemblyEditBox_Loaded;
        this.Unloaded -= AssemblyEditBox_Unloaded;
    }

    private async void AssemblyEditBox_TextChanging(RichEditBox sender, RichEditBoxTextChangingEventArgs args)
    {
        if (!args.IsContentChanging)
            return;
        
        Document.GetText(TextGetOptions.None, out var str);
        UpdateTextProperty(str);

        await UpdateSyntaxHighlightingAsync();
    }

    private void AssemblyEditBox_TextChanged(object sender, RoutedEventArgs e)
    {
    }

    private void AssemblyEditBox_SelectionChanging(RichEditBox sender, RichEditBoxSelectionChangingEventArgs args)
    {
        var start = args.SelectionStart;
        var end = start + args.SelectionLength;
        SelectedRange = new Range(start, end);
    }

    private void AssemblyEditBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        Document.Selection.GetRect(Microsoft.UI.Text.PointOptions.Transform, out Rect rect, out _);

        if (SelectedRange.End.Value - SelectedRange.Start.Value == 0)
        {
            // Highlight the line
            if (_selectedLineHighlightBorder is not null && _selectedLineHighlightBorder?.RenderTransform is TranslateTransform tt)
            {
                _selectedLineHighlightBorder.Visibility = Visibility.Visible;
                tt.Y = rect.Top + Padding.Top;
                _selectedLineHighlightBorder.Height = rect.Height + 2; // TODO: Remove 2 as a magic number
            }
        }
        else if (_selectedLineHighlightBorder is not null)
        {
            // Hide the line highlight
            _selectedLineHighlightBorder.Visibility = Visibility.Collapsed;
        }
    }
}
