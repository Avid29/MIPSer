// Avishai Dernis 2025

using Mipser.Services.Popup.Enums;
using Mipser.Services.Popup.Models;
using System.Threading.Tasks;

namespace Mipser.Services.Popup;

/// <summary>
/// An interface for a service to create popups.
/// </summary>
public interface IPopupService
{
    /// <summary>
    /// Opens a popup and awaits a selection.
    /// </summary>
    public Task<PopupResult> ShowPopAsync(PopupDetails popup);
}
