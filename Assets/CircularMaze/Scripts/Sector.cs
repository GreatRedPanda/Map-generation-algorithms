using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CircularMaze
{
    public class Sector : MonoBehaviour
    {

        public float AngleStart;
        public float AngleEnd;

        public float InnerRadius;
        public float OuterRadius;


        public Vector2 Point1;
        public Vector2 Point2;
        public Vector2 Point3;
        public Vector2 Point4;

        public Vector2 Middle1;
        public Vector2 Middle2;


        Color randCOlor;


        Vector2Int Min;
        Vector2Int Max;


        public Room roomInSector;


        public List<Sector> connectedSectors = new List<Sector>();


        public bool Intersecting(Sector other)
        {


            if (AngleStart + 5 > other.AngleEnd - 5 || other.AngleStart + 5 > AngleEnd - 5)
            {
                return false;

            }

            return true;
        }
        public Vector2Int GetPoint12Center()
        {
            //Vector2 minTile = roomInSector.tiles[0][0];
            //Vector2 maxTile = roomInSector.tiles[0][roomInSector.tiles[0].Count-1];

            int x = (int)(Point1.x + Point2.x) / 2;

            int y = (int)(Point1.y + Point2.y) / 2;

            return new Vector2Int(x, y);

        }
        public Vector2Int GetPoint34Center()
        {

            int x = (int)(Point3.x + Point4.x) / 2;

            int y = (int)(Point3.y + Point4.y) / 2;

            return new Vector2Int(x, y);

        }


        public Vector2Int GetRoomCenter()
        {

            //int x = (int)(Point1.x + Point4.x) / 2;




            //int y = (int)(Point1.y + Point1.y) / 2;

            int x = (Min.x + Max.x) / 2;




            int y = (Min.y + Max.y) / 2;


            return new Vector2Int(x, y);

        }



        public Room GetRoomInSector(float tileSize)
        {
            float innerRadiusSq = InnerRadius * InnerRadius;
            float outerRadiusSq = OuterRadius * OuterRadius;
            Room room = new Room();
            randCOlor = Random.ColorHSV();

            List<Vector2> points = new List<Vector2>() { Point1, Point2, Point3, Point4 };
            float minx = points[0].x;
            float miny = points[0].y;
            float maxx = points[0].x;
            float maxy = points[0].y;

            for (int i = 0; i < points.Count; i++)
            {

                if (points[i].x < minx)
                {
                    minx = points[i].x;
                }
                if (points[i].y < miny)
                {
                    miny = points[i].y;
                }
                if (points[i].x > maxx)
                {
                    maxx = points[i].x;
                }
                if (points[i].y > maxy)
                {
                    maxy = points[i].y;
                }
            }
            Min = new Vector2Int(Mathf.CeilToInt(minx), Mathf.CeilToInt(miny));
            Max = new Vector2Int(Mathf.CeilToInt(maxx), Mathf.CeilToInt(maxy));




            for (int i = Min.x; i < Max.x; i += (int)tileSize)
            {
                List<Vector2> tilesInRow = new List<Vector2>();
                for (int j = Min.y; j < Max.y; j += (int)tileSize)
                {


                    if (i * i + j * j > innerRadiusSq && i * i + j * j < outerRadiusSq)
                    {


                        float angle = Mathf.Atan2((j), (i)) * Mathf.Rad2Deg;

                        if (j < 0)
                            angle = 360 - Mathf.Abs(angle);

                        if (angle > AngleStart && angle < AngleEnd)
                        {


                            tilesInRow.Add(new Vector2(i, j));

                        }
                    }

                }
                if (tilesInRow.Count != 0)
                    room.tiles.Add(tilesInRow);
            }


            if (room.tiles.Count > 1)
                return room;
            else
                return null;

            //  return room;

        }



        private void OnDrawGizmos()
        {
            Gizmos.color = randCOlor;

            Gizmos.DrawSphere(new Vector3(Point1.x, 0, Point1.y), 1);


            Gizmos.DrawSphere(new Vector3(Point4.x, 0, Point4.y), 1);
            Gizmos.DrawLine(new Vector3(Point1.x, 0, Point1.y), new Vector3(Point4.x, 0, Point4.y));
            Gizmos.DrawLine(new Vector3(Point3.x, 0, Point3.y), new Vector3(Point2.x, 0, Point2.y));



        }
    }
}