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
        if (node is BinaryOperNode bNode)
        {
            AddNode(bNode);
            return;
        }

        // Handle first value node
        if (_root is null)
        {
            _root = node;

            // Set active node if root is an operation
            if (_root is OperNode operNode)
            {
                _activeNode = operNode;
            }

            return;
        }

        // Ensure tree has an operation for non-first node
        if (_activeNode is null) 
        {
            ThrowHelper.ThrowInvalidOperationException("Second value node inserted with no operation in the tree");
        }

        // Attempt to add to active node
        bool added = _activeNode.TryAddChild(node);

        // Throw if could not add
        if (!added)
        {
            ThrowHelper.ThrowInvalidOperationException("Value node inserted with no space in the tree.");
        }
    }

    /// <summary>
    /// Adds <see cref="BinaryOperNode"/> to the expression tree.
    /// </summary>
    /// <param name="node"></param>
    private void AddNode(BinaryOperNode node)
    {
        // Handle first operation node
        if (_activeNode is null)
        {
            if (_root is null)
            {
                ThrowHelper.ThrowInvalidOperationException("Cannot add binary operator to an empty tree. Insert value node first.");
            }

            // Assign root node to left child and elevate to root
            node.LeftChild = _root;
            _root = node;
            _activeNode = node;
            return;
        }

        ShiftToInsertionPoint(node.Priority);

        if (_activeNode is null)
        {
            // Node is highest priority and becomes the parent of the root node
            node.LeftChild = _root;
        }
        else
        {
            // Insert operation above current right child and
            // move right child to node's left child
            _activeNode.TryInsertNode(node);
        }

        _activeNode = node;
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
