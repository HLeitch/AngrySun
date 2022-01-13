using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GeneratorManager generatorManager;
    TerrainGeneration _terrainGenerator;
    public GameObject cameraOnTrackObject;

    public int numOfCameras = 2;

    List<CameraOnTrack> cameras = new List<CameraOnTrack>();

    // Start is called before the first frame update
    void Awake()
    {
        generatorManager = FindObjectOfType<GeneratorManager>();
        _terrainGenerator = generatorManager._terrainGenerator;


    }

    public void Generate()
    {
        CameraOnTrack camCompPointer;
        for (int index = 0; index < numOfCameras; index++)
        {
            GameObject newCam = Instantiate(cameraOnTrackObject, this.transform);
            if (newCam.TryGetComponent<CameraOnTrack>(out camCompPointer))
            {
                camCompPointer.addTrackNodes(_terrainGenerator.GetAllLocationsOfType(TerrainType.Road).ToArray());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
