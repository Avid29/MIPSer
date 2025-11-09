// Avishai Dernis 2025

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mipser.ViewModels.Pages.Abstract;

namespace Mipser.Windows.Views.Pages.Abstract;

/// <summary>
/// A base control with a <see cref="PageViewModel"/>.
/// </summary>
public abstract partial class PageControl : UserControl
{
    /// <summary>
    /// A <see cref="DependencyProperty"/> for the <see cref="ViewModel"/> property.
    /// </summary>
    public DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(PageViewModel), typeof(PageControl), new PropertyMetadata(null));

    /// <summary>
    /// Gets the view model.
    /// </summary>
    public PageViewModel ViewModel
    {
        get => (PageViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
}
