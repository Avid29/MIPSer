// Avishai Dernis 2025

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Tokenization.Models;
using Mipser.Editors.AssemblyEditBox;
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
            SetValue(ViewModelProperty, value);
            UpdateBindings();
            UpdateEvents();
        }
    }

    private void UpdateEvents()
    {
        ViewModel.NavigateToTokenEvent += ViewModel_NavigateToTokenEvent;
        ViewModel.AssembledEvent += ViewModel_AssembledEvent;
    }

    private void ViewModel_AssembledEvent(object? sender, IReadOnlyList<ILog> e)
    {
        // Find editbox
        var editBox = this.FindDescendant<AssemblyEditBox>();
        if (editBox is null)
            return;

        //editBox.ApplyLogHighlights(e);
    }

    private void ViewModel_NavigateToTokenEvent(object? sender, SourceLocation e)
    {
        // Find editbox
        var editBox = this.FindDescendant<AssemblyEditBox>();
        if (editBox is null)
            return;

        // Navigate to location
        editBox.NavigateToLocation(e);
    }

    private void UpdateBindings()
    {
        this.Bindings.Update();
    }
}
