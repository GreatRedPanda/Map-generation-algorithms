using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TreeTile : Tile
{
    public Sprite LeftUp;
    public Sprite RightUp;

    public Sprite LeftBot;
    public Sprite RightBot;

    static int YPos;
    static int XPos;


    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
       

        Vector3Int down = new Vector3Int(position.x, position.y - 1, position.z);
        Vector3Int up = new Vector3Int(position.x, position.y + 1, position.z);
        Vector3Int r = new Vector3Int(position.x + 1, position.y , position.z);
        Vector3Int l = new Vector3Int(position.x - 1, position.y, position.z);

       // if (!(tilemap.GetTile(l) is TreeTile)
       //    && !(tilemap.GetTile(up) is TreeTile)
       //     || !(tilemap.GetTile(down) is TreeTile)
       //       && !(tilemap.GetTile(r) is TreeTile)
       //     )
       // {
       //     XPos = position.x;
       //     YPos = position.y;


       //     sprite = null;
       // }
       //else
        {

            if (position.y % 2 == 0)
            {

                if (position.x % 2 == 0)
                {
                    sprite = LeftBot;

                }
                else
                {
                    sprite = RightBot;

                }
            }
            else
            {
                if (position.x % 2 == 0)
                {

                    sprite = LeftUp;
                }
                else
                {
                    sprite = RightUp;

                }
            }
            //if ((tilemap.GetTile(r) is TreeTile))
            //{

            //}
            //if ((tilemap.GetTile(r) is TreeTile))
            //{

            //}

        }

    

        base.RefreshTile(position, tilemap);

    }

#if UNITY_EDITOR

    [MenuItem("Assets/Create/Tiles/TreeTile")]
    public static void CreateTile()
    {


        string path = EditorUtility.SaveFilePanelInProject("Save Tree Tile", "New 4х4 tile", "asset", "Tile saved", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TreeTile>(), path);

    }
#endif

}
