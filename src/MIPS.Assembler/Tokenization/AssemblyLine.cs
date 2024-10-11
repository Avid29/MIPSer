// Adam Dernis 2024

using MIPS.Assembler.Tokenization.Enums;
using System;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;

namespace MIPS.Assembler.Tokenization;

/// <summary>
/// A line of tokenized line assembly of assembly.
/// </summary>
public ref struct AssemblyLine
{
    private Span<Token> _tokens;
    private Span<Token> _argPosition;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyLine"/> struct.
    /// </summary>
    /// <param name="tokens"></param>
    public AssemblyLine(Span<Token> tokens)
    {
        _tokens = tokens;
        SubTokenize();
    }

    /// <summary>
    /// Gets a token from the assembly line.
    /// </summary>
    /// <param name="index">The index of the token to retrieve.</param>
    /// <returns>The token at <paramref name="index"/> in the line.</returns>
    public readonly Token this[int index] => _tokens[index];

    /// <summary>
    /// Gets the number of tokens in the line.
    /// </summary>
    public readonly int Count => _tokens.Length;

    /// <summary>
    /// Gets what type of declaration occurs on the line, 
    /// </summary>
    public LineType Type {get; private set; }

    /// <summary>
    /// Gets the label declared on the line, if any.
    /// </summary>
    public Token? Label { get; private set; }

    /// <summary>
    /// Gets the instruction token on the line, if any.
    /// </summary>
    public Token? Instruction { get; private set; }

    /// <summary>
    /// Gets the directive token on the line, if any.
    /// </summary>
    public Token? Directive { get; private set; }

    /// <summary>
    /// Gets the args for either the instruction or directive.
    /// </summary>
    public Span<Token> Args { get; private set; }

    /// <summary>
    /// Gets the macro declared on the line, if any.
    /// </summary>
    public Token? Macro { get; private set; }

    /// <summary>
    /// Gets whether or not all args have been read.
    /// </summary>
    public bool ArgsEnd => _argPosition.IsEmpty;

    /// <summary>
    /// Gets the expression assigned to the macro.
    /// </summary>
    /// <remarks>
    /// This includes the assignment token.
    /// </remarks>
    public readonly Span<Token> MacroExpression => Args;

    /// <summary>
    /// Gets the next arg from the arg span.
    /// </summary>
    /// <returns>A span of tokens making an arg.</returns>
    public Span<Token> GetNextArg()
    {
        _argPosition = _argPosition.SplitAtNext(TokenType.Comma, out var before, out _);
        return before;
    }

    /// <summary>
    /// Resets the args iterator.
    /// </summary>
    public void ResetArgs()
    {
        _argPosition = Args;
    }

    private void SubTokenize()
    {
        Type = LineType.None;

        // Grab the label
        Span<Token> line = _tokens.TrimType(TokenType.LabelDeclaration, out var label);
        Label = label;

        if (line.Length == 0)
            return;

        var next = line[0];
        switch(next.Type)
        {   
            case TokenType.MacroDefinition:
                Macro = next;
                Type = LineType.Macro;
                break;
            case TokenType.Instruction:
                Instruction = next;
                Type = LineType.Instruction;
                break;
            case TokenType.Directive:
                Directive = next;
                Type = LineType.Directive;
                break;
        }
        
        // NOTE: The assignment token is left as part of the arg.
        // The assembler will need to verify it is present, and log if it is not.
        // However, unless proceeded by an assignment token this never should have
        // been tokenized as a macro.
        Args = line[1..];
        _argPosition = Args;
    }
}
