using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using System.Linq;


namespace RowsGeneratingMaze
{
    class Room
    {

        public int StartX;
        public int StartY;
        public int Width;
        public int Height;
        public int EndX;
        public int EndY;

        public int Group = -1;

        //public List<Rect> Exits = new List<Rect>();
        //public List<Room> Neighbours = new List<Room>();

        public Dictionary<Rect, Room> ConnectedRooms = new Dictionary<Rect, Room>();
        public Hashtable RoomsAndExits = new Hashtable();

    }
    public class LevelGenerator : MonoBehaviour
    {

        [HideInInspector]
        public NavMeshSurface2d navMeshSurface;
        [HideInInspector]
        public Transform EnemySpawnPointsParent;
        [HideInInspector]
        public EnemyGroupSpawner EnemyGroupSpawnerPrefab;
        [HideInInspector]
        public PlayerController Player;


        public Vector2 GridScale;
        public Vector2 GridSize;
        public Vector2Int RoomSizeLimit;
        public int LevelWidth;
        public int LevelHeight;
        public int CorridorWidth;
        public Vector2Int DistanceBetweenRooms;
        public Tile WallTile;
        public Tile DoorTile;
        public Tilemap WallsTileMap;
        int[,] map;
        List<List<Room>> roomsByRows;
        List<Room> allRooms = new List<Room>();

        void Start()
        {
            roomsByRows = new List<List<Room>>();
            Generate();

            //Player.transform.position = new Vector3(100 * 0.08f * 1+ 100 * 0.08f * 1/2, 
            //    100 * 0.08f * 1+ 100 * 0.08f * 1 / 2, 0);
            ////StartCoroutine(bake());

        }

        IEnumerator bake()
        {

            yield return new WaitForSeconds(2);
            navMeshSurface.BuildNavMesh();

        }
        void Generate()
        {

            map = new int[LevelWidth + 1, LevelHeight + 1];


            createRooms();
            connectRooms();
            setTiles();





        }

