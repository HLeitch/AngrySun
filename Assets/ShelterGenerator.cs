using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shelter is generated on a ground tile where there is an collidable object between the shelter and the direction vector opposite to the directional light
/// </summary>
public class ShelterGenerator : MonoBehaviour
{
    public MainLight lightSource;
    public GeneratorManager generatorManager;

    public GameObject shelter;

    public List<Shelter> sheltersSpawned = new List<Shelter>();

    private List<Vector3> locations { get { return generatorManager._terrainGenerator.GetAllLocationsOfType(TerrainType.Ground); } }

    public bool _debugging = false;

    public float maxRayDistance;
    public float cameraTrackLength = 75f;
    /// <summary>
    /// returns a euler angle representation of the direction to the main light
    /// </summary>
    private Vector3 DirectionToLight { get {
            Quaternion rot = lightSource.GetRotationOfMainLight();
             Vector3 returnval = -(rot * this.transform.forward);
            return returnval;
        } }

    /// <summary>
    /// raycasts used to test if the area is clear.
    /// </summary>
    private Ray[] raycasts = new Ray[4];

    public LayerMask raycastLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool TryPlaceShelter(Vector3 location)
    {
        //first try a single raycast from the point in the center of the terrain square. Fired to a max distance so the distance between shelter and buildings is low. 

        if (CastSingleRayTowardLight(ref raycasts[0], location))
        {
            Debug.Log($"Shelter can be placed here, {location}");
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool InstantiateShelter(Vector3 location)
    {
        GameObject _shelter = Instantiate(shelter, location, Quaternion.identity);
        Shelter _shelterComponent;
        if (_shelter.TryGetComponent<Shelter>(out _shelterComponent))
        {

            if (_shelterComponent.Generate(8, DirectionToLight))
            {
                sheltersSpawned.Add(_shelterComponent);
                return true; 
            }


            else {
                GameObject.Destroy(_shelter);
                return false; 
            }
        }
        else
        {
            Debug.LogError("Shelter Prefab does not have a Shelter Component");
            return false;
        }
    }

    public void PlaceShelter(List<Vector3> locations)
    {
        List<Vector3> possibleLocations = new List<Vector3>();

        foreach(Vector3 location in locations)
        {
            if(TryPlaceShelter(location))
            {
                possibleLocations.Add(location);
            }
        }
        if(possibleLocations.Count > 0)
        {
            int originalShelterSpawn = (possibleLocations.Count / 5) * 4;



            int shelterSpawn = originalShelterSpawn;
            //while generating shelter is unsuccessful
            while(!InstantiateShelter(possibleLocations[shelterSpawn]))
            {
                shelterSpawn++;
                if(shelterSpawn == possibleLocations.Count)
                {
                    shelterSpawn = 0;
                }
                if(shelterSpawn == originalShelterSpawn)
                {
                    Debug.Log("there are no valid locations for shelter spawn");
                    break;
                }
            }
            
        }
        else
        {
            Debug.LogWarning("There is no Shelter");
        }


    }

    //Starts a short distance from the shelter then moves away
    public List<Vector3> FindCameraTrackPositions()
    {
        List<Vector3> camTrackPositions = new List<Vector3>();

        if(sheltersSpawned.Count > 0)
        {
            Ray cameraTrackRay = new Ray();
            Shelter shelterFilming = sheltersSpawned[0];
            Transform rayTransform = shelterFilming.transform;

            cameraTrackRay.origin = rayTransform.position + new Vector3(0, 2f, 0);
            cameraTrackRay.direction = rayTransform.forward;

            float rotationAngle = 0f;

            //move camera away from other colliders. Camera will move away from colliders.
            while(Physics.Raycast(cameraTrackRay,cameraTrackLength,raycastLayerMask) && rotationAngle<360.0f)
            {
                rayTransform.Rotate(new Vector3(0, 5f, 0));
                rotationAngle += 5f;

                cameraTrackRay.direction = rayTransform.forward;
            }
            //when the raycast hasn't hit anything
            if(rotationAngle<360)
            {
                //start a distance from the camp fire
                Vector3 cameraTrackStartPosition = cameraTrackRay.origin + cameraTrackRay.direction * 2;

                camTrackPositions.Add(cameraTrackStartPosition);

                //from origin move outward in the direction of the raycast
                Vector3 camEnd = cameraTrackRay.origin + (cameraTrackRay.direction * cameraTrackLength);

                camTrackPositions.Add(camEnd);
            }


        }
        return camTrackPositions;
    }

    private bool CastSingleRayTowardLight(ref Ray ray, Vector3 location)
    {
        ray.origin = location;

        //travel in opposite direction of light
        ray.direction = DirectionToLight;
        return Physics.Raycast(ray, maxRayDistance,raycastLayerMask);
    }

    private void OnDrawGizmos()
    {

        if (_debugging)
        {
            foreach (Vector3 loc in locations)
            {
                if (CastSingleRayTowardLight(ref raycasts[0],loc))
                {
                    Gizmos.DrawRay(raycasts[0]);
                }
            }
        } 
    }

}
