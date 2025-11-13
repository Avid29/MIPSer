// Avishai Dernis 2025

using CommunityToolkit.Mvvm.ComponentModel;

namespace Mipser.ViewModels.Pages.Abstract
{
    /// <summary>
    /// A base class for page view models.
    /// </summary>
    public abstract class PageViewModel : ObservableRecipient
    {
        /// <summary>
        /// Gets or sets the title of the page.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Gets or sets if the page can be edited as text.
        /// </summary>
        public virtual bool CanTextEdit => false;

        /// <summary>
        /// Gets or sets if the page can be saved.
        /// </summary>
        public virtual bool CanSave => false;

        /// <summary>
        /// Gets or sets if the page can be assembled.
        /// </summary>
        public virtual bool CanAssemble => false;
    }
}
