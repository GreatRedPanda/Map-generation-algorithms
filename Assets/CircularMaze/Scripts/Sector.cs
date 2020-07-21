using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CircularMaze
{



    public class Sector : MonoBehaviour
    {

        public int Group;

       // public int Row;
     

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


        public  Vector2Int Min;
        public Vector2Int Max;

        Vector2 center;
        public Room roomInSector;

        public Dictionary<Door,Sector> DoorsInSector = new Dictionary<Door, Sector>();
        public List<Sector> connectedSectors = new List<Sector>();
        float innerRadiusSq { get { return InnerRadius * InnerRadius; } }


        float outerRadiusSq

        {
            get
            { return OuterRadius * OuterRadius; }
        }

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

         
            int x = (Min.x + Max.x) / 2;




            int y = (Min.y + Max.y) / 2;


            // return new Vector2Int(x, y);

            return new Vector2Int((int)center.x, (int)center.y);
        }


        public void InitSector()
        {
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


            if (InnerRadius!=OuterRadius)
            {
                float centerAngle = (AngleEnd + AngleStart) / 2;
                float centerRadius = (InnerRadius + OuterRadius) / 2;
                center = new Vector2(
    Mathf.FloorToInt(centerRadius * Mathf.Cos(centerAngle * Mathf.Deg2Rad)),
    Mathf.FloorToInt(centerRadius * Mathf.Sin(centerAngle * Mathf.Deg2Rad))
      );

            //    center = Generator.floorToCellSize(center);
            }
            else

                center = Vector2.zero;
        }
       


        public Room GetRoom()
        {

            Room room = new Room();

            int startX = Min.x;
            int startY=Min.y;
            bool startNodeFound = false;
            for (int i = Min.x; i < Max.x; i ++)
            {
             
                for (int j = Min.y; j < Max.y; j++)
                {
                    if (PointInsideSector(i, j))
                    {

                        startX = i;
                        startY = j;
                        startNodeFound = true;
                        break;
                    }

                }
                if (startNodeFound)
                    break;
            }

            Vector2 node = new Vector2(startX, startY);

            List<Vector2> closedPoints = new List<Vector2>();


            List<Vector2> openPoints = new List<Vector2>();
            openPoints.Add(node);
            


                do
                {


                Vector2 current = openPoints[0];
                closedPoints.Add(current);           
                openPoints.RemoveAt(0);

                List<Vector2> neighbours = new List<Vector2>();

                neighbours.Add(current + Vector2.right );
                neighbours.Add(current - Vector2.right );
                neighbours.Add(current + Vector2.up );
                neighbours.Add(current - Vector2.up );


                neighbours.Add(current + Vector2.one );
                neighbours.Add(current - Vector2.one );
                neighbours.Add(current + new Vector2(1, -1) );
                neighbours.Add(current - new Vector2(1, -1) );


                int flag = 0;
                foreach (var n in neighbours)
                {
                    
                    if (PointInsideSector((int)n.x, (int)n.y))
                    {

                        if (openPoints.FindIndex(x => x.x == n.x && x.y == n.y) == -1 
                            && closedPoints.FindIndex(x => x.x == n.x && x.y == n.y) == -1)
                        {
                            openPoints.Add(n);
                        }
                    }
                    else
                        flag++;
                }
                if (PointInsideSector((int)current.x, (int)current.y))
                {

                    if (flag == 0)
                        room.innerTiles.Add(new Vector3Int((int)current.x, (int)current.y,0));
                    else
                        room.outerTile.Add(new Vector3Int((int)current.x, (int)current.y, 0));
                        

                            
                }




            }
            while (openPoints.Count != 0);

                return room;

        }



        public int NeighboursNotInSector(int x, int y)
        {
            Vector2 current = new Vector2(x, y) ;
            List<Vector2> neighbours = new List<Vector2>();

            neighbours.Add(current + Vector2.right );
            neighbours.Add(current - Vector2.right );
            neighbours.Add(current + Vector2.up );
            neighbours.Add(current - Vector2.up );


            neighbours.Add(current + Vector2.one );
            neighbours.Add(current - Vector2.one );
            neighbours.Add(current + new Vector2(1, -1) );
            neighbours.Add(current - new Vector2(1, -1) );


            int flag = 0;
            foreach (var n in neighbours)
            {

                if (!PointInsideSector((int)n.x, (int)n.y))
                    flag++;
            }

            return flag;
        }



        public    bool PointInsideSector(int x, int y)
        {
            bool result = false;

            bool inRadius = false;

            if (innerRadiusSq == outerRadiusSq)
            {
                inRadius =  x * x + y * y <= outerRadiusSq;

            }
            else
            {

                inRadius = x * x + y * y >= innerRadiusSq && x * x + y * y <= outerRadiusSq;
            }

            if (inRadius)
            {


                if (AngleStart == 0 && AngleEnd == 360)
                {

                    result = true;

                }
                else
                {
                    float angle = Mathf.Atan2((y), (x)) * Mathf.Rad2Deg;
                   if (y < 0)
                        angle = 360 - Mathf.Abs(angle);
                    if (AngleStart > 360 || AngleEnd > 360)
                    {
                        if (angle > 270)
                            angle -= 360;

                        if (angle > AngleStart-360 && angle < AngleEnd-360)
                        {

                            result = true;

                        }
                    }
                    else
                    if (angle > AngleStart && angle < AngleEnd)
                    {

                        result = true;

                    }
                }
            }

            return result;
        }

    }
}