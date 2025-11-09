// Adam Dernis 2024

using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Mipser.Bindables.Files;
using Mipser.Messages.Navigation;
using Mipser.ViewModels.Pages;

namespace Mipser.Windows.Views.Pages;

/// <summary>
/// The explorer view.
/// </summary>
public sealed partial class Explorer : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Explorer"/> class.
    /// </summary>
    public Explorer()
    {
        this.InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<ExplorerViewModel>();
    }

    private ExplorerViewModel ViewModel => (ExplorerViewModel)DataContext;

    private void TreeViewItem_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is not TreeViewItem tvi)
            return;

        var node = TreeViewRoot.NodeFromContainer(tvi);
        if (node is null)
            return;

        if (node.Depth is 0)
        {
            TreeViewRoot.Expand(node);
        }
    }

    private void FileDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (sender is not TreeViewItem tvi || tvi.DataContext is not BindableFile file)
            return;

        Ioc.Default.GetRequiredService<IMessenger>().Send(new FileOpenRequestMessage(file));
    }

    private void FolderDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (sender is not TreeViewItem tvi)
            return;

        // Toggle expansion
        tvi.IsExpanded = !tvi.IsExpanded;
    }

    private async void TreeView_Expanding(TreeView sender, TreeViewExpandingEventArgs args)
    {
        if (args.Item is not BindableFolder folder || !folder.ChildrenNotLoaded)
            return;

        // Load children and ensure expansion
        await folder.LoadChildrenAsync();
        args.Node.IsExpanded = true;
    }
}
