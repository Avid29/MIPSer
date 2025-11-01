// Avishai Dernis 2025

using Mipser.Bindables.Files;
using Mipser.ViewModels.Views.Abstract;

namespace Mipser.ViewModels.Views
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
        }

        /// <inheritdoc/>
        public override string Title => File.Name;

        /// <summary>
        /// Gets the bindable file for this page.
        /// </summary>
        public BindableFile File { get; set; }
    }
}
