﻿// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using System.IO;

namespace MIPS.Tests.Helpers;

public static class TestFilePathing
{
    private const string AssemblyFilesPathBase = @"MIPS.Tests/ASMs/";
    private const string ObjectFolder = "obj/";

    public const string BranchLiteralFile = "component_tests/branch_literal.asm";
    public const string BranchRelativeFile = "component_tests/branch_relative.asm";
    public const string EmptyTestFile = "component_tests/empty.asm";
    public const string InstructionsTestFile = "component_tests/instructions.asm";
    public const string PseudoInstructionsTestFile = "component_tests/pseudo_instructions.asm";

    public const string CompositeFailTestFile = "error_tests/composite_fail.asm";
    public const string DuplicateSymbolFile = "error_tests/duplicate_symbol.asm";
    public const string SubtractAddressFile = "error_tests/subtract_address.asm";

    public const string PlaygroundTestFile1 = "playground1.asm";

    /// <summary>
    /// Gets the path of an assembly test file.
    /// </summary>
    /// <param name="testFile">The relative path of the assembly test file from the assembly file base path.</param>
    /// <returns>The path of an assembly test file/</returns>
    public static string GetAssemblyFilePath(string testFile, bool fullPath = true)
    {
        var testPath = FindTestsPath();
        if (testPath is null)
        {
            ThrowHelper.ThrowExternalException("Could not find test directory as parent");
        }

        var path = Path.Combine(testPath, AssemblyFilesPathBase, testFile);
        if (fullPath)
        {
            path = Path.GetFullPath(path);
        }

        return path;
    }

    /// <summary>
    /// Gets the path of an object file to match the <paramref name="assemblyFile"/>.
    /// </summary>
    /// <param name="assemblyFile">The assembly file to match.</param>
    /// <param name="objFolder">Whether or not to use an obj sub-folder.</param>
    /// <returns>The path of the object file.</returns>
    public static string GetMatchingObjectFilePath(string assemblyFile, bool objFolder = true)
    {
        var original = GetAssemblyFilePath(assemblyFile, true);
        var dir = Path.GetDirectoryName(original) ?? AssemblyFilesPathBase;

        if (objFolder)
        {
            dir = Path.Combine(dir, ObjectFolder);
            Directory.CreateDirectory(dir);
        }

        return Path.Combine(dir, Path.GetFileNameWithoutExtension(original) + ".obj");
    }

    private static string? FindTestsPath()
    {
        string path = Directory.GetCurrentDirectory();
        var dir = new DirectoryInfo(path);
        while (dir is not null && dir.Name != "tests")
        {
            dir = dir.Parent;
        }

        return dir?.FullName;
    }
}
