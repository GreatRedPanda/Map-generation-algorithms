using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using TMPro;
using System.Globalization;

[System.Serializable]
public class ThreshholdData
{

    public Vector2  Threshhold;
    public TileBase Tile;
    public int Scale;
    public bool ForDecor;

    [HideInInspector]
    public List<Vector3Int> Positions=new List<Vector3Int>();
    [HideInInspector]
    public List<TileBase> Tiles=new List<TileBase>();

  
    public ThreshholdData()
    { }

    public ThreshholdData(ThreshholdData other)
    {
        Threshhold = other.Threshhold;
        Scale = other.Scale;
        Tile = other.Tile;
    }
}

public class LevelGenerator : MonoBehaviour
{
    public Grid TilemapsParentGrid;
    public Tilemap TilemapPref;
    //  public TileBase GroundTile;
    public int XSize = 500;
    public int YSize = 500;

    public ThreshholdData[] Thresholds;
    public Vector2 Scale;
    public Tilemap Base;
    public Tile BasetILE;

    public int SeedGen = 745456;
     float Seed=0.5f;

    public SpriteRenderer[] Decor;
    Tilemap[] tilemaps;

    public GameObject DecorParent;

    public TextMeshProUGUI CurrentValuesText;
    public TMP_InputField GenerationSeed;
    public TMP_InputField WidthText;
    public TMP_InputField HeightText;


    void Start()
    {
        setUp();
        SetCurrentGenerationValues();


        tilemaps = new Tilemap[Thresholds.Length];
        for (int i = 0; i < tilemaps.Length; i++)
        {
            Tilemap tm = Instantiate(TilemapPref);
            tm.transform.SetParent(TilemapsParentGrid.transform);
            tm.GetComponent<TilemapRenderer>().sortingOrder = i;
            tilemaps[i] = tm;
        }


      
    }

    void SetCurrentGenerationValues()
    {
        CurrentValuesText.text = $"Current values:\n {XSize} x {YSize}\n Seed {SeedGen}";
    }


    void ApplyParameters()
    {

      
        int tempSeed = 0;


        //try
        //{
        //    string number = GenerationSeed.text;


        //    tempSeed = System.Convert.ToInt32(number);
        //    Debug.Log(GenerationSeed.text + "  " + HeightText.text+"  "+tempSeed);
        //    SeedGen = tempSeed;

        //}

        //catch
        //{

        //}


        //CultureInfo provider;

        ////// If currency symbol is allowed, use en-US culture.
        ////if ((styles & NumberStyles.AllowCurrencySymbol) > 0)
        ////    provider = new CultureInfo("en-US");
        ////else
        //    provider = CultureInfo.InvariantCulture;
      
        Debug.Log(GenerationSeed.text);
        if (int.TryParse(GenerationSeed.text, NumberStyles.Number,CultureInfo.CurrentCulture.NumberFormat, out tempSeed))
        {
            SeedGen = tempSeed;
        }
        int tempX = 0;
        if (int.TryParse(WidthText.text, out tempX))
            XSize = tempX;

        int tempY = 0;
        if (int.TryParse(HeightText.text, out tempY))
            YSize = tempY;
        SetCurrentGenerationValues();



    }


