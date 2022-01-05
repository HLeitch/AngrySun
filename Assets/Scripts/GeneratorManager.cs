using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorManager : MonoBehaviour
{
    public BuildingGenerator _buildingGenerator;
    public TerrainGeneration _terrainGenerator;
    public Scene_Cracks _sceneCracks;
    public Seed seedObject;

    // Start is called before the first frame update
    void Start()
    {
        _terrainGenerator.CreateTerrain();
        _buildingGenerator.PlaceBuildings(_terrainGenerator.GetAllBuildingTileInfo());
        _sceneCracks.PlaceCracks();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
