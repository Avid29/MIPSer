// Avishai Dernis 2025

using MIPS.Assembler;
using Mipser.Bindables.Files;
using Mipser.ViewModels.Pages.Abstract;
using RASM.Modules.Config;

namespace Mipser.ViewModels.Pages
{
    /// <summary>
    /// A view model for a file page.
    /// </summary>
    public class FilePageViewModel : PageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilePageViewModel"/> class.
        /// </summary>
        public FilePageViewModel()
        {
            File = new BindableFile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePageViewModel"/> class.
        /// </summary>
        public FilePageViewModel(BindableFile file)
        {
            File = file;

            File.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(BindableFile.Name) || e.PropertyName == nameof(BindableFile.IsDirty))
                {
                    OnPropertyChanged(nameof(Title));
                }
            };
        }

        /// <inheritdoc/>
        public override string Title => File.Name + (File.IsDirty ? " *" : string.Empty);

        /// <summary>
        /// Gets the bindable file for this page.
        /// </summary>
        public BindableFile File { get; }

        /// <summary>
        /// Assembles the file.
        /// </summary>
        public async void Assemble()
        {
            var stream = await File.GetStreamAsync();
            if (stream is null)
                return;

            await Assembler.AssembleAsync(stream, File.Name, new RasmConfig());
        }

        /// <summary>
        /// Saves changes to the file.
        /// </summary>
        public async void Save()
        {
            // TODO: Save as dialog for anonymous files.

            await File.SaveAsync();
            OnPropertyChanged(Title);
        }
    }
}
