// Adam Dernis 2024

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Mipser.Services.Files;
using Mipser.Services.Localization;
using Mipser.Services.ProjectService;
using Mipser.ViewModels;
using Mipser.ViewModels.Pages;
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
            .AddSingleton<BuildService>()

            // ViewModels
            .AddSingleton<MainViewModel>()
            .AddTransient<StatusViewModel>()
            .AddTransient<WindowViewModel>()
            .AddTransient<PanelViewModel>()
            .AddTransient<FilePageViewModel>()
            .AddTransient<ExplorerViewModel>()
            .AddTransient<CheatSheetViewModel>()
            .AddTransient<ErrorListViewModel>()
            .BuildServiceProvider();
    }
}
