// Avishai Dernis 2025

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Tokenization.Models;
using Mipser.Windows.Controls.AssemblyEditBox;
using Mipser.Messages.Editor.Enums;
using Mipser.ViewModels.Pages;
using System.Collections.Generic;

namespace Mipser.Windows.Views.Pages;

/// <summary>
/// A viewer for files.
/// </summary>
public sealed partial class FileViewer : UserControl
{
    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="ViewModel"/> property.
    /// </summary>
    public DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(FilePageViewModel), typeof(FileViewer), new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="FileViewer"/> class.
    /// </summary>
    public FileViewer()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Gets the view model.
    /// </summary>
    public FilePageViewModel ViewModel
    {
        get => (FilePageViewModel)GetValue(ViewModelProperty);
        set
        {
            UpdateEvents(value, ViewModel);
            SetValue(ViewModelProperty, value);
            UpdateBindings();
        }
    }

    private void UpdateEvents(FilePageViewModel newVM, FilePageViewModel? oldVM)
    {
        if (oldVM is not null)
        {
            oldVM.NavigateToTokenEvent -= ViewModel_NavigateToTokenEvent;
            oldVM.EditorOperationRequested -= ViewModel_EditorOperationRequested;
            oldVM.AssembledEvent -= ViewModel_AssembledEvent;
        }

        newVM.NavigateToTokenEvent += ViewModel_NavigateToTokenEvent;
        newVM.EditorOperationRequested += ViewModel_EditorOperationRequested;
        newVM.AssembledEvent += ViewModel_AssembledEvent;
    }

    private void ViewModel_AssembledEvent(object? sender, IReadOnlyList<AssemblerLog> e)
    {
        // Find editbox
        var editBox = this.FindDescendant<AssemblyEditBox>();
        if (editBox is null)
            return;

        editBox.ApplyLogHighlights(e);
    }

    private void ViewModel_NavigateToTokenEvent(object? sender, SourceLocation e)
    {
        // Find editbox
        var editBox = this.FindDescendant<AssemblyEditBox>();
        if (editBox is null)
            return;

        // Navigate to location
        editBox.NavigateToToken(e);
    }

    private void ViewModel_EditorOperationRequested(object? sender, EditorOperation e)
    {
        // Find editbox
        var editBox = this.FindDescendant<AssemblyEditBox>();
        if (editBox is null)
            return;

        editBox.ApplyOperation(e);
    }

    private void UpdateBindings()
    {
        this.Bindings.Update();
    }
}
