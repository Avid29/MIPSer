// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mipser.Services.Popup;
using Mipser.Services.Popup.Enums;
using Mipser.Services.Popup.Models;
using System;
using System.Threading.Tasks;

namespace Mipser.Windows.Services.Popup;

public class PopupService : IPopupService
{
    /// <inheritdoc/>
    public async Task<PopupResult> ShowPopAsync(PopupDetails popup)
    {
        // TODO: Multi-Windowing
        var xamlRoot = App.Current.Window?.Content.XamlRoot;
        if (xamlRoot is null)
        {
            return PopupResult.Closed;
        }


        var dialog = new ContentDialog()
        {
            Title = popup.Title,
            Content = popup.Description,
            PrimaryButtonText = popup.PrimaryButtonText,
            PrimaryButtonStyle = (Style)App.Current.Resources["AccentButtonStyle"],
            IsPrimaryButtonEnabled = popup.PrimaryButtonText is not null,
            SecondaryButtonText = popup.SecondaryButtonText,
            IsSecondaryButtonEnabled = popup.SecondaryButtonText is not null,
            CloseButtonText = popup.CloseButtonText,
            XamlRoot = xamlRoot,
        };

        var result = await dialog.ShowAsync();
        return result switch
        {
            ContentDialogResult.None => PopupResult.Closed,
            ContentDialogResult.Primary => PopupResult.Primary,
            ContentDialogResult.Secondary => PopupResult.Secondary,
            _ => PopupResult.Closed,
        };
    }
}
