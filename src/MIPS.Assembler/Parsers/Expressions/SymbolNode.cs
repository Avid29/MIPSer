// Avishai Dernis 2025

using MIPS.Assembler.Parsers.Expressions.Abstract;
using MIPS.Assembler.Parsers.Expressions.Enums;
using MIPS.Assembler.Parsers.Expressions.Evaluator;
using MIPS.Assembler.Tokenization.Models;
using MIPS.Models.Addressing;
using MIPS.Models.Modules.Tables;

namespace MIPS.Assembler.Parsers.Expressions;

/// <summary>
/// A node for a symbol reference in an expression tree.
/// </summary>
public class SymbolNode : ValueNode<SymbolEntry>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolNode"/> class.
    /// </summary>
    public SymbolNode(Token token, SymbolEntry symbolEntry) : base(token, symbolEntry)
    {
    }

    /// <inheritdoc/>
    public override ExpressionType Type => ExpressionType.Integer;

    /// <inheritdoc/>
    public override bool TryEvaluate(IEvaluator<Address> evaluator, out Address result)
    {
        // TODO: Handle undefined symbols

        result = Value.Address;
        return true;
    }
}
