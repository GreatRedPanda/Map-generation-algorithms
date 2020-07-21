using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

namespace CircularMaze
{

    public class Door
    {
        public List<Vector3Int> DoorTiles = new List<Vector3Int>();

       // public int SectorGroup;
        //public List<Sector> Sectors = new List<Sector>();

        public float DistanceToFirstTile(Vector3Int point)
        {

            return Vector3Int.Distance(DoorTiles[0], point);

        }
    }


    public class Room
    {

        public List<Vector3Int> allTiles
        {
            get
            {
                List<Vector3Int> tiles = new List<Vector3Int>();
                tiles.AddRange(innerTiles);
                tiles.AddRange(outerTile);
                return tiles;
            }

        }
        public List<Vector3Int> innerTiles = new List<Vector3Int>();

       public List<Vector3Int> outerTile = new List<Vector3Int>();


       
    }


    public class Corridor
    {
        public List<Sector> ConnectedSectors = new List<Sector>();
        public List<Vector3Int> CorridorTiles = new List<Vector3Int>();
    }


}