// Avishai Dernis 2025

using Microsoft.UI.Xaml.Controls;
using Mipser.ViewModels.Pages;
using System.Linq;

namespace Mipser.Windows.Views.Pages;

public sealed partial class CheatSheet : UserControl
{
    public CheatSheet()
    {
        InitializeComponent();
        DataContext = new CheatSheetViewModel();
    }

    private CheatSheetViewModel ViewModel => (CheatSheetViewModel)DataContext;

    private void UserControl_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // This is stupid, but whatever for now
        RegisterEncodingTable.CellData = ViewModel.GPRegisters.Select(x => (object)x).ToArray();
    }
}
