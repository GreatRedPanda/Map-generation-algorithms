using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircularMaze
{
    public class Room
    {


        public List<List<Vector2>> tiles = new List<List<Vector2>>();




        public Vector2Int GetRoomCenter()
        {


            int x = tiles.Count / 2;




            int y = tiles[x].Count / 2;


            return new Vector2Int(x, y);

        }

    }

}