    void setUp()
    {

        Base.ClearAllTiles();
        Base.size = new Vector3Int(XSize * 2 + 6, YSize * 2 + 6, 0);
        Base.origin = new Vector3Int(-2, -2, 0);
        Base.BoxFill(Base.origin, BasetILE, -2, -2, XSize * 2 + 6, YSize * 2 + 6);


        Random.InitState(SeedGen);
        Seed = Random.Range(0.4f, 1f);
    }
    int currentOffsetX = 0;
    int currentOffsetY = 0;


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.W))
        {

            Debug.Log(currentOffsetX+"  "+currentOffsetY);
            GenerateChunk(currentOffsetX, currentOffsetY, 10, 10);

            currentOffsetX += 10;

            if (currentOffsetX >= XSize)
            {
                currentOffsetX = 0;
                currentOffsetY += 10;
            }
        }
        Regenerate();


    }


    public void Generate()
    {
        StartCoroutine(generation());
    }


    bool inThegenProcess = false;
    IEnumerator generation()
    {
        bool generated = false;
        do
        {
            inThegenProcess = true;
            generated = mapFullyGenerated();
            Debug.Log(generated);
            inThegenProcess = false;
            yield return new WaitForSeconds(0.5f);

        }
        while (!generated);


    }


    public void GenerateStep()
    {
        StopAllCoroutines();
        if(!inThegenProcess)
        mapFullyGenerated();
        inThegenProcess = false;

    }

    bool mapFullyGenerated()
    {
        if ( currentOffsetY >= YSize)
            return true;
        GenerateChunk(currentOffsetX, currentOffsetY, 10, 10);

        currentOffsetX += 10;

        if (currentOffsetX >= XSize)
        {
            currentOffsetX = 0;
            currentOffsetY += 10;
        }

        return false;
    }
    public void SetXSize(string x)
    {
        Debug.Log(x);

    }
    public void SetYSize(string x)
    {


    }

    public void SetSeed(string seed)
    {

    }

    public void ShowlevelObjects()
    {

        DecorParent.SetActive(!DecorParent.activeSelf);
    }
    public void Clear()
    {
        currentOffsetX = 0;
        currentOffsetY = 0;


        for (int i = 0; i < Thresholds.Length; i++)
        {
            ThreshholdData th = Thresholds[i];

            th.Positions.Clear();
            th.Tiles.Clear();

            tilemaps[i].ClearAllTiles();
        }

        SpriteRenderer[] spriteRenderers = DecorParent.GetComponentsInChildren<SpriteRenderer>();

        foreach (var s in spriteRenderers)
        {
            Destroy(s.gameObject);
        }


        ApplyParameters();
        setUp();
    }



    void Regenerate()
    {

        if (Input.GetKeyDown(KeyCode.T))
        {
            Clear();
        }
    }
    float distanceSquared(float x, float y)
    {

        float dx = 2 * x / XSize - 1;
        float dy = 2 * y / YSize - 1;

        return dx * dx + dy * dy;
    }



    void makeDecor(List<Vector3Int> positionsForDcor, SpriteRenderer[] prefabsBase, int count)
    {

        List<Vector3Int> takenPoints = new List<Vector3Int>();
        for (int i = 0; i < count && positionsForDcor.Count > 0; i++)
        {


            int ind = Random.Range(0, positionsForDcor.Count);
            Vector3Int posInt = positionsForDcor[ind];

            int maxSize = 1;
            for (int k = 1; k < 3; k++)
            {
                if (positionsForDcor.Contains(posInt + Vector3Int.right * k) &&
                    !takenPoints.Contains(posInt + Vector3Int.right * k))
                {
                    maxSize++;
                }
                else
                    break;
            }

            List<SpriteRenderer> prefabs = prefabsBase.Where(x => (int)(x.sprite.bounds.size.x * 100f / 16f) + 1 <= maxSize).ToList();
            if (prefabs.Count != 0)
            {

                SpriteRenderer randPrefab = prefabs[Random.Range(0, prefabs.Count)];
                Vector3 pos = (Vector3)posInt * 0.16f;
                pos.z = pos.y / 10;

                SpriteRenderer levelObject = Instantiate(randPrefab);
                levelObject.transform.position = pos;
                levelObject.transform.SetParent(DecorParent.transform);
                for (int k2 = 0; k2 < (int)(levelObject.sprite.bounds.size.x * 100 / 16) + 1; k2++)
                {
                    takenPoints.Add(posInt + Vector3Int.right * k2);
                    positionsForDcor.Remove(posInt + Vector3Int.right * k2);
                }
            }
            else
                continue;

        }
    }



    void GenerateChunk(int offsetX, int offsetY, int width, int height)
    {
        List<ThreshholdData> tempThreshholds = new List<ThreshholdData>();
        foreach (var thresh in Thresholds)
        {
            ThreshholdData thd = new ThreshholdData(thresh);
            tempThreshholds.Add(thd);
        }

        
        for (int i = offsetX; i <=width+ offsetX; i++)
        {
            for (int j = offsetY; j <= height+ offsetY; j++)
            {
                float t = Mathf.PerlinNoise(i * Scale.x * Seed, j * Scale.y * Seed);
                if (t > (tempThreshholds[0].Threshhold.x + 0.4 * distanceSquared(i, j)))
                {
                    for (int x = i * tempThreshholds[0].Scale; x <= i * tempThreshholds[0].Scale + 1; x++)
                    {
                        for (int y = j * tempThreshholds[0].Scale; y <= j * tempThreshholds[0].Scale + 1; y++)
                        {
                            tilemaps[0].SetTile(new Vector3Int(x, y, 0), tempThreshholds[0].Tile);
                            tempThreshholds[0].Tiles.Add(tempThreshholds[0].Tile);
                            tempThreshholds[0].Positions.Add(new Vector3Int(x, y, 0));
                        }
                    }
                }
            }
        }

        List<Vector3Int> positionsToDecor = new List<Vector3Int>();
        for (int th = 1; th < tempThreshholds.Count; th++)
        {
            List<Vector3Int> positionsTOMapBase = tempThreshholds[th - 1].Positions.Where
                (
                x =>
                tilemaps[th - 1].GetTile(x + Vector3Int.down) != null
                && tilemaps[th - 1].GetTile(x + Vector3Int.up) != null
                && tilemaps[th - 1].GetTile(x + Vector3Int.left) != null
                && tilemaps[th - 1].GetTile(x + Vector3Int.right) != null
                && tilemaps[th - 1].GetTile(x + new Vector3Int(1, 1, 0)) != null
                && tilemaps[th - 1].GetTile(x + new Vector3Int(-1, -1, 0)) != null
                && tilemaps[th - 1].GetTile(x + new Vector3Int(1, -1, 0)) != null
                && tilemaps[th - 1].GetTile(x + new Vector3Int(-1, 1, 0)) != null
                ).ToList();


            positionsToDecor.AddRange(positionsTOMapBase.Where(x=>x.x/2< width + offsetX && x.y/2<height + offsetY));
            List<Vector3Int> positionsTOMap1 = positionsTOMapBase.Where
                (
                x => x.x % 2 == 0 && x.y % 2 == 0
                &&
                tilemaps[th - 1].GetTile(x + Vector3Int.up * 2) != null
                && tilemaps[th - 1].GetTile(x + Vector3Int.right * 2) != null
                && tilemaps[th - 1].GetTile(x + new Vector3Int(1, 1, 0) * 2) != null
                && tilemaps[th - 1].GetTile(x + new Vector3Int(1, -1, 0) * 2) != null
                ).ToList();
            foreach (var pos in positionsTOMap1)
            {

                int i = pos.x / 2 + pos.x % 2;
                int j = pos.y / 2 + pos.y % 2;
                float t = Mathf.PerlinNoise(i * Scale.x * Seed, j * Scale.y * Seed);
                if (t > (tempThreshholds[th].Threshhold.x + 0.4 * distanceSquared(i, j)))// + 0.4 * distanceSquared(i, j)
                {
                    for (int x = i * tempThreshholds[th].Scale; x <= i * tempThreshholds[th].Scale + 1; x++)
                    {
                        for (int y = j * tempThreshholds[th].Scale; y <= j * tempThreshholds[th].Scale + 1; y++)
                        {

                            tilemaps[th].SetTile(new Vector3Int(x, y, 0), tempThreshholds[th].Tile);
                            tempThreshholds[th].Tiles.Add(tempThreshholds[th].Tile);
                            tempThreshholds[th].Positions.Add(new Vector3Int(x, y, 0));
                            positionsToDecor.Remove(new Vector3Int(x, y, 0));
                        }
                    }
                }
            }


        }
        makeDecor(positionsToDecor, Decor, 100);

        for (int i = 0; i < tempThreshholds.Count; i++)
        {
            Thresholds[i].Positions.AddRange(tempThreshholds[i].Positions);
            Thresholds[i].Tiles.AddRange(tempThreshholds[i].Tiles);
        }


    }
}
