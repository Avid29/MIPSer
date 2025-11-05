// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MIPS.Assembler.Models.Instructions;
using WinUIEditor;

namespace Mipser.Editors.AssemblyEditBox;

/// <summary>
/// A modified <see cref="RichEditBox"/> to add assembly syntax-highlighting and other features.
/// </summary>
[TemplatePart(Name = CodeEditorPartName, Type = typeof(CodeEditorControl))]
public partial class AssemblyEditBox : Control
{
    private const string CodeEditorPartName = "CodeEditor";

    private CodeEditorControl? _codeEditor;

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

        _codeEditor = (CodeEditorControl)GetTemplateChild(CodeEditorPartName);

        this.Loaded += AssemblyEditBox_Loaded;

        _codeEditor.Editor.SetText(Text);

        var table = new InstructionTable(MIPS.Models.Instructions.Enums.MipsVersion.MipsIII);
        SetupKeywords(table.GetInstructions());
    }
}
