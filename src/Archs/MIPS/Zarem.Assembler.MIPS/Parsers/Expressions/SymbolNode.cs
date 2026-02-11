// Avishai Dernis 2025

using Zarem.Assembler.MIPS.Parsers.Expressions.Abstract;
using Zarem.Assembler.MIPS.Parsers.Expressions.Enums;
using Zarem.Assembler.MIPS.Tokenization.Models;
using Zarem.MIPS.Models.Modules.Tables;
using Zarem.MIPS.Models.Modules.Tables.Enums;

namespace Zarem.Assembler.MIPS.Parsers.Expressions;

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
    public override bool TryEvaluate(Evaluator evaluator, out ExpressionResult result)
    {
        // TODO: Special behavior for undefined symbols?

        var symbolName = this.ExpressionToken.Source;
        var address = evaluator.Context?.CurrentAddress ?? default;
        result = new ExpressionResult(Value.Address ?? default, new ReferenceEntry(symbolName, address, MipsReferenceType.None));
        return true;
    }
}