        void createRooms()
        {
            int nextX = 1;
            int nextY = 1;
            int maxY = 1;

            int steps = 0;

            List<Room> roomsInRow = new List<Room>();
            roomsByRows.Add(roomsInRow);
            while (true)
            {
                int randDistanceX = Random.Range(DistanceBetweenRooms.x, DistanceBetweenRooms.y);
                int randDistanceY = Random.Range(DistanceBetweenRooms.x, DistanceBetweenRooms.y);
                nextX += randDistanceX;
                // nextY += randDistanceY;
                int width = Random.Range(RoomSizeLimit.x, RoomSizeLimit.y);
                int height = Random.Range(RoomSizeLimit.x, RoomSizeLimit.y);
                int endX = nextX + width;
                int endY = nextY + height;

                if (endX >= LevelWidth - 1)
                    width = LevelWidth - 1 - nextX;
                if (endY >= LevelHeight - 1)
                    height = LevelHeight - 1 - nextY;
                endX = nextX + width;
                endY = nextY + height;
                if (LevelWidth - 1 - (endX) <= RoomSizeLimit.x)
                {

                    width += LevelWidth - 1 - (endX);
                }
                if (LevelHeight - 1 - endY <= RoomSizeLimit.x)
                {

                    height += LevelHeight - 1 - endY;
                }
                endX = nextX + width;
                endY = nextY + height;
                if (endY > maxY)
                    maxY = nextY + height;
                //  if (width >= RoomSizeLimit.x)
                {
                    carveFloor(nextX, nextY, width, height, map);
                    Room r = new Room()
                    {
                        StartX = nextX,
                        StartY = nextY,
                        EndX = nextX + width,
                        EndY = nextY + height,
                        Height = height,
                        Width = width
                    };
                    roomsInRow.Add(r);
                    allRooms.Add(r);
                }
                nextX += width;


                if (nextX >= LevelWidth - 1)
                {

                    nextY = maxY + 2;
                    if (nextY >= LevelHeight - 1 || maxY + RoomSizeLimit.x >= LevelHeight - 1)
                    {
                        break;

                    }

                    nextX = 1;

                    roomsInRow = new List<Room>();
                    roomsByRows.Add(roomsInRow);
                }
                steps++;

                //if (steps > 100)
                //    break;
            }

        }
        void connectRooms()
        {
            for (int i = 0; i < roomsByRows.Count; i++)
            {
                for (int j = 0; j < roomsByRows[i].Count; j++)
                {

                    Room curRoom = roomsByRows[i][j];
                    int neighbX = j + 1;
                    int neighbY = i + 1;


                    if (neighbY < roomsByRows.Count && j < roomsByRows[neighbY].Count)
                    {

                        Debug.Log(neighbY + "   " + roomsByRows.Count + "    " + j);
                        Room upRoom = roomsByRows[neighbY][j];
                        // сосед выше 
                        //найти промежуток пересечения
                        int possibleXMin = (curRoom.StartX + 1 < upRoom.StartX + 1) ? upRoom.StartX + 1 : curRoom.StartX + 1;
                        int possibleXMax = (curRoom.EndX - CorridorWidth < upRoom.EndX - CorridorWidth) ? curRoom.EndX - CorridorWidth : upRoom.EndX - CorridorWidth;

                        if (possibleXMin < possibleXMax)

                        {
                            int x = Random.Range(possibleXMin, possibleXMax);

                            carveFloor(x, curRoom.EndY, CorridorWidth, upRoom.StartY - curRoom.EndY, map);
                            Rect exit1 = new Rect(x, curRoom.EndY, CorridorWidth, 1);
                            Rect exit2 = new Rect(x, upRoom.StartY - 1, CorridorWidth, 1);

                            curRoom.RoomsAndExits.Add(upRoom, exit1);
                            upRoom.RoomsAndExits.Add(curRoom, exit2);

                        }
                    }
                    if (neighbX != roomsByRows[i].Count)
                    {

                        Room rightRoom = roomsByRows[i][neighbX];
                        // сосед выше 
                        //найти промежуток пересечения
                        int possibleYMin = (curRoom.StartY + 1 < rightRoom.StartY + 1) ? rightRoom.StartY + 1 : curRoom.StartY + 1;
                        int possibleYMax = (curRoom.EndY - CorridorWidth < rightRoom.EndY - CorridorWidth) ? curRoom.EndY - CorridorWidth : rightRoom.EndY - CorridorWidth;

                        if (possibleYMin < possibleYMax)
                        {
                            int y = Random.Range(possibleYMin, possibleYMax);

                            carveFloor(curRoom.EndX, y, rightRoom.StartX - curRoom.EndX, CorridorWidth, map);
                            Rect exit1 = new Rect(curRoom.EndX, y, 1, CorridorWidth);
                            Rect exit2 = new Rect(rightRoom.StartX - 1, y, 1, CorridorWidth);

                            curRoom.RoomsAndExits.Add(rightRoom, exit1);
                            rightRoom.RoomsAndExits.Add(curRoom, exit2);
                        }
                    }

                }
            }
        }
        void setTiles()
        {

            createRoomGroups();
            for (int i = 0; i < roomsByRows.Count; i++)
            {
                for (int j = 0; j < roomsByRows[i].Count; j++)
                {
                    Room curRoom = roomsByRows[i][j];
                    foreach (Room neighbour in curRoom.RoomsAndExits.Keys)
                    {

                        if (curRoom.Group != neighbour.Group)
                        {
                            Rect room_exit = (Rect)curRoom.RoomsAndExits[neighbour];

                            carveFloor((int)room_exit.x, (int)room_exit.y,
                                (int)room_exit.width, (int)room_exit.height, map, 2);
                        }
                    }
                }
            }
            for (int i = 0; i < LevelWidth + 1; i++)
            {
                for (int j = 0; j < LevelHeight + 1; j++)
                {
                    if (map[i, j] == 0)
                        WallsTileMap.SetTile(new Vector3Int(i, j, 0), WallTile);
                    else if (map[i, j] == 2)
                        WallsTileMap.SetTile(new Vector3Int(i, j, 0), DoorTile);
                }
            }




        }



