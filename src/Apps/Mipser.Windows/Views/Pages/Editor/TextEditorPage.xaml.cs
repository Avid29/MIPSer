// Avishai Dernis 2025

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Controls;
using MIPS.Assembler.Tokenization.Models;
using Mipser.Messages.Editor.Enums;
using Mipser.ViewModels.Pages;
using Mipser.Windows.Controls.CodeEditor;

namespace Mipser.Windows.Views.Pages.Editor;

public sealed partial class TextEditorPage : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextEditorPage"/> class.
    /// </summary>
    public TextEditorPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the <see cref="FilePageViewModel"/>.
    /// </summary>
    public FilePageViewModel? ViewModel
    {
        get;
        set
        {
            UpdateEvents(value, ViewModel);
            field = value;
            UpdateBindings();
        }
    }

    private bool UseAssemblyEditor => ViewModel?.File?.Name.EndsWith(".asm") ?? false;

    private bool UseTextEditor => !UseAssemblyEditor;

    private void UpdateEvents(FilePageViewModel? newVM, FilePageViewModel? oldVM)
    {
        if (oldVM is not null)
        {
            oldVM.NavigateToTokenEvent -= ViewModel_NavigateToTokenEvent;
            oldVM.EditorOperationRequested -= ViewModel_EditorOperationRequested;
        }

        if (newVM is not null)
        {
            newVM.NavigateToTokenEvent += ViewModel_NavigateToTokenEvent;
            newVM.EditorOperationRequested += ViewModel_EditorOperationRequested;
        }
    }

    private void ViewModel_NavigateToTokenEvent(object? sender, SourceLocation e)
    {
        // Find editbox
        var asmEditor = this.FindDescendant<AssemblyEditor>();
        if (asmEditor is null)
            return;

        // Navigate to location
        asmEditor.NavigateToToken(e);
    }

    private void ViewModel_EditorOperationRequested(object? sender, EditorOperation e)
    {
        // Find editbox
        var codeEditor = this.FindDescendant<CodeEditor>();
        if (codeEditor is null)
            return;

        codeEditor.ApplyOperation(e);
    }

    private void UpdateBindings()
    {
        this.Bindings.Update();
    }
}
