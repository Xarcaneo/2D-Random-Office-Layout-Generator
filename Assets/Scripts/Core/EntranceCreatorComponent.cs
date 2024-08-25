using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The <c>AdjacencyType</c> enum represents the type of adjacency between two rooms.
/// It can be either <c>None</c>, indicating no adjacency, <c>Horizontal</c>, indicating the rooms are adjacent horizontally,
/// or <c>Vertical</c>, indicating the rooms are adjacent vertically.
/// </summary>
public enum AdjacencyType
{
    None,
    Horizontal,
    Vertical
}

/// <summary>
/// The <c>EntranceCreatorComponent</c> class is responsible for creating entrances (doors) in a dungeon layout.
/// It places doors at valid wall positions where there is a corridor tile adjacent to the wall.
/// This class ensures that no room is isolated by connecting rooms through doors, and places doors in a random order to add variety to the dungeon layout.
/// </summary>
public class EntranceCreatorComponent : CoreComponent
{
    // Constants representing the sides where doors can be placed
    private const string TopSide = "Top";
    private const string BottomSide = "Bottom";
    private const string LeftSide = "Left";
    private const string RightSide = "Right";

    /// <summary>
    /// A list of RectInt objects representing rooms without doors.
    /// </summary>
    private List<RectInt> roomsWithoutDoors = new List<RectInt>();

    /// <summary>
    /// A list of RectInt objects representing rooms with doors.
    /// </summary>
    private List<RectInt> roomsWithDoors = new List<RectInt>();

    /// <summary>
    /// Prefab for the door that will be placed on the North wall.
    /// </summary>
    [SerializeField, Tooltip("Prefab for the door that will be placed on the North wall.")]
    private GameObject doorNorthPrefab;

    /// <summary>
    /// Prefab for the door that will be placed on the South wall.
    /// </summary>
    [SerializeField, Tooltip("Prefab for the door that will be placed on the South wall.")]
    private GameObject doorSouthPrefab;

    /// <summary>
    /// Prefab for the door that will be placed on the West wall.
    /// </summary>
    [SerializeField, Tooltip("Prefab for the door that will be placed on the West wall.")]
    private GameObject doorWestPrefab;

    /// <summary>
    /// Prefab for the door that will be placed on the East wall.
    /// </summary>
    [SerializeField, Tooltip("Prefab for the door that will be placed on the East wall.")]
    private GameObject doorEastPrefab;

    // The TilemapRendererComponent used for rendering tiles in the dungeon
    private TilemapRendererComponent TilemapRendererComponent { get => tilemapRendererComponent ?? core.GetCoreComponent(ref tilemapRendererComponent); }
    private TilemapRendererComponent tilemapRendererComponent;

    /// <summary>
    /// Places doors at random valid positions in each room.
    /// Ensures all rooms are connected by doors and prevents isolated rooms.
    /// </summary>
    /// <param name="grid">The grid representing the dungeon layout.</param>
    /// <param name="rooms">A list of room areas defined by <c>RectInt</c>.</param>
    /// <param name="doorsTransform">The transform to which the doors will be parented.</param>
    public void PlaceDoors(TileType[,] grid, List<RectInt> rooms, Transform doorsTransform)
    {
        foreach (var room in rooms)
        {
            PlaceRandomDoorInRoom(grid, room, doorsTransform);
        }

        while (roomsWithoutDoors.Count > 0)
        {
            bool doorPlaced = false;

            roomsWithDoors.Shuffle();
            roomsWithoutDoors.Shuffle();

            for (int i = roomsWithDoors.Count - 1; i >= 0; i--)
            {
                for (int j = roomsWithoutDoors.Count - 1; j >= 0; j--)
                {
                    AdjacencyType adjacentType = AreRoomsAdjacent(roomsWithDoors[i], roomsWithoutDoors[j]);
                    if (adjacentType != AdjacencyType.None)
                    {
                        doorPlaced = PlaceDoorBetweenRooms(grid, roomsWithDoors[i], roomsWithoutDoors[j], adjacentType, doorsTransform);
                        if (doorPlaced)
                        {
                            roomsWithDoors.Add(roomsWithoutDoors[j]);
                            roomsWithoutDoors.RemoveAt(j);
                            break;
                        }
                    }
                }
                if (doorPlaced) break;
            }

            if (!doorPlaced) break;
        }
    }

