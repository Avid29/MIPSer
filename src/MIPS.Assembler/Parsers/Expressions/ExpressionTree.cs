// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using MIPS.Assembler.Parsers.Expressions.Abstract;
using MIPS.Assembler.Parsers.Expressions.Evaluator;

namespace MIPS.Assembler.Parsers.Expressions;

/// <summary>
/// A class representing an expression tree.
/// </summary>
public class ExpressionTree<T>
{
    private ExpNode<T>? _root;
    private OperNode<T>? _activeNode;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionTree{T}"/> class.
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
    public bool TryEvaluate(IEvaluator<T> evaluator, out T? result)
    {
        result = default;
        if (_root is null)
            return false;

        return _root.TryEvaluate(evaluator, out result);
    }

    /// <summary>
    /// Adds a <see cref="ValueNode{T}"/> to the expression tree.
    /// </summary>
    public void AddNode(ValueNode<T> node)
    {
        // Handle first value node
        if (_root is null)
        {
            _root = node;
            return;
        }

        // Ensure tree has an operation for non-first node
        if (_activeNode is null)
        {
            ThrowHelper.ThrowInvalidOperationException("Second value node inserted with no operation in the tree");
        }

        // Ensure tree has space for node
        if (_activeNode.RightChild is not null)
        {
            ThrowHelper.ThrowInvalidOperationException("Value node inserted with no space in the tree.");
        }

        // Set child as active node's right child
        _activeNode.RightChild = node;
    }

    /// <summary>
    /// Adds <see cref="OperNode{T}"/> to the expression tree.
    /// </summary>
    /// <param name="node"></param>
    public void AddNode(OperNode<T> node)
    {
        // Handle first operation node
        if (_activeNode is null)
        {
            if (_root is null)
            {
                ThrowHelper.ThrowInvalidOperationException("Cannot add operation to any empty tree. Insert value node first.");
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
            node.LeftChild = _activeNode.RightChild;
            _activeNode.RightChild = node;
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
