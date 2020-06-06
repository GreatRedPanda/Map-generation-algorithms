using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using System.Linq;


namespace RowsGeneratingMaze
{
    public class GeneratorAdvanced : MonoBehaviour
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
            // connectRooms();
            setTiles();





        }

        void createRooms()
        {
            int nextX = 1;
            int nextY = 1;
            int maxY = 1;

            int steps = 0;

            int maxRowWidh = LevelWidth;
            List<Room> roomsInRow = new List<Room>();
            roomsByRows.Add(roomsInRow);

            Vector2 regionRoomsIndexes = new Vector2();
            int prevRoom = 0;
            while (true)
            {
                int randDistanceX = Random.Range(DistanceBetweenRooms.x, DistanceBetweenRooms.y);
                int randDistanceY = Random.Range(DistanceBetweenRooms.x, DistanceBetweenRooms.y);

                if (roomsInRow.Count == 0)
                    nextX = 1;
                else
                    nextX += randDistanceX;

                int newY = nextY + randDistanceY;

                //if (roomsByRows.Count == 1)
                //    newY = 1;
                int width = Random.Range(RoomSizeLimit.x, RoomSizeLimit.y);
                int height = Random.Range(RoomSizeLimit.x, RoomSizeLimit.y);
                int endX = nextX + width;
                int endY = newY + height;

                if (endX >= maxRowWidh)
                    width = maxRowWidh - nextX;
                if (endY >= LevelHeight)
                    height = LevelHeight - newY;


                endX = nextX + width;
                endY = newY + height;

                if (maxRowWidh - (endX) <= RoomSizeLimit.x)
                {
                    width += maxRowWidh - endX;
                }
                if (LevelHeight - endY <= RoomSizeLimit.x)
                {

                    height += LevelHeight - endY;
                }

                if (width < RoomSizeLimit.x && maxRowWidh - nextX >= RoomSizeLimit.x + DistanceBetweenRooms.x)
                {
                    nextX -= randDistanceX;
                    nextX += DistanceBetweenRooms.x;
                    width = RoomSizeLimit.x;
                    Debug.Log("Not enough space W");
                }
                if (height < RoomSizeLimit.x && LevelHeight - newY >= RoomSizeLimit.x + DistanceBetweenRooms.x)
                {
                    newY -= randDistanceY;
                    newY += DistanceBetweenRooms.x;

                    height = RoomSizeLimit.x;
                    Debug.Log("Not enough space");

                }
                //if (height < RoomSizeLimit.x)
                //    Debug.Log("He9ight lesser than possible " + height);

                endX = nextX + Mathf.Abs(width);
                endY = newY + Mathf.Abs(height);


                if (width >= RoomSizeLimit.x && height >= RoomSizeLimit.x)
                {

                    Room r = new Room()
                    {
                        StartX = nextX,
                        StartY = newY,
                        EndX = endX,
                        EndY = endY,
                        Height = height,
                        Width = width
                    };
                    roomsInRow.Add(r);
                    allRooms.Add(r);
                    if (roomsInRow.Count > 1)
                    {
                        Room prevRoomInRow = roomsInRow[roomsInRow.Count - 2];
                        connectRoomsHorizontal(prevRoomInRow, r);
                    }

                    //connect rooms to left and to bottom


                    int prevRow = roomsByRows.Count - 2;
                    if (prevRow >= 0)
                    {
                        for (int i = (int)regionRoomsIndexes.x; i <= (int)regionRoomsIndexes.y; i++)
                        {
                            Room botRoom = roomsByRows[prevRow][i];
                            bool succsess = connectRoomsVertical(botRoom, r);
                            if (succsess)
                                break;
                        }
                    }


                }
                //  Debug.Log("Row number "+roomsByRows.Count+" END Y"+endY+"  height"+height+"nextY"+nextY+" nextX"+nextX +" nextX+width" + (nextX +width));

                //nextY = endY;
                nextX += Mathf.Abs(width);
                if (roomsByRows.Count > 1 && nextX >= maxRowWidh)
                {
                    regionRoomsIndexes = getgenerationCoordsRegion(out maxRowWidh, out nextY, ref prevRoom);

                    Debug.Log(roomsByRows.Count + "nextY " + nextY);
                }

                if (nextX >= LevelWidth - 1 && nextY + RoomSizeLimit.x >= LevelHeight || endY >= LevelHeight) //|| maxY + RoomSizeLimit.x >= LevelHeight -1
                {
                    break;
                }
                if (nextX >= LevelWidth - 1)
                {


                    roomsInRow = new List<Room>();
                    roomsByRows.Add(roomsInRow);
                    prevRoom = 0;
                    if (roomsByRows.Count > 1)
                    {
                        regionRoomsIndexes = getgenerationCoordsRegion(out maxRowWidh, out nextY, ref prevRoom);
                    }
                }
                steps++;

                if (steps > 1000)
                    break;
            }



            List<Room> lastRow = new List<Room>();

            List<Room> lastAddedRow = roomsByRows[roomsByRows.Count - 1];
            //roomsByRows.Add(roomsInRow);
            for (int i = 0; i < lastAddedRow.Count; i++)
            {
                Room r = lastAddedRow[i];

                if (LevelHeight - r.EndY > RoomSizeLimit.x)
                {

                    //Room newRoom = new Room();
                    //newRoom.StartX = r.StartX;
                    //newRoom.Width = r.Width;
                    //newRoom.EndX = r.EndX;

                    //newRoom.StartY = r.EndY + 1;
                    //newRoom.EndY = LevelHeight ;
                    //newRoom.Height = newRoom.EndY - newRoom.StartY;

                    //if (newRoom.Height >= RoomSizeLimit.x)
                    //{
                    //    lastRow.Add(newRoom);
                    //    connectRoomsVertical(r, newRoom);
                    //}
                }
                else
                {


                    r.Height += LevelHeight - r.EndY;
                    r.EndY = LevelHeight;
                }
            }

            //if (lastRow.Count > 0)
            //{
            //    for (int i = 1; i < lastRow.Count; i++)
            //    {
            //        connectRoomsHorizontal(lastRow[i - 1], lastRow[i]);

            //    }
            //    roomsByRows.Add(lastRow);
            //}
        }



        Vector2 getgenerationCoordsRegion(out int horRegion, out int yCoord, ref int prevRoom)
        {

            Vector2 regionRoomsIndexes = new Vector2(prevRoom, roomsByRows[roomsByRows.Count - 2].Count - 1);

            int newWidth = roomsByRows[roomsByRows.Count - 2][prevRoom].EndX;

            int newY = roomsByRows[roomsByRows.Count - 2][prevRoom].EndY;
            int prevSum = newY + DistanceBetweenRooms.x;
            for (int i = prevRoom + 1; i < roomsByRows[roomsByRows.Count - 2].Count; i++)
            {
                //if (roomsByRows[roomsByRows.Count - 2][i].EndY == roomsByRows[roomsByRows.Count - 2][i - 1].EndY)

                if (Mathf.Abs(roomsByRows[roomsByRows.Count - 2][i].EndY - roomsByRows[roomsByRows.Count - 2][i - 1].EndY) < DistanceBetweenRooms.x)
                {
                    if (i == roomsByRows[roomsByRows.Count - 2].Count - 1)
                        newWidth = LevelWidth;
                    else
                        newWidth = roomsByRows[roomsByRows.Count - 2][i + 1].StartX;
                    regionRoomsIndexes.y = i;
                    if (prevSum < roomsByRows[roomsByRows.Count - 2][i].EndY + DistanceBetweenRooms.x)
                    {

                        newY = roomsByRows[roomsByRows.Count - 2][i].EndY;
                        prevSum = roomsByRows[roomsByRows.Count - 2][i].EndY + DistanceBetweenRooms.x;
                    }
                }
                else
                {
                    prevRoom = i;
                    break;
                }
            }

            horRegion = newWidth - 1;
            yCoord = newY;
            return regionRoomsIndexes;

        }


        bool connectRoomsHorizontal(Room leftRoom, Room rightRoom)
        {
            int possibleYMin = (leftRoom.StartY + 1 < rightRoom.StartY + 1) ? rightRoom.StartY + 1 : leftRoom.StartY + 1;
            int possibleYMax = (leftRoom.EndY - CorridorWidth < rightRoom.EndY - CorridorWidth) ? leftRoom.EndY - CorridorWidth : rightRoom.EndY - CorridorWidth;

            if (possibleYMin < possibleYMax)
            {
                int y = Random.Range(possibleYMin, possibleYMax);

                carveFloor(leftRoom.EndX + 1, y, rightRoom.StartX - leftRoom.EndX - 2, CorridorWidth, map);
                Rect exit1 = new Rect(leftRoom.EndX, y, 1, CorridorWidth);
                Rect exit2 = new Rect(rightRoom.StartX - 1, y, 1, CorridorWidth);

                leftRoom.RoomsAndExits.Add(rightRoom, exit1);
                rightRoom.RoomsAndExits.Add(leftRoom, exit2);
                return true;
            }
            return false;
        }

        bool connectRoomsVertical(Room botRoom, Room upRoom)
        {
            int possibleXMin = (botRoom.StartX + 1 < upRoom.StartX + 1) ? upRoom.StartX + 1 : botRoom.StartX + 1;
            int possibleXMax = (botRoom.EndX - CorridorWidth < upRoom.EndX - CorridorWidth) ? botRoom.EndX - CorridorWidth : upRoom.EndX - CorridorWidth;

            if (possibleXMin < possibleXMax)

            {
                int x = Random.Range(possibleXMin, possibleXMax);

                carveFloor(x, botRoom.EndY, CorridorWidth, upRoom.StartY - botRoom.EndY, map);
                Rect exit1 = new Rect(x, botRoom.EndY, CorridorWidth, 1);
                Rect exit2 = new Rect(x, upRoom.StartY - 1, CorridorWidth, 1);

                botRoom.RoomsAndExits.Add(upRoom, exit1);
                upRoom.RoomsAndExits.Add(botRoom, exit2);
                return true;
            }
            return false;
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
                        connectRoomsVertical(curRoom, upRoom);
                    }
                    if (neighbX != roomsByRows[i].Count)
                    {

                        Room rightRoom = roomsByRows[i][neighbX];
                        connectRoomsHorizontal(curRoom, rightRoom);
                    }


                    // getRoomsNeighbourLeft(i, j, 1, curRoom);
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
                    carveFloor(curRoom.StartX, curRoom.StartY, curRoom.Width, curRoom.Height, map);
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
            //for (int i = 0; i < LevelWidth + 1; i++)
            //{
            //    for (int j = 0; j < LevelHeight + 1; j++)
            //    {
            //        if (map[i, j] == 0)
            //            WallsTileMap.SetTile(new Vector3Int(i, j, 0), WallTile);
            //        else if (map[i, j] == 2)
            //            WallsTileMap.SetTile(new Vector3Int(i, j, 0), DoorTile);
            //    }
            //}




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


            for (int i = startX - 1; i <= startX + width + 1; i++)
            {
                WallsTileMap.SetTile(new Vector3Int(i, startY - 1, 0), WallTile);
                WallsTileMap.SetTile(new Vector3Int(i, startY + 1 + height, 0), WallTile);
            }

            for (int i = startY - 1; i <= startY + height; i++)
            {
                WallsTileMap.SetTile(new Vector3Int(startX - 1, i, 0), WallTile);
                WallsTileMap.SetTile(new Vector3Int(startX + 1 + width, i, 0), WallTile);
            }
            //        if (map[i, j] == 0)
            //            WallsTileMap.SetTile(new Vector3Int(i, j, 0), WallTile);
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
                    if (i == 0)  //(roomsByRows[i][j].Group == 0)
                        Gizmos.color = Color.red;
                    else
                         if (i == 1)
                        Gizmos.color = Color.yellow;
                    else
                         if (i == 2)
                        Gizmos.color = Color.magenta;
                    else
                         if (i == 3)
                        Gizmos.color = Color.white;
                    else
                         if (i == 4)
                        Gizmos.color = Color.green;
                    else
                         if (i == 5)
                        Gizmos.color = Color.black;
                    else
                         if (i == 6)
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


    //void getRoomsNeighbourLeft(int rowNumber, int columnNumber, int radius, Room r)
    //{

    //    for (int i = rowNumber - radius; i < rowNumber + radius; i++)
    //    {
    //        for (int j = columnNumber - radius; j < columnNumber + radius; j++)
    //        {

    //            if (i > roomsByRows.Count - 1 || i < 0)
    //                continue;

    //            if (j < 0 || j > roomsByRows[i].Count - 1)
    //                break;
    //            Room curRoom = roomsByRows[i][j];
    //            if (r == curRoom)
    //                continue;

    //            if (r.RoomsAndExits.ContainsKey(curRoom))
    //                continue;


    //              int possibleXMin = (r.StartX + 1 < curRoom.StartX + 1) ? curRoom.StartX + 1 : r.StartX + 1;
    //                int possibleXMax = (r.EndX - CorridorWidth < curRoom.EndX - CorridorWidth) ? r.EndX - CorridorWidth : curRoom.EndX - CorridorWidth;

    //                if (possibleXMin < possibleXMax)
    //                {
    //                    int x = Random.Range(possibleXMin, possibleXMax);

    //                    carveFloor(x, r.EndY, CorridorWidth, curRoom.StartY - r.EndY, map);
    //                    Rect exit1 = new Rect(x, r.EndY, CorridorWidth, 1);
    //                    Rect exit2 = new Rect(x, curRoom.StartY - 1, CorridorWidth, 1);

    //                    r.RoomsAndExits.Add(curRoom, exit1);
    //                curRoom.RoomsAndExits.Add(r, exit2);

    //                }
    //            //hor intersecting

    //            int possibleYMin = (r.StartY + 1 < curRoom.StartY + 1) ? curRoom.StartY + 1 : r.StartY + 1;
    //            int possibleYMax = (r.EndY - CorridorWidth < curRoom.EndY - CorridorWidth) ? r.EndY - CorridorWidth : curRoom.EndY - CorridorWidth;

    //            if (possibleYMin < possibleYMax)
    //            {
    //                int y = Random.Range(possibleYMin, possibleYMax);

    //                carveFloor(r.EndX, y, curRoom.StartX - r.EndX, CorridorWidth, map);
    //                Rect exit1 = new Rect(r.EndX, y, 1, CorridorWidth);
    //                Rect exit2 = new Rect(curRoom.StartX - 1, y, 1, CorridorWidth);

    //                r.RoomsAndExits.Add(curRoom, exit1);
    //                curRoom.RoomsAndExits.Add(r, exit2);
    //            }
    //        }
    //    }
    //}
