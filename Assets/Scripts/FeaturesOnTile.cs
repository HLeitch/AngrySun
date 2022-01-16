using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FeaturesOnTile : MonoBehaviour
{

    public TerrainType DestinationTerrainType;
    public List<GameObject> versions;

    public float OverlapSphereSize = 2.0f;
    
    public int maxSpawnedPerTile =5;
    protected GeneratorManager generatorManager;
    protected int[] seedAccessor  { get{ return generatorManager.seedObject.GetSeed(); } }


    public abstract void Spawn();

    public GameObject Generate(GameObject obj, Vector3 position,Vector3 rotation)
    {
        GameObject spawned = Instantiate(obj, position, Quaternion.identity, this.transform);
        spawned.transform.Rotate(rotation);

        return spawned;
    }

    /// <summary>
    /// Generates an object with random postion and rotation within bounds. Spawns ins the same ypos as before.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="position"></param>
    /// <param name="maxdisplacement"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public GameObject NoisyGenerate(GameObject obj,Vector3 position, float maxdisplacement,float modifier)
    {
        //These values make the perlin noise less patterned across tiles.
        Vector3 modifierValues = modifier * position;


        int[] _seed = seedAccessor;
        float xNoise = Mathf.PerlinNoise(position.x/100 + modifierValues.x, position.y);
        float zNoise = Mathf.PerlinNoise(position.y, position.z + modifierValues.z/100);

        //Give these values signs so spawning happens around a point in a circle
        float signedXNoise = (xNoise - 0.5f)*5;
        float signedZNoise = (zNoise - 0.5f)*5;

        float dispX = maxdisplacement * signedXNoise;
        float dispZ = maxdisplacement * signedZNoise;

        //rotation is defined using the disp_ values
        float yNoise = Mathf.PerlinNoise(dispX, dispZ);
        float yRot = 360 * yNoise;


        Vector3 noisyPosition = position + new Vector3(dispX, 0, dispZ);

        //Should only collide with the ground
        if (Physics.OverlapSphere(position, OverlapSphereSize).Length == 1)
        {
            GameObject spawned = Instantiate(obj, this.transform, true);
            spawned.transform.position = noisyPosition;
            spawned.transform.Rotate(new Vector3(0, yRot, 0));

            return spawned;
        }
        else
        {
            Debug.LogError("Spawning a feature failed because of a collison");
            return null; }
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
