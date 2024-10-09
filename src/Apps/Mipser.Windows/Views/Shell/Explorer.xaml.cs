// Adam Dernis 2024

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Mipser.Bindables.Files;
using Mipser.ViewModels.Views;

namespace Mipser.Windows.Views.Shell;

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
        DataContext = App.Current.Services.GetRequiredService<ExplorerViewModel>();
    }

    private ExplorerViewModel ViewModel => (ExplorerViewModel)DataContext;

    private async void TreeView_Expanding(TreeView sender, TreeViewExpandingEventArgs args)
    {
        if (args.Node.HasUnrealizedChildren && args.Item is BindableFolder folder)
        {
            await folder.LoadChildren();
        }
    }
}
