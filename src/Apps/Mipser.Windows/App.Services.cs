// Adam Dernis 2023

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Mipser.Services.Localization;
using Mipser.Windows.Services.Localization;
using System;

namespace Mipser.Windows;

public partial class App
{
    private static IServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton<IMessenger, WeakReferenceMessenger>()
            .AddSingleton<ILocalizationService, LocalizationService>()
            .BuildServiceProvider();
    }
}
