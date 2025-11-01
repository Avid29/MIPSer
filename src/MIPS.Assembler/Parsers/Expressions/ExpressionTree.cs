// Adam Dernis 2024

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Parsers.Expressions.Abstract;
using MIPS.Assembler.Parsers.Expressions.Evaluator;
using MIPS.Models.Addressing;

namespace MIPS.Assembler.Parsers.Expressions;

/// <summary>
/// A class representing an expression tree.
/// </summary>
public class ExpressionTree
{
    private ExpNode? _root;
    private OperNode? _activeNode;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionTree"/> class.
    /// </summary>
    public ExpressionTree()
    {
        _root = null;
        _activeNode = null;
    }

    /// <summary>
    /// Attempts to evaluate the value tree.
    /// </summary>
    /// <param name="evaluator">The <see cref="IEvaluator{T}"/> to use in evaluation.</param>
    /// <param name="result">The resulting value of the tree.</param>
    /// <returns><see cref="true"/> if successfully evaluated. <see cref="false"/> otherwise.></returns>
    public bool TryEvaluate(IEvaluator<Address> evaluator, out Address result)
    {
        result = default;
        if (_root is null)
            return false;

        return _root.TryEvaluate(evaluator, out result);
    }

    /// <summary>
    /// Adds a <see cref="AddressNode"/> to the expression tree.
    /// </summary>
    public void AddNode(ExpNode node)
    {
        // Handle first node
        if (_root is null)
        {
            switch (node)
            {
                // A binary operation cannot be added to an empty tree
                case BinaryOperNode:
                    ThrowHelper.ThrowInvalidOperationException("Cannot add binary operator to an empty tree. Insert value node first.");
                    break;
                case UnaryOperNode uNode:
                    _activeNode = uNode;
                    goto default;
                default:
                    _root = node;
                    return;
            }
        }

        // Handle insertion
        switch (node)
        {
            // Insert binary node
            case BinaryOperNode bNode:
                ShiftToInsertionPoint(bNode.Priority);
                if (_activeNode is null)
                {
                    // Node is highest priority and becomes the parent of the root node
                    bNode.LeftChild = _root;
                    _root = node;
                }
                else
                {
                    // Insert operation above current right child and
                    // move right child to node's left child
                    _activeNode.TryInsertNode(bNode);
                }
                break;

            default:
                // Handle double value condition
                if (_activeNode is null)
                {
                    ThrowHelper.ThrowInvalidOperationException("Two value nodes inserted with no operation between them");
                }

                // Attempt to add as a child of the active node
                // Throw if could not add
                if (!_activeNode.TryAddChild(node))
                {
                    ThrowHelper.ThrowInvalidOperationException("Value node inserted with no space in the tree.");
                }
                break;
        }

        // Set active node if root is an operation
        if (node is OperNode oNode)
        {
            _activeNode = oNode;
        }
    }

    /// <remarks>
    /// Sets active node to <see langword="null"/> if this is the greater priority 
    /// </remarks>
    private void ShiftToInsertionPoint(int priority)
    {
        // Look while the active node is lower priority
        while (_activeNode is not null && _activeNode.Priority < priority)
        {
            // Might set the active node to null. If the active node
            // becomes null then the new node goes above the root node
            _activeNode = _activeNode.Parent;
        }
    }
}
