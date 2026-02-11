// Avishai Dernis 2025

using System.Collections.Generic;
using Zarem.Assembler.MIPS.Models;
using Zarem.Models.Files;

namespace Zarem.Models;

/// <summary>
/// A class containing the results of a build.
/// </summary>
public class BuildResult
{
    private List<(SourceFile, AssemblerResult)> _successfullyAssembledFiles;
    private List<(SourceFile, AssemblerResult)> _failedFiles;
    private List<SourceFile> _skipped;

    internal BuildResult()
    {
        _successfullyAssembledFiles = [];
        _failedFiles = [];
        _skipped = [];
    }

    internal void Add(SourceFile source, AssemblerResult? result)
    {
        if (result is null)
        {
            _skipped.Add(source);
            return;
        }

        var target = result.Failed ? _failedFiles : _successfullyAssembledFiles;
        target.Add((source, result));
    }

    /// <summary>
    /// Gets the files that were successfully assembled.
    /// </summary>
    public IReadOnlyList<(SourceFile, AssemblerResult)> SucessfullyAssembledFiles => _successfullyAssembledFiles;

    /// <summary>
    /// Gets the files that failed to assemble.
    /// </summary>
    public IReadOnlyList<(SourceFile, AssemblerResult)> FailedFiles => _failedFiles;

    /// <summary>
    /// Gets the skipped files.
    /// </summary>
    public IReadOnlyList<SourceFile> SkippedFiles => _skipped;
}
