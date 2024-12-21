// Adam Dernis 2024

namespace MIPS.Assembler.Models.Modules.Interfaces;

/// <summary>
/// An interface for a module implementation.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Abstracts the module into a <see cref="ModuleConstructor"/> for modification or linking.
    /// </summary>
    /// <param name="config">The configuration settings.</param>
    /// <returns>The module as a <see cref="ModuleConstructor"/>.</returns>
    public ModuleConstructor? Abstract(AssemblerConfig config);
}
