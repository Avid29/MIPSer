// Avishai Dernis 2025

using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.DependencyInjection;
using MIPS.Assembler;
using Mipser.Bindables.Files;
using Mipser.ViewModels.Pages.Abstract;
using RASM.Modules.Config;
using System.ComponentModel;

namespace Mipser.ViewModels.Pages
{
    /// <summary>
    /// A view model for a file page.
    /// </summary>
    public class FilePageViewModel : PageViewModel
    {
        private BindableFile? _file;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePageViewModel"/> class.
        /// </summary>
        public FilePageViewModel()
        {
        }

        /// <inheritdoc/>
        public override string Title
        {
            get
            {
                Guard.IsNotNull(File);

                // Get name and append astrics if dirty
                var name = File.Name;
                if (File.IsDirty)
                    name += " *";

                return name;
            }
        }

        /// <summary>
        /// Gets the bindable file for this page.
        /// </summary>
        public BindableFile? File
        {
            get => _file;
            set => SetFile(value);
        }

        /// <summary>
        /// Saves changes to the file.
        /// </summary>
        public async void Save()
        {
            // TODO: Save as dialog for anonymous files.
            if (File is null)
                return;

            await File.SaveAsync();
            OnPropertyChanged(Title);
        }

        private async void SetFile(BindableFile? file)
        {
            if (_file is not null)
            {
                _file.PropertyChanged -= OnFileUpdate;
            }

            _file = file;

            if (_file is not null)
            {
                if (_file.Contents is null)
                    await _file.LoadContent();

                _file.PropertyChanged += OnFileUpdate;
            }
        }

        private void OnFileUpdate(object? sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(BindableFile.Name) || args.PropertyName == nameof(BindableFile.IsDirty))
            {
                OnPropertyChanged(nameof(Title));
            }
        }
    }
}
