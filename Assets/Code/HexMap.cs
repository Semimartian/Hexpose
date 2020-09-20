using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour
{
    private static Hex[,] hexes;
    private static Hex[] realHexes;
    public readonly static int numberOfRows = 200;
    public readonly static int numberOfColumns = 200;
    private static bool mapWasGenerated;
    [SerializeField] private Collider mapZone;
    //[SerializeField] private CombineableMesh combineableMeshPreFab;
    [SerializeField] private Hex hexPreFab;
    [SerializeField] private HexBomb hexBombPreFab;
    [SerializeField] private int bombGenerationChance = 32;
    [SerializeField] private int hardHexGenerationChance = 14;

    [Header("Materials:")]
    public Material emptyHexMat;
    public Material highLightedHexMat;
    public Material awaitingFillHexMat;
    public Material hardHexMat;
    [SerializeField] private Material dynamicColourMat;

    [SerializeField] Material[] hexColouredMaterials;
    [SerializeField] private List<Material> dynamicHexColouredMaterials = new List<Material>();
    [SerializeField] private bool useDynamicColours = true;
    [SerializeField] private float DynamicColoursDifferenceThreshold = 0.1f;

    public static HexMap instance;
    private Vector3 mapCentre;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Tried to instantiate more than one map!");
            return;
        }

        GenerateMap();
    }

    public static Hex GetHex(int q, int r)
    {
        if (hexes == null)
        {
            Debug.LogError("hexes == null");          
        }
        else if (q >= 0 && q < numberOfColumns && r >= 0 && r < numberOfRows)
        {
            return hexes[q, r];
        }
        Debug.LogError("hex is null");
        return null;
    }

    public static Hex GetHex(HexCoordinates hexCoordinates)
    {
        int q = hexCoordinates.q;
        int r = hexCoordinates.r;
        if (hexes == null)
        {
            Debug.LogError("hexes == null");
        }
        else if (q >= 0 && q < numberOfColumns && r >= 0 && r < numberOfRows)
        {
            return hexes[q, r];
        }
        Debug.LogError("hex is null");
        return null;
    }

    private int GetRandomQ()
    {
       return Random.Range(0,numberOfColumns);
    }

    private int GetRandomR()
    {
        return Random.Range(0, numberOfRows);
    }

    private Hex GetRandomHex()
    {
        return GetHex(GetRandomQ(), GetRandomR());
    }

    public Material GetHexColouredMaterial(ushort index)
    {
        if (useDynamicColours)
        {
            return dynamicHexColouredMaterials[index];
        }
        else
        {
            return hexColouredMaterials[index];
        }

    }
    public void GenerateMap()
    {
        if (mapWasGenerated)
        {
            Debug.LogError("Someone's trying to regenerate the map!");
        }
        mapCentre = mapZone.transform.position;
        hexes = new Hex[numberOfColumns, numberOfRows];

        Bounds zoneBounds = mapZone.bounds;
        List<Hex> realHexesList = new List<Hex>(32);
        Wall[] walls = FindObjectsOfType<Wall>();
        
        for (int column = 0; column < numberOfColumns; column++)
        {
            for (int row = 0; row < numberOfRows; row++)
            {
                Vector3 hexPosition = Hex.PositionInWorld(column, row);
                bool hexIsInBounds = zoneBounds.Contains(hexPosition);
                /*    (hexPosition.x >= bounds.min.x &&
                     hexPosition.x <= bounds.max.x &&
                     hexPosition.z >= bounds.min.z &&
                     hexPosition.z <= bounds.max.z);*/
                if (hexIsInBounds)
                {
                    HexTypes modifiedType = HexTypes.Floor;
                    for (int i = 0; i < walls.Length; i++)
                    {
                        Bounds wallBounds = walls[i].collider.bounds;
                        if (wallBounds.Contains(hexPosition))
                        {
                            modifiedType = walls[i].hexType;
                            break;
                        }
                    }

                    //Find Mat
                    ushort bestMatIndex;
                    {
                        Color32 bestColour = GetBackgroundColourFromBounds(hexPosition, zoneBounds);
                        if (useDynamicColours)
                        {
                            ushort? index = FindClosestDynamicMaterial(bestColour);
                            if (index == null)
                            {
                                Material mat = new Material(dynamicColourMat);
                                mat.color = bestColour;
                                dynamicHexColouredMaterials.Add(mat);
                                bestMatIndex = (ushort)(dynamicHexColouredMaterials.Count - 1);
                            }
                            else
                            {
                                bestMatIndex = (ushort)index;
                            }

                        }
                        else
                        {
                            bestMatIndex = FindClosestMaterialIndex(bestColour);
                        }
                       
                    }

                    Hex hex = Instantiate(hexPreFab);
                    hex.Construct(column, row, hex.GetComponent<HexComponent>(), bestMatIndex);
                    hex.transform.position = hexPosition;
                    hexes[column, row] = hex;
                    realHexesList.Add(hex);

                    if (modifiedType != HexTypes.Floor)
                    {
                        if(modifiedType == HexTypes.Hard)
                        {
                            hex.Harden();
                        }
                        else if (modifiedType == HexTypes.Hexposed)
                        {
                            hex.ChangeState(HexStates.Full);

                        }
                    }
                    else
                    {
                        if (Random.Range(0, bombGenerationChance) == 0)
                        {
                            HexBomb hexBomb = Instantiate
                                (hexBombPreFab, hexPosition, Quaternion.identity);
                            hexBomb.hex = hex;

                        }
                        else if (Random.Range(0, hardHexGenerationChance) == 0)
                        {

                            hex.Harden();

                        }
                    }

                }

                //Material hexMaterial = GetMaterialForHex(hex);
                //hexComponent.GetComponentInChildren<MeshRenderer>().material = hexMaterial;
                //hexComponent.SetHex(hex);
               /* if (hexComponent.GetComponentInChildren<TMPro.TextMeshPro>())
                {
                    hexComponent.GetComponentInChildren<TMPro.TextMeshPro>().text = column + "," + row;
                }*/
               // if(column == 0 && row == 0)
               /* {
                    GameObject hexModel = Instantiate(hexModelPreFab, hex.PositionInWorld(), Quaternion.identity);
                    hexModel.transform.parent = transform;
                }*/

                //  hexComponent.GetComponentInChildren<TMPro.TextMeshPro>().enabled=false;

            }
        }

        realHexes = realHexesList.ToArray();

        for (int i = 0; i < walls.Length; i++)
        {
            Destroy(walls[i].gameObject);////0;
        }
        Destroy(mapZone.gameObject);
        #region irrelevant
        //StaticBatchingUtility.Combine(gameObject);

        //waterHexMesh = Instantiate(combineableMeshPreFab, transform.position, Quaternion.identity,transform);
        /*mildClimateHexMeshes = new CombineableMesh[mildClimateMaterials.Length];
        for (int i = 0; i < mildClimateHexMeshes.Length; i++)
        {
            mildClimateHexMeshes[i] = Instantiate(combineableMeshPreFab, transform.position, Quaternion.identity, transform);
        }

        for (int i = 0; i < mildClimateHexMeshes.Length; i++)
        {
            mildClimateHexMeshes[i].CombineMeshes(mildClimateMaterials[i]);
        }*/
        #endregion
        mapWasGenerated = true;
    }

    [SerializeField] private Texture2D backGroundTexture;

    #region Hex Material Finding:
    private Color32 GetBackgroundColourFromBounds(Vector3 point, Bounds bounds)
    {
        float x = (point.x - bounds.min.x) / (bounds.max.x - bounds.min.x);
        float y = (point.z - bounds.min.z) / (bounds.max.z - bounds.min.z);
        return GetBackgroundColour(x, y);
       // Bounds normalisedBounds = new Bounds();
        //float maxX = bounds.max.x;
    }
    private Color32 GetBackgroundColour(float x, float y)
    {
        //MeshRenderer renderer = null;
       // MeshFilter meshFilter = null;
        Texture2D texture = backGroundTexture;// renderer.material.mainTexture;
        return texture.GetPixelBilinear(x, y);
       
    }

    private byte FindClosestMaterialIndex(Color32 colour)
    {
        byte closestMatIndex = 0;
        float bestDifference = 12901212;

        for (byte i = 0; i < hexColouredMaterials.Length; i++)
        {
            Color32 matColour = hexColouredMaterials[i].color;
            float RDifference = Mathf.Abs(colour.r - matColour.r);
            float GDifference = Mathf.Abs(colour.g - matColour.g);
            float BDifference = Mathf.Abs(colour.b - matColour.b);
            float averageDifference =
                (RDifference + GDifference + BDifference) / 3;
            if(averageDifference< bestDifference)
            {
                bestDifference = averageDifference;
                closestMatIndex = i;
            }
        }

        return closestMatIndex;

    }

    private ushort? FindClosestDynamicMaterial(Color32 colour)
    {
        ushort? closestMatIndex = null;
        float bestDifference = 12901212;

        for (ushort i = 0; i < dynamicHexColouredMaterials.Count; i++)
        {
            Color32 matColour = dynamicHexColouredMaterials[i].color;
            float RDifference = Mathf.Abs(colour.r - matColour.r);
            float GDifference = Mathf.Abs(colour.g - matColour.g);
            float BDifference = Mathf.Abs(colour.b - matColour.b);
            float averageDifference =
                (RDifference + GDifference + BDifference) / 3;
            if (averageDifference < bestDifference)
            {
                bestDifference = averageDifference;
                closestMatIndex = i;
            }
        }
        if (bestDifference > DynamicColoursDifferenceThreshold)
        {
            closestMatIndex = null;
        }

        Debug.Log(("bestDifference: " + bestDifference.ToString())+ 
            (closestMatIndex!=null?"Colour found": "Colour NOT found"));
        return closestMatIndex;

    }

    #endregion
    /*public static void BuildTerritoryMesh()
    {
        CombineableMesh combineableMesh = instance.territoryHighLights[player.Index];

        foreach (Hex hex in player.Territory)
        {
            Instantiate(instance.hexHighLightPreFab, hex.PositionInWorld(), Quaternion.identity, combineableMesh.transform);
        }

        combineableMesh.CombineMeshes(instance.playersHighLightsMaterials[player.Index]);
    }*/

    public List<Hex> GetHexesInRangeOf(Hex centreHex, int range)
    {
        List<Hex> hexesInRange = new List<Hex>();
        for (int dx = -range; dx < range-1; dx++)
        {
            for (int dy = Mathf.Max(-range+1,-dx-range); dy <  Mathf.Min(range,-dx+range-1); dy++)
            {
                Hex hex = GetHex(centreHex.Q + dx, centreHex.R + dy);
                if (hex != null)
                {
                    hexesInRange.Add(hex);

                }
            }
        }
        return hexesInRange;
    }


    private void FixedUpdate()
    {
        /*if (Input.GetKeyDown(KeyCode.F))
        {
            CalculateFill();
        }*/

        if (awaitingFill &&  !HexPainter.isOnAPotentialWall)
        {
            Debug.Log("CompleteFill" + HexPainter.isOnAPotentialWall);
            CompleteFill();
            awaitingFill = false;
        }
    }
    #region Flood Fill:
    private const sbyte FILLED = -1;
    private const sbyte UNFILLED = -2;
    private static List<Hex> fills = new List<Hex>();
    private static List<Hex> fillsNext = new List<Hex>();

    public struct FillUnit
    {
        public byte q;
        public byte r;

        public FillUnit(byte q, byte r)
        {
            this.q = q;
            this.r = r;
        }
    }

    private static void ClearFloodFillMap()
    {
        for (int i = 0; i < realHexes.Length; i++)
        {
            Hex hex = realHexes[i];
            bool isFillable = (hex.State == HexStates.Empty);
            hex.fillMark =isFillable ? UNFILLED : FILLED;
        }  
    }

    private static List<int> floodFillSlicesSizes = new List<int>();

    private static bool awaitingFill = false;
    private static int frameCount=0;
    public static void CalculateFill()
    {
       if( Time.frameCount == frameCount)
       {
            return;
       }
        frameCount = Time.frameCount;
        Debug.Log("CalculateFill");
        ClearFloodFillMap();
        int width = HexMap.numberOfColumns;
        int height = HexMap.numberOfRows;
        sbyte currentID = -1;

        fills.Clear();
        fillsNext.Clear();
        floodFillSlicesSizes.Clear();

        for (int i = 0; i < realHexes.Length; i++)
        {
            Hex hex = realHexes[i];
            if (hex.fillMark == UNFILLED)
            {
                currentID += 1;
                floodFillSlicesSizes.Add(0);

                hex.fillMark = currentID;
                fills.Add(hex);
            }

            while (fills.Count > 0)
            {
                // yield return new WaitForSeconds(0.004f);
                //loopCount += 1;
                foreach (Hex fill in fills)
                {
                    // yield return new WaitForSeconds(0.002f);
                    Hex[] neighbours = fill.GetNeighbours();
                    Hex neighbour;
                    for (int j = 0; j < neighbours.Length; j++)
                    {
                        neighbour = neighbours[j];
                        if (neighbour.fillMark == UNFILLED)
                        {
                            neighbour.fillMark = currentID;
                            fillsNext.Add(neighbour);
                        }
                    }
                    // Debug.Log("currentID:" + currentID);
                    floodFillSlicesSizes[currentID] += 1;//TODO: bad writing

                    /* if (fy <= 0 || fx <= 0 || fx >= width - 1 || fy >= height - 1)//TODO: find out what this is all about
                     {
                         floodFillMap[fx, fy] = TRANSPARENT;
                         continue;
                     }*/
                }

                List<Hex> swap = fills;
                swap.Clear();
                fills = fillsNext;
                fillsNext = swap;
            }
        }

        if (floodFillSlicesSizes.Count < 2)
        {
            Debug.Log("Number of slices has to be greater than 1 in order to fill the area");
        }
        else
        {
            
            sbyte largestSliceID = 0;
            int largestSizeSoFar = 0;
            for (sbyte i = 0; i < floodFillSlicesSizes.Count; i++)
            {
                if (floodFillSlicesSizes[i] > largestSizeSoFar)
                {
                    largestSliceID = i;
                    largestSizeSoFar = floodFillSlicesSizes[i];
                }
            }

            //List<Hex> hexesToFill = new List<Hex>();
            for (int i = 0; i < realHexes.Length; i++)
            {
                Hex hex = realHexes[i];
                if (hex.State == HexStates.PotentiallyFull||
                   (hex.fillMark >= 0 && hex.fillMark != largestSliceID))
                {
                    hex.ChangeState(HexStates.AwaitingFill);
                    //hexesToFill.Add(hex);
                }
            }


            instance.StartCoroutine(instance.AwaitFillIn());
        }       
    }

    IEnumerator AwaitFillIn()
    {
        /*foreach (Hex hex in hexesToFill)
        {
            hex.ChangeState(HexStates.AwaitingFill);
            //yield return null;
        }*/
        yield return new WaitForSeconds(0.45f);

        awaitingFill = true;
        HexPainter.instance.FloorCheck();
        yield return null;

    }
    private static void CompleteFill()
    {
        bool playSound = false;
        for (int i = 0; i < realHexes.Length; i++)
        {
            Hex hex = realHexes[i];
            if (hex.State == HexStates.AwaitingFill)
            {
                playSound = true;
                hex.ChangeState(HexStates.Full);
            }
        }

        if (playSound)
        {
            Vector3 position = instance.mapCentre;
            SoundManager.PlayOneShotSoundAt(SoundNames.GroundUp, position);
           // SoundManager.PlayOneShotSoundAt(SoundNames.LowGlocken, position);

        }
    }
    #endregion


    public static void PrepareHexExplosion(Hex origin)
    {
        instance.StartCoroutine(instance.PrepareHexExplosionCoroutine(origin));
    }

    private IEnumerator PrepareHexExplosionCoroutine(Hex origin)
    {
        int range = 4;
        float waitTime = 0.15f;
        List<Hex> hexesInRange = new List<Hex>(6);
        hexesInRange.Add(origin);
        if (origin.State == HexStates.Empty)
        {
            origin.ChangeState(HexStates.PotentiallyFull);

        }
        for (int i = 0; i < range; i++)
        {
            yield return new WaitForSeconds(waitTime);
            int count = hexesInRange.Count;
            for (int j = 0; j < count; j++)
            {

                Hex[] neighbours = hexesInRange[j].GetNeighbours();
                for (int n = 0; n < neighbours.Length; n++)
                {
                    Hex neighbour = neighbours[n];
                    if (neighbour.State == HexStates.Empty)// || neighbour.State == HexStates.PotentiallyFull )
                    {
                        neighbour.ChangeState(HexStates.PotentiallyFull);
                    }
                    hexesInRange.Add(neighbour);

                }
            }
        }
        //awaitingFill = true;
    }
}
