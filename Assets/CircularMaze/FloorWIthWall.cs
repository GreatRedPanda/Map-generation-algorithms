using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
public class FloorWIthWall : Tile
{

 

    public Sprite FloorSprite;

    public Sprite WallSprite;


   public Tile ShadowTile;

    public Tile LUCorner;
    public Tile UpMiddle;
    public Tile RUCorner;

    public Tile LMiddle;


    public Tile RMiddle;

    public Tile LDCorner;
    public Tile DownMiddle;
    public Tile RDCorner;

    void Tol()
    {


    }
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
      //  Tilemap t = tilemap.GetComponent<Tilemap>();

        int index = -1;
        Vector3Int up= new Vector3Int(position.x, position.y + 1, position.z);
        Vector3Int down = new Vector3Int(position.x, position.y- 1, position.z);


        //if (tilemap.GetTile(up) == null)
        //    this.sprite = WallSprite;
        //else
        //    this.sprite = FloorSprite;
       tilemap.RefreshTile(down);
        tilemap.RefreshTile(up);
        base.RefreshTile(position, tilemap);
  
        //for (int i = -1; i <= 1; i++)
        //{
        //    for (int j = -1; j <= 1; j++)
        //    {
        //        Vector3Int neighbourPos = new Vector3Int(position.x + i, position.y + j, position.z);
        //        int ind = -1;

        //        if (!(t.GetTile(neighbourPos) != null))
        //        {

        //            t.SetTile(neighbourPos, RMiddle);
        //        }


        //    }
        //}


    }
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {


        Vector3Int up = new Vector3Int(position.x, position.y + 1, position.z);

        if (tilemap.GetTile(up) == null)
        {
            tileData.sprite = WallSprite;
            tileData.colliderType = ColliderType.Grid;
           
        }
        else
        {
            tileData.sprite = FloorSprite;
            tileData.colliderType = ColliderType.None;

        }

    }
    Tile getSprite(int x, int y, int curX, int curY)
    {

        if (x < curX)
        {
            if (y < curY)
            {
                return LUCorner;
            }
            else if (y == curY)
            {

                return LMiddle;
            }
            else
            {
                return LDCorner;
            }
        }
        else if (x == curX)
        {
            if (y < curY)
            {
                return UpMiddle;
            }
            else if (y == curY)
            {
                return null;
            }
            else
            {
                return DownMiddle;
            }
        }
        else
        {
            if (y < curY)
            {
                return RUCorner;
            }
            else if (y == curY)
            {

                return RMiddle;
            }
            else
            {

                return RDCorner;
            }
        }
    }

    bool HasHeighbour(Vector3Int position, ITilemap tilemap, out int index)
    {
        TileBase tile = tilemap.GetTile(position);

        index = -1;
 
        return tile != null;
    }

    bool HasHeighbourOfOneType(Vector3Int position, Tilemap tilemap, out int index)
    {
        TileBase tile = tilemap.GetTile(position);
        index = -1;
        bool isType= tile == this;
        Debug.Log(position + "   " + isType +"   "+tile);

        return isType;
    }

#if UNITY_EDITOR

    [MenuItem("Assets/Create/Tiles/GroundTile")]
    public static void CreateTile()
    {


        string path = EditorUtility.SaveFilePanelInProject("Save terrain tile", "New 4х4 tile", "asset", "Tile saved", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<FloorWIthWall>(), path);

    }
#endif
}