    /// <summary>
    /// Places a random door in a room based on valid wall positions.
    /// Valid positions are walls adjacent to corridor tiles.
    /// </summary>
    /// <param name="grid">The grid representing the dungeon layout.</param>
    /// <param name="room">The area of the room defined by <c>RectInt</c>.</param>
    /// <param name="doorsTransform">The transform to which the doors will be parented.</param>
    private void PlaceRandomDoorInRoom(TileType[,] grid, RectInt room, Transform doorsTransform)
    {
        List<(Vector2Int position, string side)> validDoorPositions = new List<(Vector2Int position, string side)>();

        // Check positions for each side of the room
        for (int x = room.xMin + 2; x < room.xMax - 2; x++)
        {
            Vector2Int positionTop = new Vector2Int(x, room.yMax);
            if (IsValidDoorPosition(grid, positionTop, TileType.Corridor, TopSide))
            {
                validDoorPositions.Add((positionTop, TopSide));
            }

            Vector2Int positionBottom = new Vector2Int(x, room.yMin - 1);
            if (IsValidDoorPosition(grid, positionBottom, TileType.Corridor, BottomSide))
            {
                validDoorPositions.Add((positionBottom, BottomSide));
            }
        }

        for (int y = room.yMin + 2; y < room.yMax - 2; y++)
        {
            Vector2Int positionLeft = new Vector2Int(room.xMin - 1, y);
            if (IsValidDoorPosition(grid, positionLeft, TileType.Corridor, LeftSide))
            {
                validDoorPositions.Add((positionLeft, LeftSide));
            }

            Vector2Int positionRight = new Vector2Int(room.xMax, y);
            if (IsValidDoorPosition(grid, positionRight, TileType.Corridor, RightSide))
            {
                validDoorPositions.Add((positionRight, RightSide));
            }
        }

        // Place a door at a random valid position if any were found
        if (validDoorPositions.Count > 0)
        {
            roomsWithDoors.Add(room);
            var (doorPosition, side) = validDoorPositions[Random.Range(0, validDoorPositions.Count)];
            SpawnDoorPrefab(grid, doorPosition, side, doorsTransform);
        }
        else
        {
            roomsWithoutDoors.Add(room);
        }
    }

    /// <summary>
    /// Checks if a position is valid for placing a door.
    /// The position must be a wall with a specific adjacent tile type (Corridor or Floor) in the direction being checked.
    /// </summary>
    /// <param name="grid">The grid representing the dungeon layout.</param>
    /// <param name="position">The position being checked for validity.</param>
    /// <param name="adjacentTileType">The type of tile expected adjacent to the wall (e.g., Corridor or Floor).</param>
    /// <param name="side">The side being checked ("Top", "Bottom", "Left", "Right").</param>
    /// <returns>True if the position is valid, otherwise false.</returns>
    private bool IsValidDoorPosition(TileType[,] grid, Vector2Int position, TileType adjacentTileType, string side)
    {
        if (grid[position.x, position.y] != TileType.Wall) return false;

        switch (side)
        {
            case TopSide:
                return grid[position.x, position.y + 1] == adjacentTileType;  // Check North
            case BottomSide:
                return grid[position.x, position.y - 1] == adjacentTileType;  // Check South
            case LeftSide:
                return grid[position.x - 1, position.y] == adjacentTileType;  // Check West
            case RightSide:
                return grid[position.x + 1, position.y] == adjacentTileType;  // Check East
            default:
                return false;
        }
    }

