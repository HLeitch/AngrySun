using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public GameObject buildingPiece;

    public GeneratorManager manager;

    public GameObject SelectPiece()
    {
        return buildingPiece;
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    //Places buildings in the scene. Takes an array of locations to fill
    public void PlaceBuildings((List<Vector3> ,List<Vector4>) data )
    {
        List<Vector3> locs = data.Item1;
        List<Vector4> rots = data.Item2;
        foreach (Vector3 location in locs)
        {
            Quaternion rotation = new Quaternion(0,rots[locs.IndexOf(location)].y,0,0);
            Instantiate(SelectPiece(), location, rotation);
            Debug.Log($"Building placed at: {location}, rotation {rotation}");
        }

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
