using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Edge
{

    public Vector2 Start;
    public Vector2 End;


    public Vector2 Middle()
    {


        return (Start+End)/ 2;
    }


   
}
public    class Polygon
    {

    //public List<Vector2> Vertecies = new List<Vector2>();

    public List<Edge> Edges = new List<Edge>();


    public Polygon LeftPolygon;

    public Polygon RightPolygon;


    public void GetRandomEdges(out Edge edge1, out Edge edge2)
    {
        int index = Random.Range(0, Edges.Count - 1);
        Edge e1 = Edges[index];

        List<Edge> edges = new List<Edge>(Edges);
        int index1ToRemove = index - 1;
        int index2ToRemove = index + 1;


        if (index1ToRemove == -1)
            index1ToRemove = Edges.Count - 1;

        if (index2ToRemove == Edges.Count)
            index2ToRemove = 0;

        Edge e1Rem = edges[index1ToRemove];
        Edge e2Rem= edges[index2ToRemove];
        edges.Remove(e1);
        edges.Remove(e1Rem);
        edges.Remove(e2Rem);
        int index2 = Random.Range(0, edges.Count - 1);
        Edge e2 = edges[index2];
        edge1 = e1; edge2 = e2;
    }

    public void GetRandomEdgesShort(out Edge edge1, out Edge edge2)
    {
      
       


        int middle = Edges.Count / 2;
    

        int randStep = 2;
        int index = 0;
        int index2 = Random.Range(2, middle) ;
        float min= Vector2.Distance(Edges[0].Middle(), Edges[randStep].Middle());
        for (int i = 1; i < Edges.Count; i++)
        {
            int ind = i;
            int ind2 = i + randStep;
            if (ind2 >= Edges.Count)
                ind2 = Mathf.Abs(Edges.Count - ind2);
            float dis = Vector2.Distance(Edges[ind].Middle(), Edges[ind2].Middle());

            if (dis < min)
            {

                min = dis;
                index = ind;
                index2 = ind2;
            }
        }
        Edge e1 = Edges[index];

        Edge e2 = Edges[index2];

        edge1 = e1; edge2 = e2;
    }


    public void DividePolygon(Edge edge1, Vector2 pE1, Edge edge2, Vector2 pE2)
    {


      
        Edge e1DivR = new Edge() { Start = edge1.Start, End = pE1 };
        Edge e2DivR = new Edge() { Start = pE2, End = edge2.End };
        Edge eNew = new Edge() { Start = pE1, End = pE2 };


        Edge e1DivL = new Edge() { Start = pE1, End = edge1.End };
        Edge e2DivL = new Edge() { Start = edge2.Start, End = pE2 };
        Edge eNewL = new Edge() { Start = pE2, End = pE1 };


        Polygon polygon1 = new Polygon();
        Polygon polygon2 = new Polygon();


        int ind1 = Edges.IndexOf(edge1);
        int ind2 = Edges.IndexOf(edge2);

        int start = (ind1 > ind2) ? ind2 : ind1;
        int end= (ind1 > ind2) ? ind1 : ind2;


        List<Edge> edge1Neighbours = new List<Edge>();
        edge1Neighbours = Edges.Skip(start + 1).Take(end - start - 1).ToList();


        List<Edge> temp = Edges.Take(start).ToList();
        List<Edge> temp2 = Edges.Skip(end + 1).ToList();
        List<Edge> edge2Neighbours = temp.Concat(temp2).ToList();

        if (ind1 > ind2)
        {
            polygon1.Edges.Add(e1DivR);
            polygon1.Edges.Add(eNew);
            polygon1.Edges.Add(e2DivR);
            polygon1.Edges.AddRange(edge1Neighbours);


            polygon2.Edges.AddRange(temp);
            polygon2.Edges.Add(e2DivL);
             polygon2.Edges.Add(eNewL);
            polygon2.Edges.Add(e1DivL);
            polygon2.Edges.AddRange(temp2);

        }
        else
        {

            polygon1.Edges.AddRange(temp);
            polygon1.Edges.Add(e1DivR);
            polygon1.Edges.Add(eNew);
            polygon1.Edges.Add(e2DivR);
            polygon1.Edges.AddRange(temp2);

            polygon2.Edges.Add(e1DivL);
            polygon2.Edges.AddRange(edge1Neighbours);
            polygon2.Edges.Add(e2DivL);
            polygon2.Edges.Add(eNewL);
            
            

        }

        RightPolygon = polygon1;
        LeftPolygon = polygon2;
        
    }


    public bool IsEnoughSpace(float min)
    {

        //bool result = false;

        int middle = Edges.Count / 2;

        for (int i = 0; i < middle; i++)
        {
            float dis = Vector2.Distance(Edges[i].Middle(), Edges[i + middle].Middle());

            if (dis < min)
                return false;
        }
        if (middle % 2 == 1)
        {
            float dis = Vector2.Distance(Edges[Edges.Count-1].Middle(), Edges[middle-1].Middle());

            if (dis < min)
                return false;
        }

        return true;
    }
}

