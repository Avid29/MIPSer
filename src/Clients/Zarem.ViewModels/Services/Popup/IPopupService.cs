// Avishai Dernis 2025

using Zarem.Services.Popup.Enums;
using Zarem.Services.Popup.Models;
using System.Threading.Tasks;

namespace Zarem.Services.Popup;

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
