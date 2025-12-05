// Adam Dernis 2024

namespace Mipser.Windows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : MipserWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        SetupWindowStyle();

        // Open the welcome page on startup
        ViewModel.OpenWelcome();
    }
}
