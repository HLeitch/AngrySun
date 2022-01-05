using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public GameObject buildingPiece;
    public List<GameObject> buildingPieces;
    public GeneratorManager manager;

    public Seed seedObject;

    public GameObject SelectPiece((int,int) tileNum)
    {
        int[] seed = seedObject.GetSeed();
        int seedNumber = (seed[7] + seed[8]) * (tileNum.Item1+tileNum.Item2) + seed[7];

        int index = seedNumber % buildingPieces.Count;

        GameObject objSelected = buildingPieces[index];
        return objSelected;
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    //Places all buildings into the scene
    public void PlaceBuildings(List<BuildingTileInfo> bTI)
    {

        foreach(BuildingTileInfo tile in bTI)
        {
            GameObject newBuilding = Instantiate(SelectPiece(tile.gridLocation), tile.position, Quaternion.identity);
            newBuilding.transform.Rotate(tile.rotation.x, tile.rotation.y, tile.rotation.z);
        }

        int[] seed = seedObject.GetSeed();
        int seedNumber = seed[7] + seed[8];



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
