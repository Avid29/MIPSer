// Avishai Dernis 2025

using Microsoft.UI.Xaml.Controls;
using Mipser.ViewModels.Pages.App.Settings;

namespace Mipser.Windows.Views.Pages.App.SettingsSubPages;

/// <summary>
/// The app settings subpage
/// </summary>
public sealed partial class EditorSettingsSubPage : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditorSettingsSubPage"/> class.
    /// </summary>
    public EditorSettingsSubPage()
    {
        this.InitializeComponent();
    }

    public EditorSettingsViewModel? ViewModel { get; set; }

}
