using UnityEngine;
using System.Collections;
using CircularMaze;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
public class DoorTilemap : InteractableTilemap
{

public    List<Door> doors;
  

    public override void Interact(Vector3 position)
    {
        base.Interact(position);

        Vector3Int pos = tilemap.WorldToCell(position);
        
        float distance = doors[0].DistanceToFirstTile(pos);
        Door dRes = doors[0];

        foreach (var d in doors)
        {
        float distance2 =d.DistanceToFirstTile(pos);

            if (distance > distance2)
            {
                dRes = d;
                distance = distance2;
            }
        }

        Debug.Log(pos + "   " + position + "   " + dRes.DoorTiles[0]);


        foreach (var t in dRes.DoorTiles)
        {
            tilemap.SetTile(t, null);
            tilemap.RefreshTile(t);

        }
    }
}
