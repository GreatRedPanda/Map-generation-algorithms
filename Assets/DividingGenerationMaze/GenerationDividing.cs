using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum DivisionType { HOR = 1, VERT = 0, NONE = -1 }

public enum NodeType { LEFT, RIGHT }

public enum Side {  MIN_X, PLUS_X, MIN_Y, PLUS_Y}
public class Node

{

    public Node(Node parent)
    {
        Parent = parent;
        if (parent != null)
            Level = parent.Level + 1;
    }

    public int Level;
    public int StartX;
    public int StartY;

    public int EndX;

    public int EndY;


    public Node LeftNode;

    public Node RightNode;

    public Node Parent;

    public Line DivisionLine;
    public Node SisterNode
    {
        get
        {

            if (this == Parent.RightNode)
                return Parent.LeftNode;
            else
                return Parent.RightNode;
        }
    }



    public NodeType GetLeftOrRight()
    {


        if (this == Parent.RightNode)
            return NodeType.RIGHT;
        else
            return NodeType.LEFT;




    }
}


public interface IConnected {
     List<Junction> Junctions { get; set; }
}
public class Junction
{


    public int X;
    public int Y;

    public List<Line> Connections = new List<Line>();

    public List<IConnected> Inputs = new List<IConnected>();

    public int GetDistance(Junction other)
    {

        Vector2 v1 = new Vector2(X, Y);
        Vector2 v2 = new Vector2(other.X, other.Y);

        return (int)Vector2.Distance(v1, v2);
    }
    public List<Side> GetOrientation()
    {

        List<Side> sidesIncluded = new List<Side>();
        foreach (var l in Connections)
        {
            int index = l.JunctionsOnLine.IndexOf(this);
            int indLeft = index - 1;
            int indexRight = index + 1;

            if (indLeft > -1)
            {
                Junction j2 = l.JunctionsOnLine[indLeft];
                sidesIncluded.Add(getDirection(j2));             
            }


            if (indexRight < l.JunctionsOnLine.Count)
            {
                Junction j2 = l.JunctionsOnLine[indexRight];
                sidesIncluded.Add(getDirection(j2));

            }
        }

        return sidesIncluded;
    }

    Side getDirection(Junction j2)
    {

        if (X == j2.X)
        {
            if (Y < j2.Y)
                return Side.PLUS_Y;
            else
                return Side.MIN_Y ;
        }
        else 
        {
            if (X < j2.X)
                return Side.PLUS_X;
            else
                return Side.MIN_X;
        }
    }
}

public class Line
{
    public int X1;
    public int Y1;

    public int X2;
    public int Y2;

    public DivisionType Type;
    public List<Junction> JunctionsOnLine = new List<Junction>();


    public bool PointOnLine(Line line2)
    {

        bool belongs = false;

        if (line2.Type == DivisionType.VERT && Type == DivisionType.HOR)
        {
            if (line2.X1 >= X1 && line2.X1 <= X2)
                if (Y1 == line2.Y1 || Y1 == line2.Y2 || Y2 == line2.Y1 || Y2 == line2.Y2)
                {

                    belongs = true;
                }
        }
        if (line2.Type == DivisionType.HOR && Type == DivisionType.VERT)
        {
            if (line2.Y1 >= Y1 && line2.Y1 <= Y2)
                if (X1 == line2.X1 || X1 == line2.X2 || X2 == line2.X1 || X2 == line2.X2)
                {

                    belongs = true;
                }
        }
        return belongs;
    }
}
public class Corridor:IConnected
{
    public Color Color;
    public Vector3 Middle;
    public List<Vector3> Points;



    public List<Junction> Junctions { get; set; } = new List<Junction>();


}



public class Room: IConnected
{

    public int X;

    public int Y;

    public int W;

    public int H;

    public int MaxY { get { return Y + H; } }
    public int MaxX { get { return X + W; } }
    public Vector3 Center { get { return new Vector3(X + W / 2f, 1, Y + H / 2f); } }
    public List<Junction> Junctions { get; set; } = new List<Junction>();
    public Vector3 Size { get { return new Vector3(W, 1, H); } }


