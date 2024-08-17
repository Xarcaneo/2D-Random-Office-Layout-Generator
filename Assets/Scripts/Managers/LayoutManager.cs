using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// The <c>LayoutManager</c> class is responsible for generating and managing the layout of rooms and corridors
/// using a Binary Space Partitioning (BSP) tree. It initializes the core components and manages the process
/// of drawing the generated layout on the tilemap.
/// </summary>
public class LayoutManager : MonoBehaviour
{
    /// <summary>
    /// The width of the corridors in the generated layout.
    /// </summary>
    [SerializeField, Tooltip("The width of the corridors in the generated layout.")]
    private int corridorWidth = 6;

    /// <summary>
    /// The minimum size of rooms in the generated layout.
    /// </summary>
    [SerializeField, Tooltip("The minimum size of rooms in the generated layout.")]
    private int minimalRoomSize = 25;


    /// <summary>
    /// Reference to the <c>Core</c> class that manages the core components of the layout.
    /// </summary>
    private Core Core { get; set; }

    /// <summary>
    /// Unity's Start method, which initializes the layout generation process. It creates a BSP tree,
    /// splits it to form rooms and corridors, and then draws the layout on the tilemap.
    /// </summary>
    private void Start()
    {
        Core = GetComponentInChildren<Core>();
        Core.Initialize();

        // Define the bounds of the layout area
        var bounds = new RectInt(0, 0, 200, 200);

        // Create the root node of the BSP tree with the defined bounds
        var bspNode = new BSPNode(bounds, corridorWidth, minimalRoomSize);

        // Split the BSP node to create rooms and corridors
        bspNode.Split();

        // Collect all nodes from the BSP tree, including nodes with corridors
        List<BSPNode> allNodes = CollectAllNodes(bspNode);

        // Collect only the leaf nodes that represent individual rooms
        List<BSPNode> leafNodes = CollectLeafNodes(bspNode);

        // Draw the layout on the tilemap using the collected nodes
        Draw(leafNodes, allNodes, bounds);
    }

    /// <summary>
    /// Collects all nodes from the BSP tree, including those that represent corridors.
    /// </summary>
    /// <param name="node">The root node of the BSP tree or subtree.</param>
    /// <returns>A list of all nodes in the BSP tree.</returns>
    private List<BSPNode> CollectAllNodes(BSPNode node)
    {
        List<BSPNode> nodes = new List<BSPNode>();

        // Add the current node to the list
        nodes.Add(node);

        // Recursively collect child nodes
        if (node.Left != null) nodes.AddRange(CollectAllNodes(node.Left));
        if (node.Right != null) nodes.AddRange(CollectAllNodes(node.Right));

        return nodes;
    }

    /// <summary>
    /// Collects all leaf nodes from the BSP tree. Leaf nodes represent the final, smallest rooms.
    /// </summary>
    /// <param name="node">The root node of the BSP tree or subtree.</param>
    /// <returns>A list of leaf nodes representing individual rooms.</returns>
    private List<BSPNode> CollectLeafNodes(BSPNode node)
    {
        List<BSPNode> leafNodes = new List<BSPNode>();

        // If the node is a leaf, add it to the list
        if (node.IsLeaf())
        {
            leafNodes.Add(node);
        }
        else
        {
            // Recursively collect leaf nodes from child nodes
            if (node.Left != null) leafNodes.AddRange(CollectLeafNodes(node.Left));
            if (node.Right != null) leafNodes.AddRange(CollectLeafNodes(node.Right));
        }

        return leafNodes;
    }

    /// <summary>
    /// Draws the generated rooms and corridors onto the tilemap. It first draws the walls around the entire layout,
    /// then draws corridors and rooms within the layout.
    /// </summary>
    /// <param name="leafNodes">The leaf nodes representing rooms in the BSP tree.</param>
    /// <param name="allNodes">All nodes in the BSP tree, including corridors.</param>
    /// <param name="bounds">The bounds of the entire layout area.</param>
    private void Draw(List<BSPNode> leafNodes, List<BSPNode> allNodes, RectInt bounds)
    {
        // Draw walls around the entire layout bounds
        Core.GetCoreComponent<TilemapRendererComponent>().DrawWalls(bounds);

        // Draw corridors in the layout
        foreach (var node in allNodes)
        {
            Core.GetCoreComponent<TilemapRendererComponent>().DrawRect(node.Bounds, TileType.Corridor);
        }

        // Draw rooms and walls around individual rooms
        foreach (var node in leafNodes)
        {
            Core.GetCoreComponent<TilemapRendererComponent>().DrawRect(node.Bounds, TileType.Floor);
            Core.GetCoreComponent<TilemapRendererComponent>().DrawWalls(node.Bounds);
        }
    }
}
