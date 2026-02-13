// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Threading.Tasks;

namespace Zarem.ViewModels;

public partial class WindowViewModel
{
    /// <summary>
    /// Gets a command that builds the project.
    /// </summary>
    public RelayCommand StartWithoutDebuggingCommand { get; }

    private void StartWithoutDebugging()
    {

    }
}
