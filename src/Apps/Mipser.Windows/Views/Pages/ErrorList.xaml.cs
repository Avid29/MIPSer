// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MIPS.Assembler.Logging;
using MIPS.Assembler.Tokenization.Models;
using Mipser.Messages;
using Mipser.Messages.Build;
using Mipser.Messages.Navigation;
using Mipser.Models.EditorConfig.ColorScheme;
using Mipser.Services;
using Mipser.Services.Settings;
using Mipser.ViewModels.Pages;
using Mipser.Windows.Services.Settings;
using Mipser.Windows.Views.Pages.Editor;
using System.Globalization;
using Windows.Security.Cryptography.Core;

namespace Mipser.Windows.Views.Pages;

/// <summary>
/// A viewer for files.
/// </summary>
public sealed partial class ErrorList : UserControl
{
    private static readonly DependencyProperty MessageFlowDirectionProperty =
        DependencyProperty.Register(nameof(MessageFlowDirection), typeof(FlowDirection), typeof(ErrorList), new(FlowDirection.LeftToRight));

    private static readonly DependencyProperty MessageTextAlignmentProperty =
        DependencyProperty.Register(nameof(MessageTextAlignment), typeof(TextAlignment), typeof(ErrorList), new(TextAlignment.Left));

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorList"/> class.
    /// </summary>
    public ErrorList()
    {
        this.InitializeComponent();

        ViewModel = Service.Get<ErrorListViewModel>();
        DataContext = this;

        UpdateAlignment();

        // Update alignment when the build finishes in case the assembler language changed.
        Service.Get<IMessenger>().Register<ErrorList, BuildFinishedMessage>(this, (r, m) => r.UpdateAlignment());
    }

    private ErrorListViewModel ViewModel { get; }

    private static string DisplayLine(SourceLocation? location)
    {
        // The location is null. Display nothing
        if (location is null)
        {
            return string.Empty;
        }

        // Get the line
        return $"{location.Value.Line}";
    }

    private void ListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not AssemblerLogEntry log)
            return;

        var messenger = Service.Get<IMessenger>();
        messenger.Send(new NavigateToTokenRequestMessage(log.Tokens[0]));
    }

    public FlowDirection MessageFlowDirection
    {
        get => (FlowDirection)GetValue(MessageFlowDirectionProperty);
        set => SetValue(MessageFlowDirectionProperty, value);
    }

    public TextAlignment MessageTextAlignment
    {
        get => (TextAlignment)GetValue(MessageTextAlignmentProperty);
        set => SetValue(MessageTextAlignmentProperty, value);
    }

    private void UpdateAlignment()
    {
        var culture = CultureInfo.CurrentUICulture;
        var lang = Service.Get<ISettingsService>().Local.GetValue<string>(SettingsKeys.AssemblerLanguageOverride);
        if (lang is not null)
        {
            culture = CultureInfo.GetCultureInfo(lang);
        }

        var isRtl = culture.TextInfo.IsRightToLeft;
        MessageFlowDirection = isRtl ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        MessageTextAlignment = isRtl ? TextAlignment.Right : TextAlignment.Left;
    }
}
