// Adam Dernis 2024

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
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
}
