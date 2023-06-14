// Adam Dernis 2023

using Microsoft.UI.Xaml.Controls;

namespace Mipser.Windows.Controls.Editors;

/// <summary>
/// A control for editing binary files as a hex view.
/// </summary>
public sealed partial class HexEditor : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HexEditor"/> class.
    /// </summary>
    public HexEditor()
    {
        this.InitializeComponent();
    }
}
