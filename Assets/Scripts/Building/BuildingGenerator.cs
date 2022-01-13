using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public GameObject buildingPiece;
    public List<GameObject> buildingPieces;
    public GameObject shop;
    public GeneratorManager manager;
    private TerrainGeneration _terrainGenerator { get { return manager._terrainGenerator; } }


    public (int, int) cityCenterPoint = (15,7);

    private float maxDistanceToCityCenter;
    private float getMaxDistanceToCityCenter { get
        {
            float gridSize = manager._terrainGenerator.gridResolution;
            return (Mathf.Sqrt((gridSize * gridSize) * 2));
        } }
    public Seed seedObject;
    [SerializeField]
    private int maxHeightInPieces = 4;
    [SerializeField]
    /// <summary>
    /// Adjusts the rate at which buildings height changes across the scene
    /// </summary>
    private float buildingHeightFalloff = 1;
    public GameObject SelectPiece((int,int) tileNum)
    {

        int[] seed = seedObject.GetSeed();
        int seedNumber = (seed[7] + seed[8]) * (tileNum.Item1+tileNum.Item2) + seed[7];

        

        int index = seedNumber % buildingPieces.Count;
        if (_terrainGenerator.CountTilesOfSurroundingType(TerrainType.Road, tileNum.Item1, tileNum.Item2) == 2 && ((seedNumber*3)%(seed[5]+1) == 0))
        {
            return shop;
        }
        else
        {
            GameObject objSelected = buildingPieces[index];
            return objSelected;
        }
    }
    public int GetHeightInPieces((int,int) tileNum)
    {
        /*        int[] seed = seedObject.GetSeed();

                //simple noise
                //int perlinInt = (int) (Mathf.PerlinNoise((float) tileNum.Item1 * (seed[5] + 2),(float) tileNum.Item2 * (seed[4] + 2)) *100);
                float perlinInt = (Mathf.PerlinNoise((float)tileNum.Item1/10, (float)tileNum.Item2/10) * 100);
                Debug.Log(perlinInt);
        */
        /*
                //the seed number is returned by multiplying the sum of the coordinates of the tile with the perlin noise value given above plus a number from the seed. 
                int seedNumber = (int) ((tileNum.Item1 + tileNum.Item2 + 2) * perlinInt  + seed[7]);

                //the remainder when the seed is modulo-ed against the max height gives the height of the building piece.
                int heightInPieces = seedNumber % maxHeightInPieces;*/

        //Decide height based on distance from center of city
        Vector2 distVec = new Vector2(cityCenterPoint.Item1 - tileNum.Item1, cityCenterPoint.Item2 - tileNum.Item2);
        float dist = distVec.magnitude;
        float normalisedDistance =(float) 1f - (1f * (dist / maxDistanceToCityCenter * buildingHeightFalloff));

        int height =(int) ( ((float) maxHeightInPieces + 0.5f) * normalisedDistance);
        Debug.Log($"Building Height = {height}");

        return height;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    //Places all buildings into the scene
    public void PlaceBuildings(List<BuildingTileInfo> bTI)
    {
        maxDistanceToCityCenter = this.getMaxDistanceToCityCenter;

        FindCenterPoint(bTI);

        foreach (BuildingTileInfo tile in bTI)
        {
            GameObject newBuilding = Instantiate(SelectPiece(tile.gridLocation), tile.position, Quaternion.identity);
            newBuilding.transform.Rotate(tile.rotation.x, tile.rotation.y, tile.rotation.z);

            BuildingPiece bpComponent;

            if (newBuilding.TryGetComponent<BuildingPiece>(out bpComponent))
            {
                bpComponent.GrowBuilding(GetHeightInPieces(tile.gridLocation));

            }
            else
            {
                Debug.LogError("Attach a bulding piece component to the instantiated object");
            }

        }

        int[] seed = seedObject.GetSeed();
        int seedNumber = seed[7] + seed[8];



    }

    private void FindCenterPoint(List<BuildingTileInfo> bTI)
    {

        int cityCenterPointIndex = seedObject.GetSeed()[6] * 7 % bTI.Count;
        cityCenterPoint = bTI[cityCenterPointIndex].gridLocation;
    }

    public void _TestBuildingGeneration()
    {
        List<Vector3> locs = new List<Vector3> { new Vector3(0, 0, 0), new Vector3(5, 0, 0), new Vector3(10, 0, 0) };
        //PlaceBuildings(locs);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
