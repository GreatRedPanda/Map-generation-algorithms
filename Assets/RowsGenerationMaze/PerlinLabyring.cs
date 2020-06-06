using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinLabyring : MonoBehaviour
{

    public float OffsetX = 0;
    public float OffsetY;

    public float MulttX = 1;
    public float MultY = 1;
    public int Width;
    public int Height;
    float[,] mapData;
    int[,] islandData;

    // Start is called before the first frame update
    void Start()
    {
        islandData = new int[Width, Height];
        mapData = new float[Width, Height];

        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {

                mapData[i, j] = Mathf.PerlinNoise((float)i, (float)j);
                //if (mapData[i, j] > 0.3f + 0.4 * distanceSquared(i, j))
                  //  islandData[i, j] = 1;
            }
        }
    }

    float distanceSquared(float x, float y)
    {

        float dx = 2 * x / Width - 1;
        float dy = 2 * y / Height - 1;
        //# at this point 0 <= dx <= 1 and 0 <= dy <= 1
        return dx * dx + dy * dy;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        //if (mapData != null)
        {

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {

                    float noise = Mathf.PerlinNoise((float)i * MulttX, (float)j * MulttX);
                    if (noise > 0.3f )

                        Gizmos.color = Color.yellow;
                    else
                        Gizmos.color = Color.blue;

                    //Gizmos.color = Color.Lerp(Color.black, Color.white, noise);
                    Gizmos.DrawCube(new Vector3(i, noise, j) * 10, Vector3.one * 10);
                }
            }

        }
    }
}
