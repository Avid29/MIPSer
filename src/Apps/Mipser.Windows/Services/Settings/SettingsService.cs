// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Messaging;
using Mipser.Services.Settings;
using Mipser.Services.Settings.Enums;
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

            EstablishDefaults();
        }

        public ISettingsProvider Local { get; }

        private void EstablishDefaults()
        {
            // Theme
            Local.SetValue("AppTheme", Theme.Default, false);
            
            // Editor
            Local.SetValue("RealTimeAssembly", true, false);
            Local.SetValue("AnnotationThreshold", AnnotationThreshold.Errors, false);

            // Localization
            Local.SetValue<string?>("LanguageOverride", null, false);
            Local.SetValue<string?>("AssemblerLanguageOverride", null, false);
        }
    }
}
