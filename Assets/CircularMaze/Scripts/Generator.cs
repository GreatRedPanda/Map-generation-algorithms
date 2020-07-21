using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Diagnostics;

namespace CircularMaze
{

    [System.Serializable]
  public  struct CircleParameter
    {

        public float RingWidth;
        public float MinRoomAngle;
        public float MaxRoomAngle;
       
    }

    public class Generator : MonoBehaviour
    {
       
        public float SpaceBetweenCircles;
        public float SpaceBetweenSectors=5;

        public Vector2 StartAngleRandom;

       


        public Tilemap map;
        public Tilemap doorsMap;
        public Tilemap shadows;
        public Tilemap wallsMap;

        public TileBase WallTile2;
        public FloorToWallTile WallTile;
        public FloorWIthWall FloorTile;
        public TileBase DoorTile;
        public Tile ShadowTile;

        public Tilemap TestOuterWalls;
  
        public CircleParameter[] Circles;

        public GameObject KeyPrefab;
        public List<LevelObject> Prefabs;



        List<List<Sector>> sectors = new List<List<Sector>>();
        List<Sector> allSectors = new List<Sector>();
        List<Door> doors = new List<Door>();
        List<Corridor> corridors = new List<Corridor>();

        Sector mainSector;
        void Start()
        {
            WallTile.InitTile(shadows);


            GameObject ms = new GameObject("Sector center");
             mainSector = ms.AddComponent<Sector>();

            mainSector.InnerRadius = Circles[0].RingWidth;
            mainSector.OuterRadius = Circles[0].RingWidth;
            mainSector.AngleStart = 0;
            mainSector.AngleEnd = 360;


            mainSector.Point1 = new Vector2(-(int)Circles[0].RingWidth, -(int)Circles[0].RingWidth);
            mainSector.Point2 = new Vector2(-(int)Circles[0].RingWidth, (int)Circles[0].RingWidth);
            mainSector.Point3 = new Vector2((int)Circles[0].RingWidth, -(int)Circles[0].RingWidth);
            mainSector.Point4 = new Vector2((int)Circles[0].RingWidth, (int)Circles[0].RingWidth);
            mainSector.InitSector();

            allSectors.Add(mainSector);
            float prevRadius = Circles[0].RingWidth + SpaceBetweenCircles;
            for (int i = 1; i < Circles.Length; i += 1)
            {
                List<Sector> sectorsInRaw = new List<Sector>();
                float startAngle = Random.Range(StartAngleRandom.x, StartAngleRandom.y);
                float circleFill = 0;

                float innerRadius = (prevRadius);
                float outerRadius =(Circles[i].RingWidth + prevRadius);
                prevRadius += Circles[i].RingWidth + SpaceBetweenCircles;
                while (circleFill < 360)
                {
                    float angle = Random.Range(Circles[i].MinRoomAngle, Circles[i].MaxRoomAngle);

                    if (circleFill + Circles[i].MinRoomAngle+SpaceBetweenSectors > 360)
                    {

                        circleFill = 360;
                        break;
                    }
                    else if (circleFill + angle  > 360)
                    {

                        angle = Circles[i].MinRoomAngle;
                    }

                    GameObject s = new GameObject("Sector" + sectorsInRaw.Count + "_" + sectors.Count);
                    Sector sector = s.AddComponent<Sector>();

                    sectorsInRaw.Add(sector);

                    sector.Point1 = new Vector2(
                       Mathf.FloorToInt(innerRadius * Mathf.Cos((circleFill+startAngle) * Mathf.Deg2Rad)),
                        Mathf.FloorToInt(innerRadius * Mathf.Sin((circleFill + startAngle) * Mathf.Deg2Rad))
                        );

                    sector.Point2 = new Vector2(
                    Mathf.FloorToInt(outerRadius * Mathf.Cos((circleFill + startAngle) * Mathf.Deg2Rad)),
                   Mathf.FloorToInt(outerRadius * Mathf.Sin((circleFill + startAngle) * Mathf.Deg2Rad))
                      );
                    sector.AngleStart = (circleFill + startAngle);
                    circleFill += angle;
                    sector.AngleEnd = (circleFill + startAngle);


                 
                    sector.Point3 = new Vector2(
                      Mathf.FloorToInt(innerRadius * Mathf.Cos((circleFill + startAngle) * Mathf.Deg2Rad)),
                         Mathf.FloorToInt(innerRadius * Mathf.Sin((circleFill + startAngle) * Mathf.Deg2Rad))
                        );
                    sector.Point4 = new Vector2(
                      Mathf.FloorToInt(outerRadius * Mathf.Cos((circleFill + startAngle) * Mathf.Deg2Rad)),
                    Mathf.FloorToInt(outerRadius * Mathf.Sin((circleFill + startAngle) * Mathf.Deg2Rad))
                       );
         
                    sector.InnerRadius = innerRadius;
                    sector.OuterRadius = outerRadius;

                    sector.InitSector();
                    allSectors.Add(sector);
                    circleFill += SpaceBetweenSectors;
                }

                sectors.Add(sectorsInRaw);
            }

            for (int j = 0; j < sectors.Count; j++)
            {
                for (int i = 1; i < sectors[j].Count; i++)
                {                
                   float radius = (sectors[j][i].InnerRadius+ sectors[j][i].OuterRadius) / 2;
                   setArcBetweenSectors(map,  sectors[j][i - 1], sectors[j][i], radius);
                     sectors[j][i].connectedSectors.Add(sectors[j][i - 1]);
                    sectors[j][i - 1].connectedSectors.Add(sectors[j][i]);
                }
              float r = (sectors[j][sectors[j].Count - 1].InnerRadius + sectors[j][sectors[j].Count - 1].OuterRadius) / 2;
               setArcBetweenSectorsPi(map, sectors[j][sectors[j].Count - 1],  sectors[j][0],r);
                sectors[j][0].connectedSectors.Add(sectors[j][sectors[j].Count - 1]);
                sectors[j][sectors[j].Count - 1].connectedSectors.Add(sectors[j][0]);
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
                            setLineOnTexture(map, sectors[j][i].GetRoomCenter() , sectors[j + 1][k].GetRoomCenter() , sectors[j][i], sectors[j + 1][k], 1);
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
                sectors[0][i].connectedSectors.Add(mainSector);
                mainSector.connectedSectors.Add(sectors[0][i]);
                setLineOnTexture(map, sectors[0][i].GetRoomCenter() , mainSector.GetRoomCenter() , sectors[0][i], mainSector, 1);
            }
            List<Sector> mainRow = new List<Sector>() { mainSector };
            sectors.Insert(0, mainRow);
            createRoomGroups();
           // makeDoors();
            doorsMap.GetComponent<DoorTilemap>().doors = doors;         
            calculateLevelObjectsPrefSize();
            vizualizaSectorsGroup(0);
         //   vizualizaSectorsGroup(1);
            generateKeys(0);
        }


