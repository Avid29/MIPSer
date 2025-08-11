// Avishai Dernis 2025

using Microsoft.UI.Xaml.Controls;
using Mipser.ViewModels.Views;

namespace Mipser.Windows.Views;

public sealed partial class CheatSheet : UserControl
{
    public CheatSheet()
    {
        InitializeComponent();
        DataContext = new CheatSheetViewModel();
    }

    private CheatSheetViewModel ViewModel => (CheatSheetViewModel)DataContext;
}
