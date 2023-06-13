// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPS.Assembler.Models.Markers;
using MIPS.Assembler.Parsers;

namespace MIPS.Assembler.Tests;

[TestClass]
public class MarkerParsesTests
{
    private const string Byte = ".byte 10"; 
    private const string Word = ".word 10"; 
    private const string Bytes = ".byte 10, 10";

    [TestMethod(Byte)]
    public void ByteTest() => RunDataTest(Byte, 10);

    [TestMethod(Word)]
    public void WordTest() => RunDataTest(Word, 0, 0, 0, 10);

    [TestMethod(Bytes)]
    public void BytesTest() => RunDataTest(Bytes, 10, 10);

    public static void RunDataTest(string input, params byte[] expected)
    {
        var parser = new MarkerParser();

        TokenizeMarker(input, out var name, out var args);
        parser.TryParseMarker(name, args, out var marker);

        if (marker is not DataMarker dataMarker)
            Assert.Fail();

        var actual = (marker as DataMarker)?.Data;
        Guard.IsNotNull(actual);

        Assert.AreEqual(expected.Length, actual.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.AreEqual(expected[i], actual[i]);
        }
    }

    private static void TokenizeMarker(string line, out string name, out string[] args)
    {
        var nameEnd = line.IndexOf(' ');
        name = line[1..nameEnd];
        args = line[(nameEnd + 1)..].Split(',');
    }
}
