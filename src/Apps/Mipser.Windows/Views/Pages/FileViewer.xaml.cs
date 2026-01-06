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
        }
    }

    private void UpdateBindings()
    {
        this.Bindings.Update();
    }
}