  public  bool FindPoint(Junction j)
    {
        if (j.X >= X && j.X<=MaxX && j.Y >= Y && j.Y<=MaxY)
        return true;

        return false;
    }

}



public class GenerationDividing : MonoBehaviour
{
   
    int[,] map;

    public int Width;
    public int Height;

    public int MinSize;
    public int MaxSize;

    Node root;

    List<Node> leaves = new List<Node>();

    List<Corridor> corridors = new List<Corridor>();


    List<Line> lines = new List<Line>();

    List<Junction> junctions = new List<Junction>();


    List<Room> rooms = new List<Room>();

    public int offset = 0;

    void Start()
    {
        List<Node> newNodes = new List<Node>();

        //MinSize += 2;

        root = new Node(null)
        {
            StartX = 0,
            EndX = Width,
            StartY = 0,
            EndY = Height
        };

        newNodes.Add(root);
        leaves.Add(root);
        StartCoroutine(Generate(newNodes));

    }

    IEnumerator Generate(List<Node> newNodes)
    {


        while (newNodes.Count != 0)
        {

            Node current = newNodes[0];
            newNodes.Remove(current);
            generate(current, newNodes);
            // yield return new WaitForSeconds(0.5f);


        }

        leaves.RemoveAll(x => x.LeftNode != null && x.RightNode != null);

        List<Node> connectedNodes = new List<Node>();
        List<Node> waiting = new List<Node>(leaves);

        RemoveDuplicateJunctions();


        orderJunctions();

        while (waiting.Count != 0 && waiting.Count != 1)
        {
            Node current = waiting[0];
           // yield return new WaitForSeconds(1.2f);
            if (current == root)
                break;
            if (waiting.Contains(current.SisterNode))
            {
                waiting.Remove(current);
                waiting.Remove(current.SisterNode);

                Junction j = new Junction();
                Junction j2 = new Junction();
                Junction j3 = new Junction();


                connectRegions(current.DivisionLine, out j, out j2, out j3);

                if (current.LeftNode == null && current.RightNode == null)
                    makeRoom(current, j);
                if (current.SisterNode.LeftNode == null && current.SisterNode.RightNode == null)
                    makeRoom(current.SisterNode, j3);
                waiting.Add(current.Parent);
            }
            else
            {

                waiting.Remove(current);
                waiting.Add(current);

            }


        }

        RemoveDuplicateJunctions();
        unconnectedRegions();
        connectRoomAndJunctions();
        RemoveDuplicateJunctions();

        makeCorridors();
        RemoveDeadEnds();      
         FindObjectOfType<DungeonVisualizator>().Generate(rooms, corridors, junctions);
        yield return new WaitForSeconds(0.2f);
    }


