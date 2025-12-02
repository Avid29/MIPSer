// Avishai Dernis 2025

using Mipser.Services.Localization;
using Mipser.Services.Settings;
using Mipser.Services.Settings.Enums;
using System;
using System.Collections.Generic;

namespace Mipser.ViewModels.Pages.App.Settings;

/// <summary>
/// A view model for the editor settings sub-page.
/// </summary>
public class EditorSettingsViewModel : SettingsSubPageViewModel
{
    private readonly ILocalizationService _localizationService;
    private readonly ISettingsService _settingsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppSettingsViewModel"/> class.
    /// </summary>
    public EditorSettingsViewModel(ILocalizationService localizationService, ISettingsService settingsService)
    {
        _localizationService = localizationService;
        _settingsService = settingsService;
    }

    /// <inheritdoc/>
    public override string Title => _localizationService["/Settings/EditorSettingsTitle"];

    /// <summary>
    /// Gets or sets the real-time assembly setting option.
    /// </summary>
    public bool RealTimeAssembly
    {
        get => _settingsService.Local.GetValue<bool>(nameof(RealTimeAssembly));
        set
        {
            _settingsService.Local.SetValue(nameof(RealTimeAssembly), value, notify: true);
            OnPropertyChanged(nameof(RealTimeAssembly));
        }
    }

    /// <summary>
    /// Gets or sets the real-time assembly setting option.
    /// </summary>
    public AnnotationThreshold AnnotationThreshold
    {
        get => _settingsService.Local.GetValue<AnnotationThreshold>(nameof(AnnotationThreshold));
        set => _settingsService.Local.SetValue(nameof(AnnotationThreshold), value, notify: true);
    }

    /// <summary>
    /// Gets the list of available annotation threshold options.
    /// </summary>
    public IEnumerable<AnnotationThreshold> AnnotationThresholdOptions => Enum.GetValues<AnnotationThreshold>();
}
