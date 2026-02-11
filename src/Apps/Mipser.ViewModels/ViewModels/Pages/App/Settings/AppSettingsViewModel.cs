// Avishai Dernis 2025

using Mipser.Models;
using Mipser.Services;
using Mipser.Services.Settings;
using Mipser.Services.Settings.Enums;
using Mipser.Services.Versioning;
using Mipser.Services.Versioning.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mipser.ViewModels.Pages.App.Settings
{
    /// <summary>
    /// A view model for the app settings sub-page.
    /// </summary>
    public class AppSettingsViewModel : SettingsSubPageViewModel
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISettingsService _settingsService;
        private readonly IVersioningService _versioningService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsViewModel"/> class.
        /// </summary>
        public AppSettingsViewModel(ILocalizationService localizationService, ISettingsService settingsService, IVersioningService versioningService)
        {
            _localizationService = localizationService;
            _settingsService = settingsService;
            _versioningService = versioningService;
        }

        /// <inheritdoc/>
        public override string Title => _localizationService["/Settings/AppSettingsTitle"];

        /// <summary>
        /// Gets or sets the selected app theme.
        /// </summary>
        public Theme AppTheme
        {
            get => _settingsService.Local.GetValue<Theme>(SettingsKeys.AppTheme);
            set => _settingsService.Local.SetValue(SettingsKeys.AppTheme, value, notify: true);
        }

        /// <summary>
        /// Gets the list of available app theme options.
        /// </summary>
        public IEnumerable<Theme> AppThemeOptions => Enum.GetValues<Theme>();

        /// <summary>
        /// Gets or sets the app language in settings.
        /// </summary>
        public string LanguageOverride
        {
            get => _settingsService.Local.GetValue<string>(SettingsKeys.LanguageOverride) ?? "system";
            set => _settingsService.Local.SetValue(SettingsKeys.LanguageOverride, value is "system" ? null : value);
        }

        /// <summary>
        /// Gets the list of available languages in the app.
        /// </summary>
        /// <remarks>
        /// "system" is a sentinel value since null and empty cannot be used in a ComboBox.
        /// </remarks>
        public IEnumerable<string> AppLanguageOptions => _localizationService.AvailableLanguages.Prepend("system");

        /// <summary>
        /// Gets or sets whether or not the app should restore open projects when opened.
        /// </summary>
        public bool RestoreOpenProject
        {
            get => _settingsService.Local.GetValue<bool>(SettingsKeys.RestoreOpenProject);
            set => _settingsService.Local.SetValue(SettingsKeys.RestoreOpenProject, value);
        }

        /// <summary>
        /// Gets the app's version.
        /// </summary>
        public string AppVersion =>
            _localizationService["/Settings/VersionFormat",
                _versioningService.AppVersion.MajorVersion,
                _versioningService.AppVersion.MinorVersion,
                _versioningService.AppVersion.Revision];

        /// <summary>
        /// Gets the build's git version info.
        /// </summary>
        public GitVersionInfo GitInfo => _versioningService.GitVersionInfo;

        /// <summary>
        /// Gets a link to the build's commit on github.
        /// </summary>
        public string CommitGitHubLink => $"https://github.com/Avid29/MIPSer/tree/{GitInfo.Sha}";

        /// <summary>
        /// Gets a list of third-party dependencies used in MIPSer.
        /// </summary>
        public IEnumerable<ThirdPartyNotice> ThirdPartyNotices { get; } =
        [
            new()
            {
                DependencyName = "GitInfo",
                Url = "https://github.com/devlooped/GitInfo",
            },
            new()
            {
                DependencyName = "LibObjectFile",
                Url = "https://github.com/xoofx/LibObjectFile",
            },
            new()
            {
                DependencyName = "Windows Community Toolkit",
                Url = "https://github.com/CommunityToolkit/Windows",
            },
            new()
            {
                DependencyName = "WinUIEdit",
                Url = "https://github.com/BreeceW/WinUIEdit",
            },
        ];
    }
}
