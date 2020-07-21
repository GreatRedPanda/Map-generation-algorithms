using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
public class FloorToWallTile : Tile
{

    [HideInInspector]
    public Tilemap Shadows;
    public Tile Shadow;
    public Tile WallSideTile;
  //  public static List<TileBase> AllWallTiles = new List<TileBase>();
 //   public static List<Vector3Int> DownWallTilesToDecor = new List<Vector3Int>();
 //   public static List<Vector3Int> LeftWallTilesToDecor = new List<Vector3Int>();

    public void InitTile(Tilemap shadows)
    {
        Shadows = shadows;

    }
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        Tilemap t = tilemap.GetComponent<Tilemap>();

        Vector3Int down = new Vector3Int(position.x, position.y - 1, position.z);



        //if (t.GetTile(down) != this)
        //{
        //    t.SetTile(down, WallSideTile);

        //    //if (t.GetTile(down+Vector3Int.down) is FloorWIthWall)
        //    //{
        //    //    DownWallTilesToDecor.Add(down);
        //    //}
        //}

        if (Shadows != null)
        {
            Shadows.SetTile(position, null);
            Shadows.SetTile(down, null);


            Vector3Int neighbourPos = new Vector3Int(position.x + 1, position.y, position.z);
            Vector3Int neighbourPosFar = new Vector3Int(position.x + 2, position.y, position.z);
            Vector3Int downCorner = new Vector3Int(position.x + 1, position.y-1, position.z);
            Vector3Int   ldCorner = new Vector3Int(position.x - 1, position.y - 1, position.z);

            if (t.GetTile(ldCorner) == this  && t.GetTile(down)==WallSideTile)
            {

                Shadows.SetTile(down, Shadow);
            }

            if (!(t.GetTile(neighbourPos) == this))
            {
                Shadows.SetTile(neighbourPos, Shadow);
            }
            else
            {
                Shadows.SetTile(neighbourPos, null);


            }

            if (!(t.GetTile(downCorner) == this) && !(t.GetTile(downCorner) == WallSideTile))
            {
                Shadows.SetTile(downCorner, Shadow);
            }
            Shadows.SetTile(neighbourPosFar, null);



        }

        return base.StartUp(position, tilemap, go);

    }

 




    bool HasHeighbour(Vector3Int position, ITilemap tilemap, out int index)
    {
        TileBase tile = tilemap.GetTile(position);
        index = -1;
 
        return tilemap.GetTile(position) !=null;
    }

    bool HasHeighbourOfOneType(Vector3Int position, ITilemap tilemap)
    {
        TileBase tile = tilemap.GetTile(position);
     
       
        return tilemap.GetTile(position) == this;
    }

#if UNITY_EDITOR

    [MenuItem("Assets/Create/Tiles/FloorToWallTile")]
    public static void CreateTile()
    {


        string path = EditorUtility.SaveFilePanelInProject("Save terrain tile", "New floor tile", "asset", "Tile saved", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<FloorToWallTile>(), path);

    }
#endif
}
