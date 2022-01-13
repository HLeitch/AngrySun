using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene_Cracks : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] CrackLibrary;

    public List<Vector3> Positions;


    private Seed seedObject;

    TerrainGeneration tg;

    /// <summary>
    /// local version of the seed passed into the level. This is a clone of the seed in the Seed Component
    /// </summary>
    int[] _lseed;

    GeneratorManager parent = null;


    void Start()
    {
        if (parent is null) { Initialise(); }

        // placeCracks();
    }

    /// <summary>
    /// Can be handled by itself or called by another class
    /// </summary>
    public void Initialise()
    {
        parent = GetComponentInParent<GeneratorManager>();
        seedObject = parent.seedObject;
        _lseed = seedObject.GetSeed();
    }

    public void PlaceCracks()
    {
        RetreivePositions(5);

        int counter = 0;
        foreach(Vector3 t in Positions)
        {
            int chosenCrack = 3;
            
            Instantiate(CrackLibrary[chosenCrack],t,Quaternion.identity);
        }
    }

    void RetreivePositions(int numberOfCracksPerRow)
    {
        TerrainGeneration terrainManager = parent._terrainGenerator;
        int spacing = terrainManager.gridResolution / numberOfCracksPerRow;

        int counterX = 0;
        int counterZ = 0;
        while (counterX < terrainManager.gridResolution)
        {
            while (counterZ < terrainManager.gridResolution)
            {

                Vector3 newLoc = terrainManager._terrainTileLocations[counterX, counterZ];
                counterZ += spacing;
                Positions.Add(newLoc);
            }
            counterZ = 0;
            counterX += spacing;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
