using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GeneratorManager : MonoBehaviour
{
    [SerializeField]
    private bool _generated = false;

    public BuildingGenerator _buildingGenerator;
    public TerrainGeneration _terrainGenerator;
    public Scene_Cracks _sceneCracks;
    public Seed seedObject;
    public CameraController cameraController;
    public ShelterGenerator shelterGenerator;

    public MainLight mainLight;

    // Start is called before the first frame update
    void Start()
    {
        mainLight = GameObject.FindObjectOfType<MainLight>();
        Generate();
    }

    private void Generate()
    {
        if (!_generated)
        {
            StartCoroutine(_Generate());
        }
    }

    /// <summary>
    /// allows pause for a frame update before certain functions are called to allow physics to be calculated.
    /// </summary>
    /// <returns></returns>
    IEnumerator _Generate()
    {
        _generated = true;
        _terrainGenerator.CreateTerrain();
        _buildingGenerator.PlaceBuildings(_terrainGenerator.GetAllBuildingTileInfo());

        yield return new WaitForFixedUpdate();

        cameraController.Generate();


        Debug.Log(_terrainGenerator.TestTerrainTypeArray());
        shelterGenerator.PlaceShelter(_terrainGenerator.GetAllLocationsOfType(TerrainType.Ground));
        _sceneCracks.Initialise();
        _sceneCracks.PlaceCracks();
        Debug.Log($"Generator Manager completed: {Time.realtimeSinceStartup}");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            Generate();
            
        }
    }
}