        List<Corridor> visualizedCorridors = new List<Corridor>();

        void vizualizaSectorsGroup(int currentGroup)
        {
          
            
            List<Sector> sectorsInGroup = allSectors.FindAll(x => x.Group == currentGroup);

            List<Vector3Int> takenPlaces = new List<Vector3Int>();
            foreach (var s in sectorsInGroup)
            {

                
                Room r = s.GetRoom();
                s.roomInSector = r;

                takenPlaces.AddRange(r.innerTiles);
                if (r != null)
                {
                    makeRoomTiles(r);
                }

                List<Corridor> sectorCorridors = corridors.FindAll(x => x.ConnectedSectors.Contains(s) && !visualizedCorridors.Contains(x));

                foreach (var c in sectorCorridors)
                {
                    foreach (var tile in c.CorridorTiles)
                    {
                        setFloorTile(tile, map);
                    }
                    visualizedCorridors.Add(c);
                    takenPlaces.AddRange(c.CorridorTiles);
                }



            }
            makeWalls(takenPlaces);

            foreach (var s in sectorsInGroup)
            {
                makeDoors(s);

            }


        }


        void makeWalls(List<Vector3Int>  takenPlaces)
        {
            UnityEngine.Debug.Log("TAKEN TILES COUNT " + takenPlaces.Count +"  "+ takenPlaces[0]);

            foreach (var item in takenPlaces)
            {


                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        Vector3Int neighbourPos = new Vector3Int(item.x + i, item.y + j,0);

                        if ((map.GetTile(neighbourPos) == null))
                        {

                            wallsMap.SetTile(neighbourPos, WallTile2);
                        }
                        else
                            wallsMap.SetTile(neighbourPos, null);


                        if (i == -1 && j==0 )
                        {
                            if ((map.GetTile(neighbourPos) == null))
                            {

                                shadows.SetTile(item, ShadowTile);
                            }
                            else
                            {
                                shadows.SetTile(item, null);
                                shadows.SetTile(item+Vector3Int.right, null);


                            }

                        }
                    }
                }
            }
        }
        void createRoomGroups()
        {

            List<Sector> openRooms = new List<Sector>(allSectors);


            int groupsMaxCount = 10;
            int currentGroup = 0;

           // Sector startSector = openRooms[Random.Range(0, openRooms.Count)];
            Sector startSector = openRooms[0];

            startSector.Group = currentGroup;

            List<Sector> closedRooms = new List<Sector>();
            closedRooms.Add(startSector);
            List<Sector> groupPotential = new List<Sector>();

            foreach (Sector r in startSector.connectedSectors)
            {
                groupPotential.Add(r);
            }
            Sector current = startSector;
            while (currentGroup != groupsMaxCount || groupPotential.Count != 0)
            {


                for (int i = 0; i < 5; i++)
                {
                    if (groupPotential.Count == 0)
                        break;
                    Sector nextRoom = groupPotential[Random.Range(0, groupPotential.Count)];
                    groupPotential.Remove(nextRoom);
                    nextRoom.Group = currentGroup;
                    closedRooms.Add(nextRoom);
                    foreach (Sector r in nextRoom.connectedSectors)
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
                foreach (Sector r in current.connectedSectors)
                {
                    if (groupPotential.Contains(r) || closedRooms.Contains(r))
                        continue;
                    groupPotential.Add(r);
                }
            }
        }


      

        void generateKeys(int currentGroup)
        {

            List<Sector> currentSector = allSectors.FindAll(x => x.Group == currentGroup);
            {
                Sector sectorToPlaceKey= currentSector[Random.Range(0, currentSector.Count)];

                GameObject key = Instantiate(KeyPrefab);

                key.transform.position = sectorToPlaceKey.roomInSector.innerTiles[Random.Range(0, sectorToPlaceKey.roomInSector.innerTiles.Count)];

                key.transform.position *= 0.64f;
                key.GetComponent<SpriteRenderer>().color = getSectorColor(sectorToPlaceKey.Group+1);

                //currentGroup++;

              //  currentSector = allSectors.FindAll(x => x.Group == currentGroup);

            }

        }


        void makeRoomTiles(Room r)
        {
            foreach (var t in r.innerTiles)
            {
                setFloorTile(new Vector3Int((int)t.x, (int)t.y, 0) , map);
            }
        }



        void calculateLevelObjectsPrefSize()
        {
            foreach (var ls in Prefabs)
            {
                ls.CalculatePrefabSize();
            }
            Prefabs.OrderBy(x => x.XSize);      
        }



        bool generated = false;
        int MaxSize=3;
        Stopwatch stopwatch = new Stopwatch();

        int CurrentGroup = 0;

        private void Update()
        {
            if (!generated)
            {
                generated = true;

                stopwatch.Start();
                generatestuff(0);


                stopwatch.Stop();
                CurrentGroup++;
            }


            if (Input.GetKeyDown(KeyCode.T))
            {
                   vizualizaSectorsGroup(CurrentGroup);
                generateKeys(CurrentGroup);
                generatestuff(CurrentGroup);
                CurrentGroup++;
            }

        }

        void generatestuff(int group)
        {

            List<Sector> generateSector = allSectors.FindAll(x => x.Group == group);

            foreach (var s in generateSector)
            {
                List<Vector3Int> sectorFreeDecorTiles = getTilesFronWall(s);

                if (sectorFreeDecorTiles.Count != 0)
                    generateLeftWallDecor(sectorFreeDecorTiles.Count / 2, sectorFreeDecorTiles, 1);

                List<Vector3Int> sectorFreeDecorTilesR = getTilesToRight(s);

                if (sectorFreeDecorTilesR.Count != 0)
                    generateLeftWallDecor(sectorFreeDecorTilesR.Count / 2, sectorFreeDecorTilesR, -1);

            }
        }



        bool checkObjectDown(Vector3Int pos, int size)
        {
            bool res = false;
            for (int i = -1; i <= size+1; i++)
            {
                if ( (map.GetTile(Vector3Int.right * i + pos) is FloorToWallTile))
                {

                    res = true;
                    break;

                }
            }

            return res;
        }

        List<Vector3Int> takenTiles = new List<Vector3Int>();
        void generateLeftWallDecor(int triesCount, List<Vector3Int> tilesPlaces, int direction)
        {


            List<Vector3Int> freeTiles = new List<Vector3Int>(tilesPlaces);

            for (int j = 0; j < triesCount && freeTiles.Count > 0; j++)
            {
                int randPos = Random.Range(0, freeTiles.Count);
                int freePlacesByX = 0;
                int freePlacesByY = 0;
               
                Vector3Int pos = freeTiles[randPos];

                for (int i = 0; i <MaxSize; i++)
                {
                    int freePlacesColumn = 0;
                    for (int k = 0; k < MaxSize; k++)
                    {

                        Vector3Int neibPos = pos + direction * Vector3Int.right * i + Vector3Int.up * k;
                        TileBase neighbour = map.GetTile(neibPos);
                        int nIndex = takenTiles.IndexOf(neibPos);
                        if (!( neighbour is FloorToWallTile) && nIndex == -1 && doorsMap.GetTile(neibPos) == null
                           // && !( map.GetTile(pos+ Vector3Int.right * i + Vector3Int.up * k) is FloorToWallTile)
                            )
                        {

                            freePlacesColumn++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (freePlacesColumn > 0)
                    {

                        freePlacesByX++;
                        if (freePlacesByY == 0 || freePlacesByY > freePlacesColumn)
                            freePlacesByY = freePlacesColumn;

                    }
                    else
                        break;
                }
                UnityEngine.Debug.Log(freePlacesByX);

                freePlacesByX--;
                UnityEngine.Debug.Log(freePlacesByX);



                //if(direction==-1)
                //    UnityEngine.Debug.Log("no free places"+freePlacesByX+"   "+freePlacesByY);
                if (freePlacesByX == 0 && freePlacesByY == 0)
                {
                   
                    freeTiles.RemoveAt(randPos);
                    takenTiles.Add(pos);
                    continue;
                }

                List<LevelObjectOrientation> types = new List<LevelObjectOrientation>(){ LevelObjectOrientation.All };
                if (direction == 1)
                {
                    if (map.GetTile(pos + Vector3Int.left) is FloorToWallTile && map.GetTile(pos + Vector3Int.up * freePlacesByY + Vector3Int.left) is FloorToWallTile)
                        types.Add(LevelObjectOrientation.LeftOnly);
                }

                if (map.GetTile(pos + Vector3Int.down) is FloorToWallTile)
                {
                    types = new List<LevelObjectOrientation>() { LevelObjectOrientation.DownOnly };
                }
                List<LevelObject> result = Prefabs.FindAll(x => x.XSize <= freePlacesByX && x.YSize <= freePlacesByY && types.Contains(x.Orientation)).ToList();

                if (result.Count == 0)
                    continue;
                int prefabIndex = Random.Range(0, result.Count);
                //if (checkObjectDown(pos + Vector3Int.down, result[prefabIndex].XSize))
                //    continue;
                for (int i = 0; i < result[prefabIndex].XSize; i++)
                {
                  //  for (int k = 0; k < result[prefabIndex].YSize; k++)
                    {


                        freeTiles.Remove(pos + direction * Vector3Int.right * i );
                        takenTiles.Add(pos + direction * Vector3Int.right * i );
                    }
                }


                LevelObject lo = GameObject.Instantiate(result[prefabIndex]);
                lo.transform.parent = transform.parent;

                if (direction == -1)
                    pos += Vector3Int.left * (result[prefabIndex].XSize-1);
                lo.transform.position = ((Vector3)pos) * 0.64f+Vector3.forward*pos.y * 0.16f;
   
                lo.SetShadows(shadows, pos );
            }

        }

        List<Vector3Int> getTilesFronWall(Sector s)
        {

            HashSet<Vector3Int> positions = new HashSet<Vector3Int>(s.roomInSector.innerTiles);
            HashSet <Vector3Int> allCoords = new HashSet<Vector3Int>();
             allCoords.UnionWith(positions);
            List<Vector3Int> freePlacesInSector = allCoords.Where(x => (map.GetTile(x+Vector3Int.up) is FloorWIthWall) && doorsMap.GetTile(x) == null
               && ( !(map.GetTile(x+Vector3Int.up*2) is FloorWIthWall)  || !(map.GetTile(x + Vector3Int.left) is FloorWIthWall))
                                 ).ToList();

            UnityEngine.Debug.Log(freePlacesInSector.Count+ " freePlacesInSector ");
            return freePlacesInSector;
        }


        List<Vector3Int> getTilesToRight(Sector s)
        {

            HashSet<Vector3Int> positions = new HashSet<Vector3Int>(s.roomInSector.innerTiles);
            HashSet<Vector3Int> allCoords = new HashSet<Vector3Int>();
            allCoords.UnionWith(positions);
            List<Vector3Int> freePlacesInSector = allCoords.Where(x => (map.GetTile(x + Vector3Int.up) is FloorWIthWall) &&  !(map.GetTile(x + Vector3Int.right) is FloorWIthWall)).ToList();
            UnityEngine.Debug.Log(freePlacesInSector.Count + " freePlacesInSector TO RIGHT ");
            return freePlacesInSector;
        }



        void setFloorTile(Vector3Int pos, Tilemap map)
        {
            map.SetTile(pos, FloorTile);
        }
        
  


        void setLineOnTexture(Tilemap floor, Vector2 p1, Vector2 p2, Sector s1, Sector s2, int radius)
        {
            int dx = (int)Mathf.Abs(p2.x - p1.x);
            int stepX = (p1.x < p2.x) ? 1 : -1;
            int dy = (int)Mathf.Abs(p2.y - p1.y);
            int stepY = (p1.y < p2.y) ? 1 : -1;

            int error = (dx > dy ? dx : -dy) / 2;      
            Door d1 = new Door();
            Door d2 = new Door();
            s1.DoorsInSector.Add(d1, s2);
            s2.DoorsInSector.Add(d2, s1);

            doors.Add(d1);
            doors.Add(d2);

            Corridor corridor = new Corridor();
            corridor.ConnectedSectors.Add(s1);
            corridor.ConnectedSectors.Add(s2);
            corridors.Add(corridor);
            while (true)
            {              
                for (int k = (int)p1.x - radius; k <= (int)p1.x + radius; k++)
                {
                    for (int n = (int)p1.y - radius; n <= (int)p1.y + radius; n++)
                    {
                      
                     //   setFloorTile(new Vector3Int(k, n, 0), floor);
                        corridor.CorridorTiles.Add(new Vector3Int(k, n, 0));
                        addTileToDoor(floor,d1, s1, d2, s2, k,n);
                    }
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


        void addTileToDoor(Tilemap floor,Door d1, Sector s1, Door d2, Sector s2, int k, int n)
        {

            bool ins1 = s1.PointInsideSector(k, n);
            bool ins2 = s2.PointInsideSector(k, n);
            if (ins1)
            {
                int neighboursInSector = s1.NeighboursNotInSector(k, n);
                if (neighboursInSector != 0 )
                {
                    d1.DoorTiles.Add(new Vector3Int(k, n, 0));
                }
            }
            else if (ins2)
            {
                int neighboursInSector = s2.NeighboursNotInSector(k, n);
                if (neighboursInSector != 0 )
                {
                    d2.DoorTiles.Add(new Vector3Int(k, n, 0));
                }
            }
        }

        void setArcBetweenSectors(Tilemap tex,  Sector s1, Sector s2, float radius)
        {

            float angleStart = s1.AngleEnd-5;
            float angleEnd = s2.AngleStart+5;
            addArcCorridorTiles(s1, s2, angleStart, angleEnd, radius, tex);
        }

        void setArcBetweenSectorsPi(Tilemap tex, Sector s1, Sector s2, float radius)
        {
            float angleStart = s1.AngleEnd-5 ;
            float angleEnd = s2.AngleStart+5 ;
            addArcCorridorTiles(s1, s2, angleStart, 360 + angleEnd, radius, tex);
        }


        void addArcCorridorTiles(Sector s1, Sector s2, float angleStart, float angleEnd, float radius, Tilemap floor)
        {
            float currentAngle = angleStart;
            Door d = new Door();
            Door d2 = new Door();

            s1.DoorsInSector.Add(d, s2);
            s2.DoorsInSector.Add(d2, s1);


            //d.Sectors.Add(s1);
            //d.Sectors.Add(s2);

            //d2.Sectors.Add(s1);
            //d2.Sectors.Add(s2);
            doors.Add(d);
            doors.Add(d2);

            Corridor corridor = new Corridor();
            corridor.ConnectedSectors.Add(s1);
            corridor.ConnectedSectors.Add(s2);
            corridors.Add(corridor);


            while (currentAngle < angleEnd )
            {

                for (float i = radius - 1; i <= radius + 1; i += 0.5f)
                {
                    Vector2 point = new Vector2(
                     Mathf.FloorToInt(i * Mathf.Cos(currentAngle * Mathf.Deg2Rad)),
                   Mathf.FloorToInt(i * Mathf.Sin(currentAngle * Mathf.Deg2Rad))
                      );
                    corridor.CorridorTiles.Add(new Vector3Int((int)point.x, (int)point.y, 0));

                    //  setFloorTile(new Vector3Int((int)point.x, (int)point.y, 0), floor);
                    addTileToDoor(floor, d, s1, d2, s2, (int)point.x, (int)point.y);
                }
                currentAngle += 0.5f;
            }

        }
       


       
        void makeDoors(Sector  sector)
        {
            foreach (var item in sector.DoorsInSector)
            {
                if (item.Value.Group != sector.Group)
                {

                    foreach (var t in item.Key.DoorTiles)
                    {
                        if (isDoorInfrontOfWall(map, t))
                      
                        {

                           // DoorTile.color = getSectorColor((sector.Group > item.Value.Group) ? sector.Group : item.Value.Group);
                            doorsMap.SetTile(t, DoorTile);
                        }
                    }
                }
            }
        }

        bool isDoorInfrontOfWall(Tilemap floor, Vector3Int doorTilePos)
        {
            bool res = true;

            TileBase t = floor.GetTile(doorTilePos+Vector3Int.up);

          // TileBase t2 = floor.GetTile(doorTilePos + Vector3Int.up );

            if (t==null)
                res = false;
            return res;

        }

        private void OnDrawGizmos()
        {
            if (sectors == null)
                return;


            for (int i = 0; i < sectors.Count; i++)
            {
                for (int j = 0; j < sectors[i].Count; j++)
                {

                        Gizmos.color = getSectorColor(sectors[i][j].Group);
                 
                    Vector2 center = sectors[i][j].GetRoomCenter();
                    Gizmos.DrawSphere(new Vector3(center.x * 0.32f*2, center.y * 0.32f*2, 0), 0.32f);
                }
            }

        }

        Color getSectorColor(int group)
        {
            if (group == 0)  //(roomsByRows[i][j].Group == 0)
               return Color.red;
            else
        if (group == 1)
                return Color.yellow;
            else
        if (group == 2)
                return Color.magenta;
            else
        if (group == 3)
                return Color.white;
            else
        if (group == 4)
                return Color.green;
            else
        if (group == 5)
                return Color.black;
            else
        if (group == 6)
                return  Color.blue;
            else
                return Color.cyan;
        }

    }
}