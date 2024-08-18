using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// The <c>TilemapRendererComponent</c> class is responsible for rendering different types of tiles
/// onto a Unity <c>Tilemap</c>. It can draw specific areas, such as floors or corridors, and also
/// draw walls around designated rooms.
/// </summary>
public class TilemapRendererComponent : CoreComponent
{
    /// <summary>
    /// The tile used for rendering walls.
    /// </summary>
    [SerializeField, Tooltip("The tile used for rendering walls.")]
    private TileBase wallTile;

    /// <summary>
    /// The tile used for rendering floors.
    /// </summary>
    [SerializeField, Tooltip("The tile used for rendering floors.")]
    private TileBase floorTile;

    /// <summary>
    /// The tile used for rendering corridors.
    /// </summary>
    [SerializeField, Tooltip("The tile used for rendering corridors.")]
    private TileBase corridorTile;

    /// <summary>
    /// The tile used for rendering grass.
    /// </summary>
    [SerializeField, Tooltip("The tile used for rendering grass.")]
    private TileBase grassTile;

    /// <summary>
    /// The <c>Tilemap</c> where tiles will be drawn.
    /// </summary>
    [SerializeField, Tooltip("The Tilemap where tiles will be drawn.")]
    private Tilemap tilemap;

    /// <summary>
    /// Draws a rectangular area on the Tilemap using the specified tile type.
    /// </summary>
    /// <param name="rect">The area to fill, defined by a <c>RectInt</c>.</param>
    /// <param name="tileType">The type of tile to use for filling the area.</param>
    public void DrawRect(RectInt rect, TileType tileType)
    {
        TileBase tile;

        // Determine which tile to use based on the tile type
        switch (tileType)
        {
            case TileType.Floor:
                tile = floorTile;
                break;
            case TileType.Corridor:
                tile = corridorTile;
                break;
            case TileType.Grass:
                tile = grassTile;
                break;
            default:
                Debug.LogWarning("Unknown TileType, defaulting to Floor tile.");
                tile = floorTile;
                break;
        }

        // Fill the rectangular area with the selected tile
        for (int x = rect.xMin; x < rect.xMax; x++)
        {
            for (int y = rect.yMin; y < rect.yMax; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }

    /// <summary>
    /// Draws walls around the perimeter of the specified room.
    /// </summary>
    /// <param name="room">The room area to surround with walls, defined by a <c>RectInt</c>.</param>
    public void DrawWalls(RectInt room)
    {
        // Draw the top and bottom walls
        for (int x = room.xMin - 1; x <= room.xMax; x++)
        {
            tilemap.SetTile(new Vector3Int(x, room.yMin - 1, 0), wallTile);
            tilemap.SetTile(new Vector3Int(x, room.yMax, 0), wallTile);
        }

        // Draw the left and right walls
        for (int y = room.yMin - 1; y <= room.yMax; y++)
        {
            tilemap.SetTile(new Vector3Int(room.xMin - 1, y, 0), wallTile);
            tilemap.SetTile(new Vector3Int(room.xMax, y, 0), wallTile);
        }
    }

    public void ClearTilemaps() => tilemap.ClearAllTiles();
}
