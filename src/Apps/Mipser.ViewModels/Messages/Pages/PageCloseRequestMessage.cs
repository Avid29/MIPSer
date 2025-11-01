// Adam Dernis 2024

using Mipser.ViewModels.Pages.Abstract;

namespace Mipser.Messages.Pages;

/// <summary>
/// A message sent to close a page.
/// </summary>
public sealed class PageCloseRequestMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PageCloseRequestMessage"/> class.
    /// </summary>
    public PageCloseRequestMessage(PageViewModel? page = null)
    {
        Page = page;
    }

    /// <summary>
    /// Gets the page to close.
    /// </summary>
    public PageViewModel? Page { get; }

    /// <summary>
    /// Closes the current page.
    /// </summary>
    public bool Current => Page is null;
}
