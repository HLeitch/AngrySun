using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorManager : MonoBehaviour
{
    public BuildingGenerator _buildingGenerator;
    public TerrainGeneration _terrainGenerator;


    // Start is called before the first frame update
    void Start()
    {
        _terrainGenerator.CreateTerrain();
        _buildingGenerator.PlaceBuildings(_terrainGenerator.GetAllLocationsAndRotationsOfType(TerrainType.Building));
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
