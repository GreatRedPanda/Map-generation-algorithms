using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class TileWithDownWalls : Tile
{
    public Sprite LeftUpCorner;
    public Sprite RightUpCorner;
    public Sprite LeftBotCorner;
    public Sprite RightBotCorner;


    public Sprite LeftSide;
    public Sprite RightSide;
    public Sprite UpSide;
    public Sprite DownSide;

    public Sprite Filled;



    public TileBase WallTile;

    public TileBase TileAllowedToPlaceWall;

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector3Int neighbourPos = new Vector3Int(position.x + i, position.y + j, position.z);
                if (tilemap.GetTile(neighbourPos) is TileWithDownWalls)
                tilemap.RefreshTile(neighbourPos);
    }
        }
    }

    public override  void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        string data = "";
        for (int i = -1; i <=1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                Vector3Int neighb = position + new Vector3Int(j,i, 0);

                if (tilemap.GetTile(neighb) is TileWithDownWalls)
                {

                    data += "1";
                }
                else
                    data += "0";

              //  tilemap.RefreshTile(neighb);
            }
        }
        getSpritebyCode(data, ref  tileData, tilemap.GetComponent<Tilemap>(),  position);

    //    base.RefreshTile(position, tilemap);

    }



    void getSpritebyCode(string data, ref TileData tileData, Tilemap tilemap,Vector3Int position)
    {

        if (data[1] == '0' && data[3] == '0' && data[4] == '1' && data[6] == '1')
        {
            tileData.sprite = LeftBotCorner;
        }
        else if (data[1] == '0' && data[4] == '0' && data[6] == '1' && data[3] == '1')
        {
            tileData.sprite = RightBotCorner;
        }
        else if (data[4] == '0' && data[6] == '0' && data[1] == '1' && data[3] == '1')
        {
            tileData.sprite = RightUpCorner;
        }
        else if (data[3] == '0' && data[6] == '0' && data[1] == '1' && data[4] == '1')
        {
            tileData.sprite = LeftUpCorner;
        }

        else
             if (data[1] == '1' && data[3] == '1' && data[4] == '0' && data[6] == '1')
        {
            tileData.sprite = RightSide;
        }
        else if (data[1] == '1' && data[4] == '1' && data[6] == '1' && data[3] == '0')
        {
            tileData.sprite = LeftSide;
        }
        else if (data[4] == '1' && data[6] == '0' && data[1] == '1' && data[3] == '1')
        {
            tileData.sprite = UpSide;
        }
        else if (data[3] == '1' && data[6] == '1' && data[1] == '0' && data[4] == '1')
        {
            tileData.sprite = DownSide;
        }
        else if (data[3] == '1' && data[6] == '1' && data[1] == '1' && data[4] == '1')
            tileData.sprite = Filled;
        else
        {
           // tilemap.SetTile(position, null);


        }

    }
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {

        //Vector3Int down = new Vector3Int(position.x, position.y - 1, position.z);


        //if (!(tilemap.GetTile(down) is TileWithDownWalls))
        //{

        //    Tilemap t = tilemap.GetComponent<Tilemap>();
        //    for (int i = 0; i < 4; i++)
        //    {

        //        if (tilemap.GetTile(down + Vector3Int.down * i) == TileAllowedToPlaceWall || tilemap.GetTile(down + Vector3Int.down * i) == null)
        //            t.SetTile(down + Vector3Int.down * i, WallTile);
        //        else
        //            break;
        //    }


        //}
        return base.StartUp(position, tilemap, go);



    }

#if UNITY_EDITOR

    [MenuItem("Assets/Create/Tiles/TileWithDownWalls")]
    public static void CreateTile()
    {


        string path = EditorUtility.SaveFilePanelInProject("Save TileWithDownWalls", "New TileWithDownWalls", "asset", "Tile saved", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TileWithDownWalls>(), path);

    }
#endif

}
