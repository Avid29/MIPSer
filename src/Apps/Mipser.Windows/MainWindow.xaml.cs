// Adam Dernis 2023

using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using System;
using System.Threading.Tasks;

namespace Mipser.Windows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        this.InitializeComponent();
        ExtendTitleBar();
        
    }

    private void ExtendTitleBar()
    {
        //this.ExtendsContentIntoTitleBar = true;
        //this.SetTitleBar(AppTitleBar);

    }
}
