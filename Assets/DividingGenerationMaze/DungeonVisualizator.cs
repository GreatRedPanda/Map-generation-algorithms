using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DungeonVisualizator : MonoBehaviour
{
    public GameObject CorridorPrefab;

    public GameObject RoomCorner;

    public GameObject RoomWall;

    public GameObject RoomCenter;


    public GameObject Entry;


    public GameObject Junction3;
    public GameObject Junction2;
    public GameObject Junction4;
    public GameObject Junction1;


    public Vector3 Size;

    List<AnchorConnector> connectors = new List<AnchorConnector>();
    public GameObject DecorationsParent;
    public void Generate(List<Room> rooms, List<Corridor> corridors, List<Junction> junctions)

    {
       // Vector3 size = RoomCenter.GetComponent<Renderer>().bounds.size;
       Vector3 size = Size;
        Debug.Log(size + "RoomCenterSize");
        foreach (var r in rooms)
        {
            List<AnchorConnector> anchors = new List<AnchorConnector>();
            for (int i = r.X; i <= r.X+r.W; i++)
            {
                for (int j = r.Y; j <= r.Y+r.H; j++)
                {
                 
                    int yRot = GetYRotation(r, i,j);
                    GameObject rPartPref=GetRoomPrefab(r,i,j);
                    GameObject rp = Instantiate(rPartPref);
                    anchors.AddRange( rp.GetComponentsInChildren<AnchorConnector>());
                    connectors.AddRange(rp.GetComponentsInChildren<AnchorConnector>());
                    rp.transform.position = new Vector3(i*size.x, 0, j * size.z);
                    rp.transform.rotation = Quaternion.Euler(rPartPref.transform.rotation.eulerAngles.x, yRot, rPartPref.transform.rotation.eulerAngles.z); ;

                }


            }
           GenerateRoomStuff(anchors);
        }


        foreach (var connector in connectors)
        {
            Destroy(connector.gameObject);
        }
        connectors.Clear();
        foreach (var c in corridors)
        {


            Junction j1 = c.Junctions[0];

            Junction j2 = c.Junctions[1];
            int yRot = 0;
            if (j1.X == j2.X) // vertical
            {

                int min = j1.Y; int max = j2.Y;

                if (min > max)
                {
                    min = j2.Y; max = j1.Y;
                }


                //for (float i = min+size.z/2; i < max ; i++)
                    for (int i = min +1 ; i < max; i++)
               {
                    GameObject rp = Instantiate(CorridorPrefab);
                    rp.transform.position = new Vector3(j1.X * size.x, 0, i * size.z);
                    rp.transform.rotation = Quaternion.Euler(CorridorPrefab.transform.rotation.eulerAngles.x, yRot, CorridorPrefab.transform.rotation.eulerAngles.z); ;

                }
            }
            else 
            {
                int min = j1.X; int max = j2.X;
                yRot = 90;
                if (min > max)
                {
                    min = j2.X; max = j1.X;
                }

                for (int i = min + 1; i < max; i++)
                {
                    GameObject rp = Instantiate(CorridorPrefab);
                    rp.transform.position = new Vector3(i * size.x, 0, j1.Y * size.z);
                    rp.transform.rotation = Quaternion.Euler(CorridorPrefab.transform.rotation.eulerAngles.x, yRot, CorridorPrefab.transform.rotation.eulerAngles.z); ;

                }
            }
        }


        foreach (var j in junctions)
        {

            if (j.Inputs.Find(x => x is Room) != null)
                continue;

            int yRot = 0;
            GameObject rPartPref = Junction4;
            List<Side> sides = j.GetOrientation();
            if (sides.Count == 1)
            {
                rPartPref = Junction1;

                if (sides.Contains(Side.MIN_X))
                    yRot = 270;
                else if (sides.Contains(Side.MIN_Y))
                    yRot = 180;
                else if (sides.Contains(Side.PLUS_X))
                    yRot = 90;
                else if (sides.Contains(Side.PLUS_Y))
                    yRot = 0;
            }
            else
                if (sides.Count == 2)
            {
                rPartPref = Junction2;



                if (sides.Contains(Side.PLUS_X) && sides.Contains(Side.PLUS_Y))
                    yRot = 270;
                else if (sides.Contains(Side.PLUS_X) && sides.Contains(Side.MIN_Y))
                    yRot = 90;
                else if (sides.Contains(Side.MIN_X) && sides.Contains(Side.PLUS_Y))
                    yRot = 0;
                else if (sides.Contains(Side.MIN_X) && sides.Contains(Side.MIN_Y))
                    yRot = 180;

            }
            else
                if (sides.Count == 3)
            {
                rPartPref = Junction3;

                if (!sides.Contains(Side.MIN_X))
                    yRot = 0 ;
                else if (!sides.Contains(Side.MIN_Y))
                    yRot = -90;
                else if (!sides.Contains(Side.PLUS_X))
                    yRot = 180;
                else if (!sides.Contains(Side.PLUS_Y))
                    yRot = 90;
            }
            
            GameObject rp = Instantiate(rPartPref);
            rp.transform.position = new Vector3((j.X) * size.x, 0, (j.Y) * size.z);
            rp.transform.rotation = Quaternion.Euler(rPartPref.transform.rotation.eulerAngles.x, yRot, rPartPref.transform.rotation.eulerAngles.z); ;

        }
    }

    int GetYRotation(Room r, int i, int j)
    {

        int yRot = 0;

     
        if ((i == r.X || i == r.X + r.W) && !(j == r.Y || j == r.Y + r.H)) // у стены
        {

            if (i == r.X)
                yRot = 90;
            else if (i == r.X + r.W)
                yRot = 270;
        }
        else if (!(i == r.X || i == r.X + r.W) && (j == r.Y || j == r.Y + r.H)) // у стены
        {


            if (j == r.Y)
                yRot = 0;
            else if (j == r.Y + r.H)
                yRot = 180;

        }
        else if (i == r.X && j == r.Y
            || i == r.X && j == r.Y + r.H
            || i == r.X + r.W && j == r.Y
            || i == r.X + r.W && j == r.Y + r.H)  // углы
        {

            if (i == r.X + r.W && j == r.Y + r.H)
                yRot = 180;
            else
                if (i == r.X + r.W && j == r.Y)
                yRot = 270;
            if (i == r.X && j == r.Y)
                yRot = 0;
            else
               if (i == r.X && j == r.Y + r.H)
                yRot = 90;
        }
        else // центр
        {

        }
        return yRot;

    }
    GameObject GetRoomPrefab(Room r, int i, int j)
    {

        GameObject rPartPref;
        if (r.Junctions.Find(x => x.X == i && x.Y == j) != null)
            rPartPref = Entry;
        else
        if ((i == r.X || i == r.X + r.W) && !(j == r.Y || j == r.Y + r.H)) // у стены
        {
            rPartPref = RoomWall;
        }
        else if (!(i == r.X || i == r.X + r.W) && (j == r.Y || j == r.Y + r.H)) // у стены
        {
            rPartPref = RoomWall;



        }
        else if (i == r.X && j == r.Y
            || i == r.X && j == r.Y + r.H
            || i == r.X + r.W && j == r.Y
            || i == r.X + r.W && j == r.Y + r.H)  // углы
        {
            rPartPref = RoomCorner;
        }

        else // центр
        {
            rPartPref = RoomCenter;

        }
        return rPartPref;
    }


    void GenerateRoomStuff(List<AnchorConnector> anchors)
    {

        if (anchors == null || anchors.Count == 0)
            return;
        foreach (var a in anchors)
        {

            if (a.PossibleConnections.Length == 0)
                continue;
            GameObject go = a.PossibleConnections[Random.Range(0, a.PossibleConnections.Length)];
            if (go == null)
                continue;
            GameObject newModule = Instantiate(go);
            newModule.transform.SetParent(DecorationsParent.transform);
            AnchorConnector newAnchor = newModule.GetComponentInChildren<AnchorConnector>();
            connectors.AddRange(newModule.GetComponentsInChildren<AnchorConnector>());
            MatchExits(a, newAnchor);
        }
    }
    protected void MatchExits(AnchorConnector oldExit, AnchorConnector newExit)
    {


        var newModule = newExit.transform.parent;
        var forwardVectorToMatch = -oldExit.transform.forward;
        var correctiveRotation = Azimuth(forwardVectorToMatch) - Azimuth(newExit.transform.forward);
        newModule.RotateAround(newExit.transform.position, Vector3.up, correctiveRotation);
        var correctiveTranslation = oldExit.transform.position - newExit.transform.position;
        newModule.transform.position += correctiveTranslation;
    }


    protected static float Azimuth(Vector3 vector)
    {
        return Vector3.Angle(Vector3.forward, vector) * Mathf.Sign(vector.x);
    }

}
