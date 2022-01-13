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

    public List<GameObject> shelterObjects;
    public GameObject campFire;

    private List<Vector3> locations { get { return generatorManager._terrainGenerator.GetAllLocationsOfType(TerrainType.Ground); } }

    public bool _debugging = false;

    public float maxRayDistance;
    /// <summary>
    /// returns a euler angle representation of the direction to the main light
    /// </summary>
    private Quaternion DirectionToLight { get {
            Quaternion rot = lightSource.GetRotationOfMainLight();
            return rot;
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

            Instantiate(campFire, location, Quaternion.identity);

            //raycast points in a circle around the campfire, then instantiate shelter depending on if in light or if colliding with anything else. since trees should come before shelter
            //use actual physical location to determine which shelters will be placed where.

            return true;
                }
        else
        {
            return false;
        }
    }
    public void PlaceShelter(List<Vector3> locations)
    {
        foreach(Vector3 location in locations)
        {
            TryPlaceShelter(location);
        }
    }



    private bool CastSingleRayTowardLight(ref Ray ray, Vector3 location)
    {
        ray.origin = location;

        //travel in opposite direction of light
        ray.direction = -(DirectionToLight * this.transform.forward);
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
