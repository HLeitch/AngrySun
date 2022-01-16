using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TreeGenerator : FeaturesOnTile
{

    public override void Spawn()
    {
        List<Vector3> groundTiles = generatorManager._terrainGenerator.GetAllLocationsOfType(TerrainType.Ground);
        

        foreach (Vector3 tile in groundTiles)
        {


            int counter = 0;
            while (counter < maxSpawnedPerTile)
            {

                int type = counter % versions.Count;
                NoisyGenerate(versions[type], tile, 5, FeaturesModifierArrays.trees[counter]);
                counter++;
            }
            
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        generatorManager = FindObjectOfType<GeneratorManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
