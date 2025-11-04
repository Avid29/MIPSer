// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Mipser.Editors.AssemblyEditBox;

public partial class AssemblyEditBox
{
    private void AssemblyEditBox_Loaded(object sender, RoutedEventArgs e)
    {
        // While loaded, detach the loaded event and attach unloaded event
        this.Loaded -= AssemblyEditBox_Loaded;
        this.Unloaded += AssemblyEditBox_Unloaded;

        TextChanging += AssemblyEditBox_TextChanging;
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

        await UpdateSyntaxHighlighting();
    }
}
