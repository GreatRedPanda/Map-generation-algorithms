using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
public class InteractableTilemap : MonoBehaviour
{
    protected Tilemap tilemap;

    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public virtual void Interact(Vector3 position)
    {



    }


    protected Vector3Int GetLocalTile(Vector3 position)
    {


        Vector3 parentScale = transform.parent.localScale;
        Vector3 tileSize = tilemap.cellSize;

        return new Vector3Int((int)(position.x*parentScale.x*tileSize.x), (int)(position.y * parentScale.y * tileSize.y),0);
    }
}
