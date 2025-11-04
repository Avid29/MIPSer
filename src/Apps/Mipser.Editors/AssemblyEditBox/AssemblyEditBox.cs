// Avishai Dernis 2025

using Microsoft.UI.Xaml.Controls;

namespace Mipser.Editors.AssemblyEditBox;

/// <summary>
/// A modified <see cref="RichEditBox"/> to add assembly syntax-highlighting and other features.
/// </summary>
public partial class AssemblyEditBox : RichEditBox
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyEditBox"/> class.
    /// </summary>
    public AssemblyEditBox()
    {
        DefaultStyleKey = typeof(AssemblyEditBox);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        this.Loaded += AssemblyEditBox_Loaded;
    }
}
