// Adam Dernis 2024

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Mipser.Services.Build;
using Mipser.Services.Files;
using Mipser.Services.Localization;
using Mipser.Services.Project;
using Mipser.ViewModels;
using Mipser.ViewModels.Pages;
using Mipser.ViewModels.Pages.App;
using Mipser.Windows.Services.Cache;
using Mipser.Windows.Services.FileSystem;
using Mipser.Windows.Services.Localization;
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

            // Dependent Services
            .AddSingleton<IFileService, FileService>()
            .AddSingleton<IProjectService, ProjectService>()
            .AddSingleton<BuildService>()

            // Page ViewModels
            .AddTransient<AboutPageViewModel>()
            .AddTransient<CheatSheetViewModel>()
            .AddTransient<FilePageViewModel>()

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
