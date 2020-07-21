using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PolygonBasedGeneration :MonoBehaviour
{

    public GameObject Cube;
    Vector3 size;
    //List<Line> lines = new List<Line>();
    //List<Line> lines2 = new List<Line>();

    public float Radius = 60;

    public int MinDots = 4;
    public int MaxPoints = 10;

    List<Polygon> polygons = new List<Polygon>();
    List<Vector2> randPointTes = new List<Vector2>();
 Polygon root = new Polygon();
    private void Start()
    {
       
        GenMethod1(root);
        polygons.Add(root);


        size = Cube.GetComponent<Renderer>().bounds.size;
        StartCoroutine(generate());
    }

    IEnumerator generate()
    {

        int iterations = 0;

        List<Polygon> newPolygons = new List<Polygon>();
        newPolygons.Add(root);
        while (newPolygons.Count != 0)
        {

            Polygon current = newPolygons[0];
            newPolygons.Remove(current);

            Edge e1; Edge e2;
            current.GetRandomEdgesShort(out e1, out e2);
            Vector2 v1 = createRandomPoint(e1);
            Vector2 v2 = createRandomPoint(e2);
            current.DividePolygon(e1, v1, e2, v2);

            if (current.LeftPolygon.IsEnoughSpace(10))
            {
                polygons.Add(current.LeftPolygon);
                newPolygons.Add(current.LeftPolygon);
            }
            else
            {

                current.LeftPolygon = null;
            }


            if (current.RightPolygon.IsEnoughSpace(10))
            {
                polygons.Add(current.RightPolygon);
                newPolygons.Add(current.RightPolygon);
            }
            else
            {
                current.RightPolygon = null;
            }


           // yield return new WaitForSeconds(1f);
            iterations++;
          
            //if (iterations > 10)
            //    break;
        }
        yield return new WaitForSeconds(1f);
       StartCoroutine( SetCube());
    }


    IEnumerator SetCube()
    {
        //while (true)

        List<Polygon> leaves = new List<Polygon>(polygons);
          leaves.RemoveAll(x => x.LeftPolygon != null && x.RightPolygon != null);
        //Polygon rp = leaves[Random.Range(0, leaves.Count)];

        foreach (var rp in leaves)
        {



            Vector3 scaledSize = size / 10;
            float nextOffsetPercent = 0;
            for (int i = 0; i < rp.Edges.Count; i++)
            {

                Edge e = rp.Edges[i];
                float percent = 0.1f;
                float sizePercentX = scaledSize.magnitude / Vector2.Distance(e.Start, e.End);
//Debug.Log("cUDEB SIZE PERCETN" + sizePercentX);
                Vector2 lastCoord = Vector2.zero;

                Vector3 vector = e.End - e.Start;
                while (percent < 0.9f)
                {
                    Vector2 m = GetPointByPercent(e, percent);
                    m.x += scaledSize.x / 2;
                    m.y += scaledSize.y / 2;


                    GameObject cube = Instantiate(Cube);
                    cube.transform.forward = new Vector3(vector.x, 0, vector.y);
                    cube.transform.position = new Vector3(m.x, 10, m.y) * 10;

                    int k = -1;
                    if (vector.x > 0 && vector.x > vector.y)
                        k = 1;
                    cube.transform.position += -mult(cube.transform.right, size);
                    lastCoord = m;

                    m.x += scaledSize.x / 2;
                    m.y += scaledSize.y / 2;
                    float cubeercent = GetPercentByPoint(e, m);
                    percent += sizePercentX + 0.02f;

                }
                if (i < rp.Edges.Count - 1)
                {

                    Vector3 v = Vector3.Project(new Vector3(lastCoord.x, 10, lastCoord.y), new Vector3(vector.x, 10, vector.y));
                    projecttions.Add(v);
                    Debug.Log(v);
                    // projecttions.Add(Vector2.Perpendicular(lastCoord - (e.End - e.Start)));
                }
                //  yield return new WaitForSeconds(1f);

                
            }
break;

        }
    
            yield return new WaitForSeconds(5f);

        
    }
    List<Vector3> projecttions = new List<Vector3>();
    Vector3 mult(Vector3 a, Vector3 b)
    {

        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
    //void GenMethod2()
    //{


    //    int points = Random.Range(MinDots, MaxPoints + 1);

    //    List<Vector2> randPoints = new List<Vector2>();


    //    for (int i = 0; i < points; i++)
    //    {

    //    }
    //}
    void GenMethod1(Polygon p)
    {

        int startAngle = Random.Range(0, 180);
        int angle = startAngle;

        int x1 = (int)(Radius * Mathf.Cos(angle * Mathf.Deg2Rad));
        int y1 = (int)(Radius * Mathf.Sin(angle * Mathf.Deg2Rad));
        

        while ( (angle - startAngle) / 360 < 1)
        {
            int addAngle = Random.Range(50, 91);


            int newAngle = angle + addAngle;

            if ((newAngle-startAngle) / 360 >= 1)
            {
                break;
            }
            
            else
            {
                angle = newAngle;
                int x = (int)(Radius * Mathf.Cos(angle * Mathf.Deg2Rad));
                int y = (int)(Radius * Mathf.Sin(angle * Mathf.Deg2Rad));

                if (p.Edges.Count == 0)
                    p.Edges.Add(new Edge() { Start = new Vector2(x1, y1), End = new Vector2(x,y) });
                
              
                else
                    p.Edges.Add(new Edge() { Start = p.Edges[p.Edges.Count-1].End, End = new Vector2(x, y) });

              
            }
        }

        p.Edges.Add(new Edge() { Start = p.Edges[p.Edges.Count - 1].End, End = p.Edges[0].Start });
  
    }


    Vector2 createRandomPoint(Edge e)
    {

        float t = Random.Range(0.3f, 0.7f);

        return GetPointByPercent(e,t);
       // randPointTes.Add(new Vector2(x, y));
    }


    Vector2 GetPointByPercent(Edge e, float percent)
    {
        float x = (1 - percent) * e.Start.x + percent * e.End.x;

        float y = (1 - percent) * e.Start.y + percent * e.End.y;
        return new Vector2(x, y);

    }

    float GetPercentByPoint(Edge e, Vector2 point)
    {
        float t1 = (e.Start.x -point.x)/(e.Start.x+e.End.x);

        float t2 = (e.Start.y - point.y) / (e.Start.y + e.End.y);

        Debug.Log("Проценты равны"+ t1+"    "+t2);
        return t1;

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        int c = 10;
        foreach (var p in polygons)
        {
            //Gizmos.color = Random.ColorHSV();
            foreach (var l in p.Edges)
            {
                Gizmos.DrawLine(new Vector3(l.Start.x,c,l.Start.y) * 10, new Vector3(l.End.x, c, l.End.y) * 10);
            }
            //c+=10;
        }
        Gizmos.color = Color.red;
        //foreach (var l in root.RightPolygon.Edges)
        //{
        //    Gizmos.DrawLine(l.Start * 10, l.End * 10);
        //}
        foreach (var p in projecttions)
        {
            Gizmos.DrawSphere(p * 10, 2f);
        }

    }

}