using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;



public enum LevelObjectOrientation { LeftOnly, RightOnly, All, DownOnly }
public class LevelObject : MonoBehaviour
{
    public  SpriteRenderer SpriteRenderer;

    public SpriteRenderer[] Sprites;
    public int TileX;
    public int TileY;
    public int XSize;
    public int YSize;

    public bool HorSplit;
    public bool VertSplit;

    public int SortLayerDiff=1;
    public LevelObjectOrientation Orientation=LevelObjectOrientation.All;


    public void CalculatePrefabSize()
    {
        XSize = 0;
        YSize = 0;
        if (SpriteRenderer != null)
        {
         //   SpriteRenderer SpriteRenderer = GetComponent<SpriteRenderer>();
            Vector2 size = SpriteRenderer.sprite.bounds.size;
            XSize = (int)(size.x * 100 / TileX)+1;
            YSize = (int)(size.y * 100 / TileY)+1;
        }
        else if (Sprites != null && Sprites.Length != 0)
        {
            for (int i = 0; i < Sprites.Length; i++)
            {
                SpriteRenderer SpriteRenderer = Sprites[i];//.GetComponent<SpriteRenderer>();
                Vector2 size = SpriteRenderer.sprite.bounds.size;

                
                if(VertSplit || i==0)
                XSize += (int)(size.x * 100);
                if(HorSplit  || i==0)
                YSize += (int)(size.y * 100);
            }

            Debug.Log(XSize+"    "+YSize);
              XSize =XSize  / TileX+1;

              YSize = YSize  / TileY+1;
        }



    }
    public void SetShadows(Tilemap shadows, Vector3Int pos)
    {


        //if (SpriteRenderer != null)
        //{
        //    TileBase t = shadows.GetTile(pos);
          
        // //   if (t == null)
        //      SpriteRenderer.sortingOrder = shadows.GetComponent<TilemapRenderer>().sortingOrder + SortLayerDiff;
        //}
        //else if (Sprites != null && Sprites.Length != 0)
        //{
        //    for (int i = 0; i < Sprites.Length; i++)
        //    {
        //        TileBase t = shadows.GetTile(pos+Vector3Int.right*i);
        //    //    if (t == null)
        //        {
        //          //  Debug.Log("Shadows null");
        //            Sprites[i].sortingOrder = shadows.GetComponent<TilemapRenderer>().sortingOrder + SortLayerDiff;
        //        }
        //     //   else
        //        {
        //            //Debug.Log("Shadows not null");
        //        }
        //    }

        //}




    }



}