    void CutRooms()
    {

        foreach (var r in rooms)
        {

            int minX = r.X;
            int minY = r.Y;
            int maxX = r.MaxX;
            int maxY = r.MaxY;
            int minXCut = r.X;
            int minYCut = r.Y;
            int maxXCut = r.MaxX;
            int maxYCut = r.MaxY;
            List<Junction> minxJ = r.Junctions.Where(x => x.X == minX).ToList();
            List<Junction> minyJ = r.Junctions.Where(x => x.Y == minY).ToList();
            List<Junction> maxxJ = r.Junctions.Where(x => x.X == maxX).ToList();
            List<Junction> maxyJ = r.Junctions.Where(x => x.Y == maxY).ToList();

            if (minxJ.Count == 0)
            {


            }
        }
    }
    void getGenerationArea(Line line, out int sX, out int eX, out int sY, out int eY)
    {
         sX = line.X1;
         eX = line.X2;
         sY = line.Y1;
         eY = line.Y2;

        if (line.JunctionsOnLine.Count > 2)
        {
            List<Junction> genbase = new List<Junction>(line.JunctionsOnLine);

            int randJunctionIndex = Random.Range(0, line.JunctionsOnLine.Count);


            for (int i = 1; i < genbase.Count; i++)
            {
                if (genbase[i].GetDistance(genbase[i - 1]) > 2)
                {
                    sX = genbase[i-1].X;
                    eX = genbase[i].X;
                    sY = genbase[i-1].Y;
                    eY = genbase[i].Y;
                    return;

                }
            }
            int neighbourIndex = 0;
            if (randJunctionIndex == 0)
                neighbourIndex = 1;
            else if (randJunctionIndex == line.JunctionsOnLine.Count-1)
                neighbourIndex = randJunctionIndex - 1;
            else
            {

                int left = (Random.Range(0, 1f) > 0.5f) ? 1 : -1;

                neighbourIndex = randJunctionIndex + left;
            }

            if (neighbourIndex < randJunctionIndex)
            {
                sX = line.JunctionsOnLine[neighbourIndex].X;
                eX = line.JunctionsOnLine[randJunctionIndex].X;
                sY = line.JunctionsOnLine[neighbourIndex].Y;
                eY = line.JunctionsOnLine[randJunctionIndex].Y;
            }
            else
            {
                eX = line.JunctionsOnLine[neighbourIndex].X;
                sX = line.JunctionsOnLine[randJunctionIndex].X;
                eY = line.JunctionsOnLine[neighbourIndex].Y;
                sY = line.JunctionsOnLine[randJunctionIndex].Y;
            }
        }

       

    }
    void connectRegions(Line line, out Junction j, out Junction j2, out Junction j3)
    {

        int sX = line.X1;
        int eX = line.X2;
        int sY = line.Y1;
        int eY = line.Y2;

        getGenerationArea(line, out sX, out eX, out sY, out eY);


        Debug.Log($"GENERATION AREA sx {sX}  sy {sY}  eX {eX}  eY {eY}");
        int x = sX; int y = sY;



        Line l = new Line();
        j = new Junction();
        j2 = new Junction();
        j3 = new Junction();
        offset++;
        if (sX == eX)
        {
      
            y = Random.Range(sY + offset, eY + 1 - offset);

            Debug.Log($"  {y} RANDOM RANGE:  sY + offset {sY + offset},  eY + 1 - offset { eY + 1 - offset} ");
            l.X1 = sX - 1;
            l.X2 = eX + 1;
            l.Y1 = y;
            l.Y2 = y;
            l.Type = DivisionType.HOR;
            j.X = l.X1; j.Y = y;
            j2.X = sX; j2.Y = y;
            j3.X = l.X2; j3.Y = y;
         
        }

        else if (sY == eY)
        {
            x = Random.Range(sX + offset, eX + 1 - offset);
            Debug.Log($"  {x} RANDOM RANGE:  sX + offset {sX + offset},  eX + 1 - offset { eX + 1 - offset} ");
            l.X1 = x;
            l.X2 = x;
            l.Y1 = sY - 1;
            l.Y2 = eY + 1;
            l.Type = DivisionType.VERT;
            j.X = x; j.Y = l.Y1;
            j2.X = x; j2.Y = sY;
            j3.X = x; j3.Y = l.Y2;

        }
        offset--;
        l.JunctionsOnLine.AddRange(new Junction[] { j, j2, j3 });
        line.JunctionsOnLine.Add(j2);
        orderJunctionsOnLine(line);
        j.Connections.Add(l);
        j2.Connections.Add(l);
        j2.Connections.Add(line);
        j3.Connections.Add(l);

        lines.Add(l);
        junctions.AddRange(new Junction[] { j, j2, j3 });

    }

    void unconnectedRegions()
    {

        foreach (var l in lines)
        {
            if (l.JunctionsOnLine.Count == 2)
            {
                Junction j = new Junction();
                Junction j2 = new Junction();
                Junction j3 = new Junction();
                connectRegions(l, out j, out j2, out j3);
            }

        }
    }
    void connectRoomAndJunctions()
    {
        foreach (var r in rooms)
        {
            List<Junction> junctionsConnectedToRoom = junctions.FindAll(x => !r.Junctions.Contains(x) && r.FindPoint(x));

            foreach (var j in junctionsConnectedToRoom)
            {
                j.Inputs.Add(r);
                r.Junctions.Add(j);
            }
        }


    }


