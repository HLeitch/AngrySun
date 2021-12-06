using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MeshRenderer))]
public class WIndowPositioner : MonoBehaviour
{
    public MeshRenderer _mMesh;

    //max axis to any point on the mesh from the center.
    private Vector3 meshLims { get { return _mMesh.bounds.extents; } }
    //max distance to any point from the center of the mesh.
    private float maxInternalDistance;
    [SerializeField]
    [Range(0.0f,3.0f)]
    private float distanceBetweenWindows = 2f;
    private float gapBetweenFloors { get { return meshLims.y * 2 / numberOfFloors + 1; } }

    [SerializeField]
    private int numberOfFloors = 5;

    public GameObject window;

    /// <summary>
    /// returns an approximate normalised direction to the closest mesh. recommended to create int results from -1 -> 1.
    /// </summary>
    private Vector3 roughDirectionToMesh { get { return (_mMesh.bounds.ClosestPoint(transform.position) - this.transform.position).normalized; } }

    /// <summary>
    /// Find the distance to the closest point on the mesh renderer
    /// </summary>
    private float distanceToSurface { get { return (_mMesh.bounds.ClosestPoint(this.transform.position) - this.transform.position).magnitude; } }

    private float angleBetweenWindowRays { get{ return Mathf.Atan2(distanceBetweenWindows, distanceToSurface); } }

    private GameObject GetWindow()
    {
        return window;
    }
  
    // Start is called before the first frame update
    void Start()
    {
        if (_mMesh == null)
        {
            Debug.LogError($"ATTACH MESH TO WINDOWPOSITIONER, {this}");
        }
        else
        {
        }

    }

    public void GenerateWindows()
    {
        foreach ((Vector3, Vector3) pointData in FindWindowPositionsAndNormals())
        {

            Quaternion rot = Quaternion.LookRotation(pointData.Item2, Vector3.up);
            Vector3 pos = pointData.Item1 + (0.05f * pointData.Item2);


            GameObject newWindow = Instantiate(GetWindow(), pos, rot, this.transform);


        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Returns a list of tuples. Position of windows (in world space) and the normal rotation of these positions.
    /// </summary>
    /// <returns></returns>
    private List<(Vector3,Vector3)> FindWindowPositionsAndNormals()
    {
        List<(Vector3,Vector3)> positions = new List<(Vector3, Vector3)>();
        RaycastHit HitInstance = new RaycastHit();
        Vector3 _roughDirectionToMesh = roughDirectionToMesh;

        //amount of iterations required to place a window at every desired point. 
        //TODO: ADJUST NUMBER FOR X AND Z SIDES
        int numWindowsInRow = (int) (meshLims.x * 2 / distanceBetweenWindows);

        float gapBetweenFloors = meshLims.y*2 / (numberOfFloors + 1);

        int floor = 0;
        while (floor < numberOfFloors)
        {
            float heightOfFloor = (0.5f * gapBetweenFloors) + (floor * gapBetweenFloors);

            int iteration = 0;
            while (iteration < numWindowsInRow)
            {
                
                Vector3 directionalDistance = _roughDirectionToMesh*distanceToSurface;

                Vector3 raycastPos = this.gameObject.transform.position;

                //Raycast to a point on the building. If on x side the z coord will be displaced and vice versa
                raycastPos.x += (iteration * distanceBetweenWindows*_roughDirectionToMesh.z);
                raycastPos.z += (iteration * distanceBetweenWindows * _roughDirectionToMesh.x);


                raycastPos.y += heightOfFloor;
                if (Physics.Raycast(raycastPos, directionalDistance, out HitInstance))
                {

                    Debug.Log("raycast collided");
                    //replace gameObject rotation with normal to surface of mesh
                    Vector3 position = HitInstance.point;
                    Vector3 normalAngle = HitInstance.normal;
                    positions.Add((position, normalAngle));

                }
                iteration++;
            }
            floor++;
        }



        return positions;
    }



    private void OnDrawGizmosSelected()
    {
        foreach( (Vector3,Vector3) pointData in FindWindowPositionsAndNormals())
        {
            Gizmos.color = Color.magenta;

            Gizmos.DrawSphere(pointData.Item1, 0.2f);
        }
    }

}
