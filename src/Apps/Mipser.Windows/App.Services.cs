// Adam Dernis 2024

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Mipser.Services.FileSystem;
using Mipser.Services.Localization;
using Mipser.ViewModels;
using Mipser.ViewModels.Pages;
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
            .AddSingleton<IMessenger, WeakReferenceMessenger>()
            .AddSingleton<ILocalizationService, LocalizationService>()
            .AddSingleton<IFileSystemService, FileSystemService>()

            // ViewModels
            .AddSingleton<MainViewModel>()
            .AddTransient<ExplorerViewModel>()
            .AddTransient<CheatSheetViewModel>()
            .AddTransient<PanelViewModel>()
            .AddTransient<WindowViewModel>()
            .BuildServiceProvider();
    }
}