    void makeCorridors()
    {
        orderJunctions();

        List<Junction> connectedJ = new List<Junction>();
        foreach (var j in junctions)
        {
            foreach (var l in j.Connections)
            {
                int index = l.JunctionsOnLine.IndexOf(j);
                int indLeft = index - 1;
                int indexRight = index + 1;

                if (indLeft > -1)
                {

                    Junction j2 = l.JunctionsOnLine[indLeft];

                    if (!connectedJ.Contains(j2))

                    {
                        Corridor cor = makeCorridor(j2, j);
                        j.Inputs.Add(cor);
                        j2.Inputs.Add(cor);
                        if (j2.Connections.Count == 1)

                            connectedJ.Add(j2);
                    }
                }


                if (indexRight < l.JunctionsOnLine.Count)
                {
                    Junction j2 = l.JunctionsOnLine[indexRight];

                    if (!connectedJ.Contains(j2))
                    {
                        Corridor cor = makeCorridor(j, j2);
                        j.Inputs.Add(cor);
                        j2.Inputs.Add(cor);

                        if (j2.Connections.Count == 1)

                            connectedJ.Add(j2);
                    }
                }
            }
            connectedJ.Add(j);
        }
    }


    void orderJunctions()
    {

        foreach (var l in lines)
        {
            orderJunctionsOnLine(l);
        }
    }
    void orderJunctionsOnLine(Line l)
    {
        if (l.X1 == l.X2)
        {

            l.JunctionsOnLine = l.JunctionsOnLine.OrderBy(x => x.Y).ToList();

        }

        else if (l.Y1 == l.Y2)
        {

            l.JunctionsOnLine = l.JunctionsOnLine.OrderBy(x => x.X).ToList();

        }
    }


    Corridor makeCorridor(Junction j1, Junction j2)
    {
        Corridor c = new Corridor();
        int x1 = j1.X;
        int y1 = j1.Y; int x2 = j2.X; int y2 = j2.Y;
        c.Points = new List<Vector3>() { new Vector3(x1, 0, y1), new Vector3(x2, 0, y2) };

        c.Color = Random.ColorHSV();
        c.Junctions.Add(j1); c.Junctions.Add(j2);
        corridors.Add(c);

        return c;
    }

    void makeRoom(Node n, Junction j)
    {

        int sX = n.StartX + offset;
        int sY = n.StartY + offset;

        int eX = n.EndX - offset;
        int eY = n.EndY - offset;


        Room r = new Room();



        int xS = sX;
        int yS = sY;
        int xE = eX;
        int yE = eY;

        r.X = xS; r.Y = yS; r.W = eX - sX; r.H = eY - sY; r.Junctions.Add(j);
        rooms.Add(r);

        j.Inputs.Add(r);
    }


