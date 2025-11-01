// Avishai Dernis 2025

using CommunityToolkit.Mvvm.ComponentModel;

namespace Mipser.ViewModels.Views.Abstract
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
    }
}
