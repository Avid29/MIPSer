// Avishai Dernis 2025

using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using MIPS.Assembler.Models.Instructions;
using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.Registers;
using Mipser.Models.CheatSheet;
using Mipser.Services.Localization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        var table = new InstructionTable(MipsVersion.MipsIII);
        var instructions = table.GetInstructions(false);

        CommonInstructions = new(LoadInstructionSet("CommonInstructions.json", instructions) ?? []);
        FloatInstructions = new(LoadInstructionSet("FloatInstructions.json", instructions) ?? []);
        CoProc0Instructions = new(LoadInstructionSet("CoProc0Instructions.json", instructions) ?? []);
        Specialized0Instructions = new(LoadInstructionSet("SpecializedInstructions.json", instructions) ?? []);

        PrimaryEncodingPatterns = new(LoadEncodingPatterns("PrimaryEncodings.json") ?? []);
        CoProcessor1Patterns = new(LoadEncodingPatterns("CoProcessor1Encodings.json") ?? []);
        CoProcessor0Patterns = new(LoadEncodingPatterns("CoProcessor0Encodings.json") ?? []);
        UniquePatterns = new(LoadEncodingPatterns("UniqueEncodings.json") ?? []);

        GPRegisters = [..Enumerable.Range(0, 32).Select(x => (Register)x)];

        IsActive = true;
    }

    private static IEnumerable<EncodingPattern>? LoadEncodingPatterns(string filename)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resources = assembly.GetManifestResourceNames();
        var resource = resources.First(x => x.EndsWith(filename));
        
        using Stream? stream = assembly.GetManifestResourceStream(resource);
        if (stream is null)
            return null;

        var patterns = JsonSerializer.Deserialize<EncodingPattern[]>(stream);
        if (patterns is null)
            return null;

        return patterns;
    }

    private static IEnumerable<IGrouping<string, InstructionMetadata>>? LoadInstructionSet(string filename, InstructionMetadata[] instructions)
    {
        // Load groupings
        var assembly = Assembly.GetExecutingAssembly();
        var resources = assembly.GetManifestResourceNames();
        var resource = resources.First(x => x.EndsWith(filename));
        
        using Stream? stream = assembly.GetManifestResourceStream(resource);
        if (stream is null)
            return null;

        var collection = JsonSerializer.Deserialize<InstructionCollection>(stream);
        if (collection is null)
            return null;

        // Grab localization service
        var localize = Ioc.Default.GetRequiredService<ILocalizationService>();

        // Create the grouped collection
        var simplePairs = collection.Groups.SelectMany(
            group => group.Instructions.Select(instruction => (group.GroupName, instruction)));
        var pairs = simplePairs.Join(instructions,
            pair => pair.instruction,
            instruction => instruction.Identifier,
            (pair, instruction) => (pair.GroupName, instruction));
        var groups = pairs.GroupBy(x => localize[x.GroupName], x => x.instruction);

        return groups;
    }

    /// <summary>
    /// Gets an <see cref="ObservableCollection{EncodingPattern}"/> of the primary encoding patterns.
    /// </summary>
    public ObservableCollection<EncodingPattern> PrimaryEncodingPatterns { get; }

    /// <summary>
    /// Gets an <see cref="ObservableCollection{EncodingPattern}"/> of the coprocessor1 encoding patterns.
    /// </summary>
    public ObservableCollection<EncodingPattern> CoProcessor1Patterns { get; }

    /// <summary>
    /// Gets an <see cref="ObservableCollection{EncodingPattern}"/> of the coprocessor0 encoding patterns.
    /// </summary>
    public ObservableCollection<EncodingPattern> CoProcessor0Patterns { get; }

    /// <summary>
    /// Gets an <see cref="ObservableCollection{EncodingPattern}"/> of unique encoding patterns.
    /// </summary>
    public ObservableCollection<EncodingPattern> UniquePatterns { get; }

    /// <summary>
    /// Gets an <see cref="ObservableGroupedCollection{String, InstructionMetadata}"/> of common instruction metadatas, grouped by category.
    /// </summary>
    public ObservableGroupedCollection<string, InstructionMetadata>? CommonInstructions { get; }
    
    /// <summary>
    /// Gets an <see cref="ObservableGroupedCollection{String, InstructionMetadata}"/> of floating-point instruction metadatas, grouped by category.
    /// </summary>
    public ObservableGroupedCollection<string, InstructionMetadata>? FloatInstructions { get; }
    
    /// <summary>
    /// Gets an <see cref="ObservableGroupedCollection{String, InstructionMetadata}"/> of coproc0 instruction metadatas, grouped by category.
    /// </summary>
    public ObservableGroupedCollection<string, InstructionMetadata>? CoProc0Instructions { get; }
    
    /// <summary>
    /// Gets an <see cref="ObservableGroupedCollection{String, InstructionMetadata}"/> of specialized instruction metadatas, grouped by category.
    /// </summary>
    public ObservableGroupedCollection<string, InstructionMetadata>? Specialized0Instructions { get; }

    /// <summary>
    /// Gets the list of general purpose registers.
    /// </summary>
    public Register[] GPRegisters { get; }
}