    /// <summary>
    /// Spawns the appropriate door prefab at the specified position.
    /// Determines the correct door prefab based on the wall side and places it in the dungeon.
    /// </summary>
    /// <param name="grid">The grid representing the dungeon layout.</param>
    /// <param name="position">The position where the door will be placed.</param>
    /// <param name="side">The side where the door will be placed.</param>
    /// <param name="doorsTransform">The transform to which the door will be parented.</param>
    private void SpawnDoorPrefab(TileType[,] grid, Vector2Int position, string side, Transform doorsTransform)
    {
        GameObject doorPrefab = null;
        switch (side)
        {
            case TopSide:
                doorPrefab = doorSouthPrefab; // Assuming "Top" corresponds to the door opening to the south
                break;
            case BottomSide:
                doorPrefab = doorNorthPrefab; // Assuming "Bottom" corresponds to the door opening to the north
                break;
            case LeftSide:
                doorPrefab = doorEastPrefab; // Assuming "Left" corresponds to the door opening to the east
                break;
            case RightSide:
                doorPrefab = doorWestPrefab; // Assuming "Right" corresponds to the door opening to the west
                break;
        }

        if (doorPrefab != null)
        {
            TilemapRendererComponent.DrawSingleTile(new Vector2Int(position.x, position.y), TileType.Floor, grid);
            grid[position.x, position.y] = TileType.Door;

            var door = Instantiate(doorPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
            door.transform.SetParent(doorsTransform);
        }
    }

    /// <summary>
    /// Removes all child GameObjects from the specified doors transform.
    /// Clears the list of rooms with and without doors.
    /// </summary>
    /// <param name="doorsTransform">The transform whose child objects (doors) will be removed.</param>
    public void RemoveDoors(Transform doorsTransform)
    {
        roomsWithDoors.Clear();
        roomsWithoutDoors.Clear();

        // Iterate through each child of the provided transform and destroy it
        foreach (Transform child in doorsTransform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Places a door between two adjacent rooms, ensuring they are connected in a cohesive and logical manner.
    /// This method first identifies whether the rooms are horizontally or vertically adjacent based on the provided
    /// adjacency type. Then, it calculates the center points of the overlapping section between the rooms, draws a 
    /// line representing the potential path of a door, and finally, it places the door at the first valid wall 
    /// position along that line. The process ensures that the rooms are accessible to one another without compromising 
    /// the integrity of the dungeon layout.
    /// </summary>
    /// <param name="grid">The 2D array representing the dungeon layout, where each tile type determines the room boundaries and structures.</param>
    /// <param name="roomWithDoor">The rectangular region defining the room that already contains an existing door. This room will serve as a reference point.</param>
    /// <param name="roomWithoutDoor">The rectangular region defining the room that currently lacks a door. The goal is to connect this room with the adjacent one.</param>
    /// <param name="adjacentType">The type of adjacency (Horizontal, Vertical, or None) between the two rooms.</param>
    /// <param name="doorsTransform">The transform component under which the newly created door object will be parented, ensuring it follows the correct hierarchy in the scene.</param>
    /// <returns>Returns a boolean value indicating whether a door was successfully placed between the two rooms. True if the operation was successful, otherwise false.</returns>
    private bool PlaceDoorBetweenRooms(TileType[,] grid, RectInt roomWithDoor, RectInt roomWithoutDoor, AdjacencyType adjacentType, Transform doorsTransform)
    {
        // Calculate the center points of both rooms
        Vector2Int centerRoomWithDoor = new Vector2Int(
            (roomWithDoor.xMin + roomWithDoor.xMax) / 2,
            (roomWithDoor.yMin + roomWithDoor.yMax) / 2
        );
        Vector2Int centerRoomWithoutDoor = new Vector2Int(
            (roomWithoutDoor.xMin + roomWithoutDoor.xMax) / 2,
            (roomWithoutDoor.yMin + roomWithoutDoor.yMax) / 2
        );

        List<Vector2Int> linePoints = new List<Vector2Int>();
        string doorSide = TopSide; // Default value, will be set correctly below

        if (adjacentType == AdjacencyType.Horizontal)
        {
            // The rooms are adjacent horizontally
            int sharedYMin = Mathf.Max(roomWithDoor.yMin, roomWithoutDoor.yMin);
            int sharedYMax = Mathf.Min(roomWithDoor.yMax, roomWithoutDoor.yMax) - 2;

            int x = roomWithDoor.xMax == roomWithoutDoor.xMin ? roomWithDoor.xMax : roomWithDoor.xMin;

            // Determine correct door side based on the relative positions
            if (roomWithDoor.xMax == roomWithoutDoor.xMin)
            {
                doorSide = RightSide; // Room with door is on the left, door should be on the right
            }
            else if (roomWithDoor.xMin == roomWithoutDoor.xMax)
            {
                doorSide = LeftSide; // Room with door is on the right, door should be on the left
            }

            // Draw vertical line
            for (int y = sharedYMin; y <= sharedYMax; y++)
            {
                linePoints.Add(new Vector2Int(x - 1, y));
            }
        }
        else if (adjacentType == AdjacencyType.Vertical)
        {
            // The rooms are adjacent vertically
            int sharedXMin = Mathf.Max(roomWithDoor.xMin, roomWithoutDoor.xMin);
            int sharedXMax = Mathf.Min(roomWithDoor.xMax, roomWithoutDoor.xMax) - 2;

            int y = roomWithDoor.yMax == roomWithoutDoor.yMin ? roomWithDoor.yMax : roomWithDoor.yMin;

            // Determine correct door side based on the relative positions
            if (roomWithDoor.yMax == roomWithoutDoor.yMin)
            {
                doorSide = TopSide; // Room with door is below, door should be on the top
            }
            else if (roomWithDoor.yMin == roomWithoutDoor.yMax)
            {
                doorSide = BottomSide; // Room with door is above, door should be on the bottom
            }

            // Draw horizontal line
            for (int x = sharedXMin; x <= sharedXMax; x++)
            {
                linePoints.Add(new Vector2Int(x, y - 1));
            }
        }

        if (linePoints.Count > 0)
        {
            // Choose a random point from the collected line points
            Vector2Int randomPoint = linePoints[Random.Range(0, linePoints.Count)];
            // Place the door at the selected random point
            SpawnDoorPrefab(grid, randomPoint, doorSide, doorsTransform);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if two rooms are adjacent (not overlapping).
    /// Two rooms are considered adjacent if they share a wall horizontally or vertically.
    /// </summary>
    /// <param name="room1">The first room.</param>
    /// <param name="room2">The second room.</param>
    /// <returns>Returns the type of adjacency (Horizontal, Vertical, or None).</returns>
    private AdjacencyType AreRoomsAdjacent(RectInt room1, RectInt room2)
    {
        // Check if rooms are adjacent horizontally
        bool adjacentHorizontally =
            (room1.xMin == room2.xMax || room1.xMax == room2.xMin) &&
            (room1.yMin < room2.yMax && room1.yMax > room2.yMin);

        // Check if rooms are adjacent vertically
        bool adjacentVertically =
            (room1.yMin == room2.yMax || room1.yMax == room2.yMin) &&
            (room1.xMin < room2.xMax && room1.xMax > room2.xMin);

        // Determine the type of adjacency
        if (adjacentHorizontally)
        {
            return AdjacencyType.Horizontal;
        }
        else if (adjacentVertically)
        {
            return AdjacencyType.Vertical;
        }
        else
        {
            return AdjacencyType.None;
        }
    }
}
