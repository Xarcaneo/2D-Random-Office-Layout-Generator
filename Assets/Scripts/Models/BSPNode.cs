using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a node in a Binary Space Partitioning (BSP) tree.
/// This node can either be a leaf node with no children or an internal node with two child nodes.
/// Each node is defined by a rectangular area (bounds) and can be split to create two child nodes.
/// </summary>
public class BSPNode
{
    // The bounds of this node, represented as a RectInt (integer rectangle).
    private RectInt _bounds;

    // Left and right child nodes, representing the two areas created after splitting this node.
    private BSPNode _left;
    private BSPNode _right;

    // The width of corridors that will connect rooms created within this node.
    private int _corridorWidth;

    // The minimum size that a room can be within this node.
    private int _minRoomSize;

    // The corridor created during the splitting process, represented as a RectInt.
    public RectInt corridor;

    /// <summary>
    /// Gets the left child node.
    /// </summary>
    public BSPNode Left => _left;

    /// <summary>
    /// Gets the right child node.
    /// </summary>
    public BSPNode Right => _right;

    /// <summary>
    /// Gets the bounding rectangle of this node.
    /// </summary>
    public RectInt Bounds => _bounds;

    /// <summary>
    /// Initializes a new instance of the BSPNode class with the specified bounds, corridor width, and minimum room size.
    /// </summary>
    /// <param name="bounds">The bounding rectangle for this node.</param>
    /// <param name="corridorWidth">The width of the corridors to be created between rooms.</param>
    /// <param name="minRoomSize">The minimum size of a room that can be created.</param>
    public BSPNode(RectInt bounds, int corridorWidth, int minRoomSize)
    {
        _bounds = bounds; // Set the bounds of the node.
        _corridorWidth = corridorWidth; // Set the corridor width.
        _minRoomSize = minRoomSize; // Set the minimum room size.
    }

    /// <summary>
    /// Splits this node into two child nodes based on the given minimum room size and corridor width.
    /// This method recursively splits the node until the resulting areas are too small to be further subdivided.
    /// </summary>
    public void Split()
    {
        // Check if the current region is too small to split further.
        if (_bounds.width <= _minRoomSize * 2 + _corridorWidth && _bounds.height <= _minRoomSize * 2 + _corridorWidth)
        {
            return; // Stop splitting if the area is too small.
        }

        // Determine the direction of the split (horizontal or vertical).
        bool splitHorizontally = DetermineSplitDirection();

        // Perform the split.
        if (splitHorizontally)
        {
            SplitHorizontally();
        }
        else
        {
            SplitVertically();
        }

        // Recursively split the children.
        _left?.Split();
        _right?.Split();
    }

    /// <summary>
    /// Determines whether this node is a leaf (i.e., has no children).
    /// A leaf node represents the smallest indivisible area in the BSP tree.
    /// </summary>
    /// <returns>True if the node is a leaf; otherwise, false.</returns>
    public bool IsLeaf()
    {
        return _left == null && _right == null;
    }

    /// <summary>
    /// Determines the direction of the split based on the node's dimensions and a random factor.
    /// The split direction is chosen based on the aspect ratio of the bounds, or randomly if the aspect ratio is close to square.
    /// </summary>
    /// <returns>True if the node should be split horizontally; otherwise, false.</returns>
    private bool DetermineSplitDirection()
    {
        if (_bounds.width > _bounds.height)
        {
            return false; // Prefer vertical split if the width is greater.
        }
        else if (_bounds.height > _bounds.width)
        {
            return true; // Prefer horizontal split if the height is greater.
        }
        else
        {
            return Random.value > 0.5f; // 50/50 split if the dimensions are equal.
        }
    }

    /// <summary>
    /// Splits the node horizontally, creating two child nodes and a corridor between them.
    /// The split is made along the Y-axis, dividing the area into two smaller rectangles.
    /// </summary>
    private void SplitHorizontally()
    {
        // Determine the Y-coordinate for the split, ensuring there is enough space for rooms and the corridor.
        int splitY = Random.Range(_bounds.yMin + _minRoomSize, _bounds.yMax - _minRoomSize - _corridorWidth);

        // Create left child node from the lower part of the bounds.
        _left = new BSPNode(new RectInt(_bounds.xMin, _bounds.yMin, _bounds.width, splitY - _bounds.yMin), _corridorWidth, _minRoomSize);

        // Create right child node from the upper part of the bounds.
        _right = new BSPNode(new RectInt(_bounds.xMin, splitY + _corridorWidth, _bounds.width, _bounds.yMax - splitY - _corridorWidth), _corridorWidth, _minRoomSize);

        // Create a horizontal corridor between the two child nodes.
        CreateHorizontalCorridor(splitY);
    }

    /// <summary>
    /// Splits the node vertically, creating two child nodes and a corridor between them.
    /// The split is made along the X-axis, dividing the area into two smaller rectangles.
    /// </summary>
    private void SplitVertically()
    {
        // Determine the X-coordinate for the split, ensuring there is enough space for rooms and the corridor.
        int splitX = Random.Range(_bounds.xMin + _minRoomSize, _bounds.xMax - _minRoomSize - _corridorWidth);

        // Create left child node from the left part of the bounds.
        _left = new BSPNode(new RectInt(_bounds.xMin, _bounds.yMin, splitX - _bounds.xMin, _bounds.height), _corridorWidth, _minRoomSize);

        // Create right child node from the right part of the bounds.
        _right = new BSPNode(new RectInt(splitX + _corridorWidth, _bounds.yMin, _bounds.xMax - splitX - _corridorWidth, _bounds.height), _corridorWidth, _minRoomSize);

        // Create a vertical corridor between the two child nodes.
        CreateVerticalCorridor(splitX);
    }

    /// <summary>
    /// Creates a horizontal corridor between two rooms at a specified Y coordinate.
    /// This method ensures the corridor is properly aligned and within bounds.
    /// </summary>
    /// <param name="splitY">The Y coordinate where the corridor is created.</param>
    private void CreateHorizontalCorridor(int splitY)
    {
        // Ensure that the corridor is within the node bounds and not overlapping incorrectly.
        int corridorStartY = Mathf.Clamp(splitY, _bounds.yMin, _bounds.yMax - _corridorWidth);

        // Define the bounds of the corridor.
        RectInt corridorBounds = new RectInt(_bounds.xMin, corridorStartY, _bounds.width, _corridorWidth);

        // Store the corridor bounds.
        corridor = corridorBounds;
    }

    /// <summary>
    /// Creates a vertical corridor between two rooms at a specified X coordinate.
    /// This method ensures the corridor is properly aligned and within bounds.
    /// </summary>
    /// <param name="splitX">The X coordinate where the corridor is created.</param>
    private void CreateVerticalCorridor(int splitX)
    {
        // Ensure that the corridor is within the node bounds and not overlapping incorrectly.
        int corridorStartX = Mathf.Clamp(splitX, _bounds.xMin, _bounds.xMax - _corridorWidth);

        // Define the bounds of the corridor.
        RectInt corridorBounds = new RectInt(corridorStartX, _bounds.yMin, _corridorWidth, _bounds.height);

        // Store the corridor bounds.
        corridor = corridorBounds;
    }
}
