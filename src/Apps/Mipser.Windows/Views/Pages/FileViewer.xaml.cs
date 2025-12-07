// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Tokenization.Models;
using Mipser.Messages;
using Mipser.Messages.Editor.Enums;
using Mipser.Models.EditorConfig.ColorScheme;
using Mipser.Services;
using Mipser.Services.Settings.Enums;
using Mipser.ViewModels.Pages;
using Mipser.Windows.Controls.CodeEditor;
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

        Service.Get<IMessenger>().Register<FileViewer, SettingChangedMessage<Theme>>(this, (r, m) => SyntaxHighlighting.ReloadFromSettings());
        Service.Get<IMessenger>().Register<FileViewer, SettingChangedMessage<EditorColorScheme>>(this, (r, m) => SyntaxHighlighting.ReloadFromSettings());
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
        }

        newVM.NavigateToTokenEvent += ViewModel_NavigateToTokenEvent;
        newVM.EditorOperationRequested += ViewModel_EditorOperationRequested;
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
