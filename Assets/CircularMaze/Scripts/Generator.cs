using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace CircularMaze
{
    public class Generator : MonoBehaviour
    {
        public static Generator Instance;

        public float SpaceBetweenCircles;

        public GameObject FloorPrefab;
        public GameObject WallPrefab;

        List<Room> rooms = new List<Room>();


        public Tilemap map;

        public Tile TilePrefab;
        public Tile TilePrefabRoad;
        public Tile DoorTile;
        void Awake()
        {

            if (Instance == null)
            {
                Instance = this;
                //Можно еще приписать
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // этот удалит весь объект
                Destroy(gameObject);
                // этот удалит только лишний компонент
                Destroy(this);


            }
        }


        List<List<Sector>> sectors = new List<List<Sector>>();
        public float[] Radiuses;





        void Start()
        {
            int size = 10;

            //   map.BoxFill(Vector3Int.one*100, TilePrefabRoad,  -size * 200, -size * 200, size * 200, size * 200);
            Room mainRoom = new Room();


            for (int i = -(int)Radiuses[0] * size; i < (int)Radiuses[0] * size; i += 10)
            {
                List<Vector2> tilesInRow = new List<Vector2>();
                for (int j = -(int)Radiuses[0] * size; j < (int)Radiuses[0] * size; j += 10)
                {
                    if (i * i + j * j < (Radiuses[0]) * size * size * (Radiuses[0]))
                    {
                        tilesInRow.Add(new Vector2(i, j));

                    }

                }
                mainRoom.tiles.Add(tilesInRow);
            }




            float prevRadius = Radiuses[0] + SpaceBetweenCircles;
            for (int i = 1; i < Radiuses.Length; i += 1)
            {
                List<Sector> sectorsInRaw = new List<Sector>();

                float circleFill = 0;

                float innerRadius = size * (prevRadius);// + SpaceBeenCircles ;
                float outerRadius = size * (Radiuses[i] + prevRadius);// - SpaceBetweenCircles;
                prevRadius += Radiuses[i] + SpaceBetweenCircles;
                while (circleFill < 360)
                {
                    float angle = Random.Range(20, 40);



                    GameObject s = new GameObject("Sector" + sectorsInRaw.Count + "_" + sectors.Count);
                    Sector sector = s.AddComponent<Sector>();

                    sectorsInRaw.Add(sector);

                    sector.Point1 = new Vector2(
                       Mathf.FloorToInt(innerRadius * Mathf.Cos(circleFill * Mathf.Deg2Rad)),
                        Mathf.FloorToInt(innerRadius * Mathf.Sin(circleFill * Mathf.Deg2Rad))
                        );

                    sector.Point2 = new Vector2(
                    Mathf.FloorToInt(outerRadius * Mathf.Cos(circleFill * Mathf.Deg2Rad)),
                   Mathf.FloorToInt(outerRadius * Mathf.Sin(circleFill * Mathf.Deg2Rad))
                      );

                    sector.Point1 = floorToCellSize(sector.Point1, size);
                    sector.Point2 = floorToCellSize(sector.Point2, size);



                    sector.AngleStart = circleFill;
                    circleFill += angle;
                    sector.AngleEnd = circleFill;


                    if (circleFill > 360)
                        circleFill = 360;
                    sector.Point3 = new Vector2(
                      Mathf.FloorToInt(innerRadius * Mathf.Cos(circleFill * Mathf.Deg2Rad)),
                         Mathf.FloorToInt(innerRadius * Mathf.Sin(circleFill * Mathf.Deg2Rad))
                        );
                    sector.Point4 = new Vector2(
                      Mathf.FloorToInt(outerRadius * Mathf.Cos(circleFill * Mathf.Deg2Rad)),
                    Mathf.FloorToInt(outerRadius * Mathf.Sin(circleFill * Mathf.Deg2Rad))
                       );

                    sector.Point3 = floorToCellSize(sector.Point3, size);
                    sector.Point4 = floorToCellSize(sector.Point4, size);


                    sector.InnerRadius = innerRadius;
                    sector.OuterRadius = outerRadius;

                    circleFill += 5;
                }

                sectors.Add(sectorsInRaw);
            }


            //float size=  FloorPrefab.GetComponent<Renderer>().bounds.size.x;
            // Debug.Log(size);
            for (int i = 0; i < sectors.Count; i++)
            {

                for (int k = 0; k < sectors[i].Count; k++)
                {



                    Room r = sectors[i][k].GetRoomInSector(10);
                    sectors[i][k].roomInSector = r;
                    if (r != null)
                    {

                        rooms.Add(r);

                        makeRoomTiles(r);

                    }
                }
            }

            makeRoomTiles(mainRoom);
            for (int j = 0; j < sectors.Count; j++)
            {


                for (int i = 1; i < sectors[j].Count; i++)
                {
                    setLineOnTexture(map, sectors[j][i].GetPoint12Center() / 10, sectors[j][i - 1].GetPoint34Center() / 10, 1);
                    sectors[j][i].connectedSectors.Add(sectors[j][i - 1]);
                    sectors[j][i - 1].connectedSectors.Add(sectors[j][i]);
                }
            }



            for (int j = 0; j < sectors.Count - 1; j++)
            {
                for (int i = 0; i < sectors[j].Count; i++)
                {
                    int k = 0;

                    while (k < sectors[j + 1].Count)
                    {
                        if (sectors[j][i].Intersecting(sectors[j + 1][k]) && k != i)
                        {

                            //  setLineOnTexture(map, sectors[j][i].GetRoomCenter() / 10, sectors[j + 1][k].GetRoomCenter() / 10, 1);

                            setLineOnTexture(map, sectors[j][i].GetRoomCenter() / 10, sectors[j + 1][k].GetRoomCenter() / 10, sectors[j][i], sectors[j + 1][k], 1);

                            sectors[j][i].connectedSectors.Add(sectors[j + 1][k]);
                            sectors[j + 1][k].connectedSectors.Add(sectors[j][i]);
                            break;
                        }
                        k++;
                    }
                }
            }

            for (int i = 0; i < sectors[0].Count; i++)
            {



                setLineOnTexture(map, sectors[0][i].GetRoomCenter() / 10, new Vector2(0, 0), 1);


            }


            // ДВЕРИ ДОБАВЛЯТЬ НА ПЕРЕСЕЯЕНИИ ДОРОГИ С ТАЙЛАМИ  MAP





        }


        void makeRoomTiles(Room r)
        {

            for (int x = 0; x < r.tiles.Count; x++)
            {
                for (int y = 0; y < r.tiles[x].Count; y++)
                {

                    if (y <= 1 || y >= r.tiles[x].Count - 2 || x <= 1 || x >= r.tiles.Count - 2)
                    {
                        Tile tile = Instantiate(TilePrefab);


                        map.SetTile(new Vector3Int((int)r.tiles[x][y].x, (int)r.tiles[x][y].y, 0) / 10, tile);
                    }
                    else
                    {
                        Tile tile = Instantiate(TilePrefab);


                        map.SetTile(new Vector3Int((int)r.tiles[x][y].x, (int)r.tiles[x][y].y, 0) / 10, tile);

                    }
                }
            }
        }

        void setLineOnTexture(Tilemap tex, Vector2 p1, Vector2 p2, int radius)
        {
            int dx = (int)Mathf.Abs(p2.x - p1.x);
            int stepX = (p1.x < p2.x) ? 1 : -1;
            int dy = (int)Mathf.Abs(p2.y - p1.y);
            int stepY = (p1.y < p2.y) ? 1 : -1;

            int error = (dx > dy ? dx : -dy) / 2;



            while (true)
            {
                for (int k = (int)p1.x - radius; k <= p1.x + radius; k++)
                {
                    for (int n = (int)p1.y - radius; n <= p1.y + radius; n++)
                    {
                        TileBase t = tex.GetTile(new Vector3Int(k, n, 0));
                        if (t == null)
                        {
                            Tile tile = Instantiate(TilePrefabRoad);

                            tex.SetTile(new Vector3Int(k, n, 0), tile);

                        }
                        //else
                        //{
                        //    Tile tile = Instantiate(TilePrefab);

                        //    tex.SetTile(new Vector3Int(k, n, 0), tile);
                        //}

                    }
                    //  if (k < tex.width - 1 && k >= 0)
                    //  tex.SetPixel(k, n, col);
                }

                if (p1.x == p2.x && p1.y == p2.y)
                    break;
                int e2 = error;
                if (e2 < dy)
                {

                    error += dx;
                    p1.y += stepY;
                }

                if (e2 > -dx)
                {
                    error -= dy;
                    p1.x += stepX;

                }
            }

        }

        void setLineOnTexture(Tilemap tex, Vector2 p1, Vector2 p2, Sector s1, Sector s2, int radius)
        {
            int dx = (int)Mathf.Abs(p2.x - p1.x);
            int stepX = (p1.x < p2.x) ? 1 : -1;
            int dy = (int)Mathf.Abs(p2.y - p1.y);
            int stepY = (p1.y < p2.y) ? 1 : -1;

            int error = (dx > dy ? dx : -dy) / 2;


            Tile tilePref = null;
            int flag = 0;
            while (true)
            {


                if (flag <= 1 && s1.OuterRadius * s1.OuterRadius < p1.x * p1.x * 100 + p1.y * p1.y * 100)
                {
                    tilePref = (TilePrefabRoad);

                    flag++;
                }
                else
                {
                    tilePref = (TilePrefab);
                }
                for (int k = (int)p1.x - radius; k <= p1.x + radius; k++)
                {
                    for (int n = (int)p1.y - radius; n <= p1.y + radius; n++)
                    {



                        Tile tile = Instantiate(tilePref);


                        tex.SetTile(new Vector3Int(k, n, 0), tile);



                    }
                    //  if (k < tex.width - 1 && k >= 0)
                    //  tex.SetPixel(k, n, col);
                }

                if (p1.x == p2.x && p1.y == p2.y)
                    break;
                int e2 = error;
                if (e2 < dy)
                {

                    error += dx;
                    p1.y += stepY;
                }

                if (e2 > -dx)
                {
                    error -= dy;
                    p1.x += stepX;

                }
            }

        }

        Vector2 floorToCellSize(Vector2 point, int size)
        {

            int ost = (int)point.x / size;
            point.x = size * ost;
            ost = (int)point.y / size;
            point.y = size * ost;

            return point;
        }



        private void OnDrawGizmos()
        {
            //for (int i = -(int)Radiuses[Radiuses.Length - 1] * 10; i < (int)Radiuses[Radiuses.Length - 1] * 10; i += 10)
            //{
            //    for (int j = -(int)Radiuses[Radiuses.Length - 1] * 10; j < (int)Radiuses[Radiuses.Length - 1] * 10; j += 10)
            //    {

            //        if (i * i + j * j < Radiuses[Radiuses.Length - 1] * 10 * Radiuses[Radiuses.Length - 1] * 10)
            //            Gizmos.DrawSphere(new Vector3(i, 0, j), 1);

            //    }
            //}






        }
    }
}