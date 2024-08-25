/// <summary>
/// Represents the different types of tiles that can be placed in the dungeon grid.
/// </summary>
public enum TileType
{
    /// <summary>
    /// Represents an empty or uninitialized tile in the grid.
    /// </summary>
    Null,

    /// <summary>
    /// Represents a wall tile in the grid.
    /// </summary>
    Wall,

    /// <summary>
    /// Represents a floor tile in the grid.
    /// </summary>
    Floor,

    /// <summary>
    /// Represents a door tile in the grid.
    /// </summary>
    Door,

    /// <summary>
    /// Represents a corridor tile in the grid.
    /// </summary>
    Corridor,

    /// <summary>
    /// Represents a grass tile in the grid.
    /// </summary>
    Grass
}