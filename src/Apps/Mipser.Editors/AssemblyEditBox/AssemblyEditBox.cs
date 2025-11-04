// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Mipser.Editors.AssemblyEditBox;

/// <summary>
/// A modified <see cref="RichEditBox"/> to add assembly syntax-highlighting and other features.
/// </summary>
[TemplatePart(Name = SelectedLineHighlightBorderPartName, Type = typeof(Border))]
public partial class AssemblyEditBox : RichEditBox
{
    private const string SelectedLineHighlightBorderPartName = "SelectedLineHighlightBorder";

    private Border? _selectedLineHighlightBorder;

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

        _selectedLineHighlightBorder = GetTemplateChild(SelectedLineHighlightBorderPartName) as Border;

        this.Loaded += AssemblyEditBox_Loaded;
    }
}
