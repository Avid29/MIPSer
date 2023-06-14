// Adam Dernis 2023

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Mipser.Services.Files;
using Mipser.Services.Localization;
using Mipser.ViewModels;
using Mipser.ViewModels.Views;
using Mipser.Windows.Services.Files;
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
            .AddSingleton<IFilesService, FilesService>()

            // ViewModels
            .AddSingleton<WindowViewModel>()
            .AddSingleton<OpenFilesViewModel>()
            .BuildServiceProvider();
    }
}