    void generate(Node currentNode, List<Node> newNodes)
    {



        float horOrVert = Random.Range(0f, 1f);


        if (horOrVert < 0.5f)//hor
        {
            if (currentNode.StartY + MinSize >= currentNode.EndY - MinSize)
            {
              //  Debug.Log((currentNode.StartY + MinSize) + "      " + (currentNode.EndY - MinSize));
            }
            else
            {
                int y = Random.Range(currentNode.StartY + MinSize, currentNode.EndY - MinSize);

                int sX = currentNode.StartX; int eX = currentNode.EndX;
                int sY = currentNode.StartY; int eY = y;
                int sY2 = y; int eY2 = currentNode.EndY;




                Line l = new Line() { X1 = sX, Y1 = y, X2 = eX, Y2 = y, Type = DivisionType.HOR };
                Junction j = new Junction() { X = sX, Y = y };
                Junction j2 = new Junction() { X = eX, Y = y };
                j.Connections.Add(l);
                j2.Connections.Add(l);
                l.JunctionsOnLine.AddRange(new Junction[] { j, j2 });

                List<Line> intersectingLines = lines.Where(x => x.PointOnLine(l) == true).ToList();

                foreach (var li in intersectingLines)
                {
                    if (l.X1 == li.X1)
                    {
                        li.JunctionsOnLine.Add(j);
                        j.Connections.Add(li);
                    }
                    else if (l.X2 == li.X1)
                    {
                        li.JunctionsOnLine.Add(j2);
                        j2.Connections.Add(li);
                    }
                }
                lines.Add(l);
                junctions.AddRange(new Junction[] { j, j2 });
                currentNode.LeftNode = new Node(currentNode) { StartX = sX, EndX = eX, StartY = sY, EndY = eY, DivisionLine = l };
                currentNode.RightNode = new Node(currentNode) { StartX = sX, EndX = eX, StartY = sY2, EndY = eY2, DivisionLine = l };
                newNodes.Add(currentNode.LeftNode);
                newNodes.Add(currentNode.RightNode);

                leaves.Add(currentNode.LeftNode);
                leaves.Add(currentNode.RightNode);

                return;
            }
        }

      //  else //ver
        {
            if (currentNode.StartX + MinSize >= currentNode.EndX - MinSize)
            {
                Debug.Log((currentNode.StartX + MinSize) + "      " + (currentNode.EndX - MinSize));
            }
            else
            {
                int x = Random.Range(currentNode.StartX + MinSize, currentNode.EndX - MinSize);

                int sX = currentNode.StartX; int eX = x;

                int sX2 = x; int eX2 = currentNode.EndX;

                int sY = currentNode.StartY; int eY = currentNode.EndY;




                Line l = new Line() { X1 = x, Y1 = sY, X2 = x, Y2 = eY, Type = DivisionType.VERT };
                Junction j = new Junction() { X = x, Y = sY };
                Junction j2 = new Junction() { X = x, Y = eY };
                l.JunctionsOnLine.AddRange(new Junction[] { j, j2 });

                List<Line> intersectingLines = lines.Where(y => y.PointOnLine(l) == true).ToList();
                j.Connections.Add(l); j2.Connections.Add(l);
                foreach (var li in intersectingLines)
                {
                    if (l.Y1 == li.Y1)
                    {
                        li.JunctionsOnLine.Add(j);
                        j.Connections.Add(li);
                    }
                    else if (l.Y2 == li.Y1)
                    {
                        li.JunctionsOnLine.Add(j2);
                        j2.Connections.Add(li);
                    }
                }
                lines.Add(l); junctions.AddRange(new Junction[] { j, j2 });

                currentNode.LeftNode = new Node(currentNode) { StartX = sX, EndX = eX, StartY = sY, EndY = eY, DivisionLine = l };
                currentNode.RightNode = new Node(currentNode) { StartX = sX2, EndX = eX2, StartY = sY, EndY = eY, DivisionLine = l };

                newNodes.Add(currentNode.LeftNode);
                newNodes.Add(currentNode.RightNode);

                leaves.Add(currentNode.LeftNode);
                leaves.Add(currentNode.RightNode);





            }

        }


    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        int c = 0;
        //foreach (var n in leaves)
        //{
        //    //Gizmos.color =
        //    //Random.ColorHSV();

        //    if (n != null)
        //    {
        //        Gizmos.DrawLine(new Vector3(n.StartX, c, n.StartY) * 10f, new Vector3(n.StartX, c, n.EndY) * 10f);
        //        Gizmos.DrawLine(new Vector3(n.StartX, c, n.EndY) * 10f, new Vector3(n.EndX, c, n.EndY) * 10f);
        //        Gizmos.DrawLine(new Vector3(n.EndX, c, n.EndY) * 10f, new Vector3(n.EndX, c, n.StartY) * 10f);
        //        Gizmos.DrawLine(new Vector3(n.EndX, c, n.StartY) * 10f, new Vector3(n.StartX, c, n.StartY) * 10f);
        //    }
        //    // Gizmos.DrawCube(new Vector3(n.StartX + n.EndX / 2f, c, n.StartY + n.EndX / 2f) * 10, new Vector3(-n.StartX + n.EndX, 0.5f, -n.StartY + n.EndX) * 10f);
        //    // c+=2;
        //}

        foreach (var cor in corridors)
        {
            //Gizmos.color = cor.Color;
            if (cor.Points != null)
            {
                for (int i = 0; i < cor.Points.Count - 1; i++)
                {
                    Gizmos.DrawLine(cor.Points[i] * 10f, cor.Points[i + 1] * 10f);
                }
            }
        }


        //foreach (var l in lines)
        //{
        //    float y = l.JunctionsOnLine.Count * 10;
        //    if (l.JunctionsOnLine.Count == 1)
        //        Gizmos.color = Color.magenta;
        //    else if (l.JunctionsOnLine.Count == 2)
        //        Gizmos.color = Color.yellow;
        //    else if (l.JunctionsOnLine.Count == 3)
        //        Gizmos.color = Color.cyan;

        //    else if (l.JunctionsOnLine.Count == 4)
        //        Gizmos.color = Color.red;
        //    else if (l.JunctionsOnLine.Count == 0)
        //        Gizmos.color = Color.green;
        //    else if (l.JunctionsOnLine.Count == 5)
        //        Gizmos.color = Color.black;
        //    else
        //        Gizmos.color = Color.white;

        //    Gizmos.DrawLine(new Vector3(l.X1, 0, l.Y1) * 10, new Vector3(l.X2, 0, l.Y2) * 10);
        //}
        foreach (var r in rooms)
        {
            Gizmos.DrawCube(r.Center * 10, r.Size * 10);

        }
        foreach (var j in junctions)
        {

            if (j.Inputs.Count == 1)
                Gizmos.color = Color.magenta;
            else if (j.Inputs.Count == 2)
                Gizmos.color = Color.yellow;
            else if (j.Inputs.Count == 3)
                Gizmos.color = Color.cyan;

            else if (j.Inputs.Count == 4)
                Gizmos.color = Color.red;
            else if (j.Inputs.Count == 0)
                Gizmos.color = Color.green;
            else if (j.Inputs.Count == 5)
                Gizmos.color = Color.black;
            else
                Gizmos.color = Color.white;
            Gizmos.DrawSphere(new Vector3(j.X, 0, j.Y) * 10, 2);
        }
    }


