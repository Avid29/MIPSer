﻿// Avishai Dernis 2025

namespace MIPS.Assembler.Tokenization.Enums;

/// <summary>
/// The tokenizer mode determines the context in which the tokenizer operates.
/// </summary>
public enum TokenizerMode
{
    /// <summary>
    /// Tokenizing assembly code.
    /// </summary>
    Assembly,

    /// <summary>
    /// Tokenizing an expression within a line of assembly code.
    /// </summary>
    Expression,

    /// <summary>
    /// Tokenizing a behavior expression. This is used for the cheatsheet and other documentation purposes.
    /// </summary>
    BehaviorExpression,
}
