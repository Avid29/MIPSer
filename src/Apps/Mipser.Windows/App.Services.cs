// Adam Dernis 2024

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Mipser.Services;
using Mipser.Services.Files;
using Mipser.Services.Settings;
using Mipser.Services.Versioning;
using Mipser.ViewModels;
using Mipser.ViewModels.Pages;
using Mipser.ViewModels.Pages.App;
using Mipser.Windows.Services;
using Mipser.Windows.Services.FileSystem;
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
            .AddSingleton<IMessenger, WeakReferenceMessenger>()
            .AddSingleton<ILocalizationService, LocalizationService>()
            .AddSingleton<IFileSystemService, FileSystemService>()
            .AddSingleton<ICacheService, CacheService>()
            .AddSingleton<IVersioningService, VersioningService>()

            // Dependent Services
            .AddSingleton<ISettingsService, SettingsService>()
            .AddSingleton<IFileService, FileService>()
            .AddSingleton<IProjectService, ProjectService>()
            .AddSingleton<BuildService>()

            // Page ViewModels
            .AddTransient<AboutPageViewModel>()
            .AddTransient<CreateProjectViewModel>()
            .AddTransient<CheatSheetViewModel>()
            .AddTransient<FilePageViewModel>()
            .AddTransient<SettingsPageViewModel>()
            .AddTransient<WelcomePageViewModel>()

            // Panel ViewModels
            .AddTransient<ExplorerViewModel>()
            .AddTransient<ErrorListViewModel>()

            // ViewModels
            .AddTransient<StatusViewModel>()
            .AddTransient<WindowViewModel>()
            .AddTransient<PanelViewModel>()
            .AddSingleton<MainViewModel>()
            .BuildServiceProvider();
    }
}
