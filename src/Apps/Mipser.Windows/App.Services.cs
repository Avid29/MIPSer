// Adam Dernis 2024

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Mipser.Services;
using Mipser.Services.Files;
using Mipser.Services.Popup;
using Mipser.Services.Settings;
using Mipser.Services.Versioning;
using Mipser.ViewModels;
using Mipser.ViewModels.Pages;
using Mipser.ViewModels.Pages.App;
using Mipser.Windows.Services;
using Mipser.Windows.Services.FileSystem;
using Mipser.Windows.Services.Popup;
using Mipser.Windows.Services.Settings;
using Mipser.Windows.Services.Versioning;
using System;

namespace Mipser.Windows;

public partial class App
{
    private static IServiceProvider ConfigureServices()
    {
        // Register services
        return new ServiceCollection()

            // Basic Services
            .AddSingleton<ICacheService, CacheService>()
            .AddSingleton<IClipboardService, ClipboardService>()
            .AddSingleton<IDispatcherService, DispatcherService>()
            .AddSingleton<IFileSystemService, FileSystemService>()
            .AddSingleton<ILocalizationService, LocalizationService>()
            .AddSingleton<IMessenger, WeakReferenceMessenger>()
            .AddSingleton<IPopupService, PopupService>()
            .AddSingleton<IVersioningService, VersioningService>()

            // Dependent Services
            .AddSingleton<ISettingsService, SettingsService>()
            .AddSingleton<IProjectService, ProjectService>()
            .AddSingleton<IFileService, FileService>()
            .AddSingleton<BuildService>()

            // Page ViewModels
            .AddTransient<AboutPageViewModel>()
            .AddTransient<CreateProjectViewModel>()
            .AddTransient<CheatSheetViewModel>()
            .AddTransient<FilePageViewModel>()
            .AddTransient<SettingsPageViewModel>()
            .AddTransient<WelcomePageViewModel>()

            // Panel ViewModels
            .AddSingleton<ExplorerViewModel>()
            .AddSingleton<ErrorListViewModel>()

            // ViewModels
            .AddTransient<StatusViewModel>()
            .AddTransient<WindowViewModel>()
            .AddTransient<PanelViewModel>()
            .AddSingleton<MainViewModel>()
            .BuildServiceProvider();
    }
}
