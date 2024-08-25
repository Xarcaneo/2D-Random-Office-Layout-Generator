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
    /// <param name="grid">The grid to assign tile types to.</param>
    public void DrawRect(RectInt rect, TileType tileType, TileType[,] grid)
    {
        TileBase tile = GetTileForType(tileType);

        // Fill the rectangular area with the selected tile
        for (int x = rect.xMin; x < rect.xMax; x++)
        {
            for (int y = rect.yMin; y < rect.yMax; y++)
            {
                grid[x,y] = tileType;
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }

    /// <summary>
    /// Draws walls around the perimeter of the specified room and assigns the grid tile type.
    /// </summary>
    /// <param name="room">The room area to surround with walls, defined by a <c>RectInt</c>.</param>
    /// <param name="grid">The grid to assign tile types to.</param>
    public void DrawWalls(RectInt room, TileType[,] grid)
    {
        // Draw the top and bottom walls
        for (int x = room.xMin - 1; x <= room.xMax; x++)
        {
            grid[x, room.yMin - 1] = TileType.Wall;
            tilemap.SetTile(new Vector3Int(x, room.yMin - 1, 0), wallTile);

            grid[x, room.yMax] = TileType.Wall;
            tilemap.SetTile(new Vector3Int(x, room.yMax, 0), wallTile);
        }

        // Draw the left and right walls
        for (int y = room.yMin - 1; y <= room.yMax; y++)
        {
            grid[room.xMin - 1, y] = TileType.Wall;
            tilemap.SetTile(new Vector3Int(room.xMin - 1, y, 0), wallTile);

            grid[room.xMax, y] = TileType.Wall;
            tilemap.SetTile(new Vector3Int(room.xMax, y, 0), wallTile);
        }
    }

    /// <summary>
    /// Sets a single tile at the specified position on the Tilemap using the specified tile type.
    /// </summary>
    /// <param name="position">The position on the Tilemap where the tile should be placed.</param>
    /// <param name="tileType">The type of tile to place at the position.</param>
    /// <param name="grid">The grid representing the dungeon layout.</param>
    public void DrawSingleTile(Vector2Int position, TileType tileType, TileType[,] grid)
    {
        // Get the tile based on the tile type
        TileBase tile = GetTileForType(tileType);

        // Update the grid to reflect the new tile type
        grid[position.x, position.y] = tileType;

        // Set the tile on the tilemap at the specified position
        tilemap.SetTile(new Vector3Int(position.x, position.y, 0), tile);
    }

    /// <summary>
    /// Gets the appropriate tile based on the tile type provided.
    /// </summary>
    /// <param name="tileType">The type of tile to get.</param>
    /// <returns>The corresponding <c>TileBase</c> for the provided tile type.</returns>
    private TileBase GetTileForType(TileType tileType)
    {
        switch (tileType)
        {
            case TileType.Floor:
                return floorTile;
            case TileType.Corridor:
                return corridorTile;
            case TileType.Grass:
                return grassTile;
            default:
                Debug.LogWarning($"Unknown TileType: {tileType}, defaulting to Floor tile.");
                return floorTile;
        }
    }

    public void ClearTilemaps() => tilemap.ClearAllTiles();
}