        void createRoomGroups()
        {

            List<Room> openRooms = new List<Room>(allRooms);
            int groupsMaxCount = 10;
            int currentGroup = 0;

            Room startRoom = allRooms[Random.Range(0, allRooms.Count)];

            startRoom.Group = currentGroup;

            List<Room> closedRooms = new List<Room>();
            closedRooms.Add(startRoom);
            List<Room> groupPotential = new List<Room>();

            foreach (Room r in startRoom.RoomsAndExits.Keys)
            {
                groupPotential.Add(r);
            }
            Room current = startRoom;
            while (currentGroup != groupsMaxCount || groupPotential.Count != 0)
            {


                for (int i = 0; i < 10; i++)
                {
                    if (groupPotential.Count == 0)
                        break;
                    Room nextRoom = groupPotential[Random.Range(0, groupPotential.Count)];
                    groupPotential.Remove(nextRoom);
                    nextRoom.Group = currentGroup;
                    closedRooms.Add(nextRoom);
                    foreach (Room r in nextRoom.RoomsAndExits.Keys)
                    {
                        if (groupPotential.Contains(r) || closedRooms.Contains(r))
                            continue;
                        groupPotential.Add(r);
                    }
                }
                if (groupPotential.Count == 0)
                    break;
                currentGroup++;
                current = groupPotential[Random.Range(0, groupPotential.Count)];
                groupPotential.Remove(current);
                current.Group = currentGroup;
                closedRooms.Add(current);
                foreach (Room r in current.RoomsAndExits.Keys)
                {
                    if (groupPotential.Contains(r) || closedRooms.Contains(r))
                        continue;
                    groupPotential.Add(r);
                }
            }
        }
        void carveFloor(int startX, int startY, int width, int height, int[,] map, int type = -1)
        {
            for (int i = startX; i < startX + width; i++)
            {
                for (int j = startY; j < startY + height; j++)
                {
                    map[i, j] = type;

                }
            }

            //int x = Random.Range(startX, startX + width);
            //int y = Random.Range(startY, startY + height);
            //addEnemySpawner(x, y);
        }


        void addEnemySpawner(int x, int y)
        {
            EnemyGroupSpawner spawner = Instantiate(EnemyGroupSpawnerPrefab);

            spawner.transform.SetParent(EnemySpawnPointsParent);

            spawner.transform.position = new Vector3(100 * 0.08f * x + 100 * 0.08f * x / 2,
                100 * 0.08f * y + 100 * 0.08f * y / 2, 0);
        }


        private void OnDrawGizmos()
        {
            if (roomsByRows == null)
                return;


            for (int i = 0; i < roomsByRows.Count; i++)
            {
                for (int j = 0; j < roomsByRows[i].Count; j++)
                {
                    if (roomsByRows[i][j].Group == 0)
                        Gizmos.color = Color.red;
                    else
                         if (roomsByRows[i][j].Group == 1)
                        Gizmos.color = Color.yellow;
                    else
                         if (roomsByRows[i][j].Group == 2)
                        Gizmos.color = Color.magenta;
                    else
                         if (roomsByRows[i][j].Group == 3)
                        Gizmos.color = Color.white;
                    else
                         if (roomsByRows[i][j].Group == 4)
                        Gizmos.color = Color.green;
                    else
                         if (roomsByRows[i][j].Group == 5)
                        Gizmos.color = Color.black;
                    else
                         if (roomsByRows[i][j].Group == 6)
                        Gizmos.color = Color.blue;
                    else
                        Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(new Vector3(100 * 0.08f * (roomsByRows[i][j].StartX + roomsByRows[i][j].Width / 2),
                100 * 0.08f * (roomsByRows[i][j].StartY + roomsByRows[i][j].Height / 2), 0), 5);
                }
            }

        }
    }
}