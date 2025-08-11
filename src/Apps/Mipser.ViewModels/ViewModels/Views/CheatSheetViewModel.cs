// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using MIPS.Assembler.Models.Instructions;
using Mipser.Models;
using Mipser.Services.Localization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Mipser.ViewModels.Views;

/// <summary>
/// A view model for the cheatsheet.
/// </summary>
public class CheatSheetViewModel : ObservableRecipient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CheatSheetViewModel"/> class.
    /// </summary>
    public CheatSheetViewModel()
    {
        // TODO: Load the instruction metadata from a service.
        var table = new InstructionTable(MIPS.Models.Instructions.Enums.MipsVersion.MipsIII);
        var instructions = table.GetInstructions();

        // Load groupings
        var assembly = Assembly.GetExecutingAssembly();
        var resources = assembly.GetManifestResourceNames();
        var resource = resources.First(x => x.EndsWith("CommonInstructions.json"));
        
        using Stream? stream = assembly.GetManifestResourceStream(resource);
        if (stream is null)
            return;

        var collection = JsonSerializer.Deserialize<InstructionCollection>(stream);
        if (collection is null)
            return;

        // Grab localization service
        var localize = Ioc.Default.GetService<ILocalizationService>();

        // Create the grouped collection
        var simplePairs = collection.Groups.SelectMany(
            group => group.Instructions.Select(instruction => (group.GroupName, instruction)));
        var pairs = simplePairs.Join(instructions,
            pair => pair.instruction,
            instruction => instruction.Name,
            (pair, instruction) => (pair.GroupName, instruction));
        var groups = pairs.GroupBy(x => localize?[x.GroupName] ?? x.GroupName, x => x.instruction);

        Instructions = new(groups);

        IsActive = true;
    }
    /// <summary>
    /// Gets an <see cref="ObservableGroupedCollection{String, InstructionMetadata}"/> of grouped instruction metadatas.
    /// </summary>
    public ObservableGroupedCollection<string, InstructionMetadata>? Instructions { get; }
}
