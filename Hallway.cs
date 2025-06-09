using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hallway
{
    Vector2Int startPosition;
    Vector2Int endPosition;

    Room starRoom;
    Room endRoom;
    public Room StartRoom { get { return starRoom; } set { starRoom = value; } }
    public Room EndRoom { get { return endRoom; } set { endRoom = value; } }

    public Vector2Int StartPositionAbsolute { get { return startPosition + starRoom.Area.position; } }
    public Vector2Int EndPositionAbsolute { get { return endPosition + endRoom.Area.position; } }

    public Hallway(vector2Int startPosition, Room startRoom = null)
    {
        this.StartRoom = startPosition;
        this.StartRoom = startRoom;
    }
}
