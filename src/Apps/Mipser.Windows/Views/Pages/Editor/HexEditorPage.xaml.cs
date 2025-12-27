// Avishai Dernis 2025

using Microsoft.UI.Xaml.Controls;
using Mipser.ViewModels.Pages;


namespace Mipser.Windows.Views.Pages.Editor;

public sealed partial class HexEditorPage : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HexEditorPage"/> class.
    /// </summary>
    public HexEditorPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the <see cref="FilePageViewModel"/>.
    /// </summary>
    public FilePageViewModel? ViewModel { get; set; }
}