    void RemoveDuplicateJunctions()
    {
        int removedJunctions = 0;
        List<Junction> junctionsCopy = new List<Junction>(junctions);
        while (junctionsCopy.Count != 0)
        {
            Junction j = junctionsCopy[0];
            Junction j2 = junctions.Find(x => x != j && x.X == j.X && x.Y == j.Y);

            if (j2 != null)
            {
                removedJunctions++;
                junctions.Remove(j2);

                //j.Connections.AddRange(j2.Connections.Where(x => j.Connections.Contains(x) == false));
               // j.Connections.AddRange(j2.Connections);

                foreach (var line in j2.Connections)
                {
                    if (!j.Connections.Contains(line))
                        j.Connections.Add(line);
                }
                foreach (var L in j2.Connections)
                {
                    L.JunctionsOnLine.Remove(j2);
                    if(!L.JunctionsOnLine.Contains(j))
                    L.JunctionsOnLine.Add(j);
                }
                junctionsCopy.Remove(j2);
            }
            junctionsCopy.Remove(j);
        }

        Debug.Log("УДАЛЕНО ДУБЛИКАТОВ"+removedJunctions);
    }


    void RemoveDeadEnds()
    {


        List<Junction> junctionsOne = junctions.FindAll(x => x.Inputs.Count == 1);


        foreach (var j in junctionsOne)
        {


            int index = j.Connections[0].JunctionsOnLine.IndexOf(j);

            int neighbourIndex = (index ==0)? 1:index-1;


            Junction neighbour = j.Connections[0].JunctionsOnLine[neighbourIndex];

            neighbour.Inputs.Remove(j.Inputs[0]);

            j.Connections[0].JunctionsOnLine.Remove(j);
            corridors.Remove(j.Inputs[0] as Corridor);
            junctions.Remove(j);
        }
    }
}
