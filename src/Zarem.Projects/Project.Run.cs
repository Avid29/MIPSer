// Avishai Dernis 2025

using System.Threading.Tasks;
using Zarem.Models.Files;

namespace Zarem;

public partial class Project
{
    /// <inheritdoc/>
    public async Task RunAsync(ObjectFile file)
    {
        // TODO: Log error

        var executable = await Format.ImportAsync(file);
        if (executable is null)
            return;
    }
}
