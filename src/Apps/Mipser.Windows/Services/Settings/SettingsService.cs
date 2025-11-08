// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Messaging;
using Mipser.Services.Settings;
using Windows.Storage;

namespace Mipser.Windows.Services.Settings
{
    /// <summary>
    /// An implementation of the <see cref="ISettingsService"/>
    /// </summary>
    public class SettingsService : ISettingsService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService"/> class.
        /// </summary>
        /// <param name="messenger"></param>
        public SettingsService(IMessenger messenger)
        {
            Local = new SettingsProvider(messenger, ApplicationData.Current.LocalSettings.Values);
        }

        public ISettingsProvider Local { get; }
    }
}
