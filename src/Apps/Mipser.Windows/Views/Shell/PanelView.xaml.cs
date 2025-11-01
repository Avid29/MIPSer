// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using Mipser.ViewModels;
using Mipser.ViewModels.Views.Abstract;

namespace Mipser.Windows.Views.Shell;

/// <summary>
/// The main content tab view.
/// </summary>
public sealed partial class PanelView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PanelView"/> class.
    /// </summary>
    public PanelView()
    {
        this.InitializeComponent();
    }

    private PanelViewModel ViewModel => (PanelViewModel)DataContext;

    private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        if (args.Item is not PageViewModel)
            ThrowHelper.ThrowInvalidDataException();

        ViewModel.OpenPages.Remove((PageViewModel)args.Item);
    }
}
