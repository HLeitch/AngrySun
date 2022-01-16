using System;
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
    [SerializeField]
    [Range(0,10)]
    private int numOfWindowsInRow = 3;
    private float gapBetweenFloors { get { return meshLims.y * 2 / numberOfFloors + 1; } }

    [SerializeField]
    private int numberOfFloors = 5;

    public GameObject window;
    public GameObject brokenWindow;

    public GameObject parent;

    /// <summary>
    /// Window Positioning will not execute while this is false
    /// </summary>
    public bool inUse;
    [SerializeField]
    private Vector3 scaleOfWindows;
    [SerializeField]
    private bool rotateToFaceMesh;

    /// <summary>
    /// returns an approximate normalised direction to the closest mesh. recommended to create int results from -1 -> 1.
    /// </summary>
    private Vector3 roughDirectionToMesh { get 
        {
            //Vector3 center = new Vector3(_mMesh.bounds.center.x, 0, _mMesh.bounds.center.z);
            Vector3 center = _mMesh.bounds.ClosestPoint(this.transform.position);
            Vector3 positionOfPlacer = this.transform.position;
            return (center - positionOfPlacer); } 
    }

    /// <summary>
    /// Find the distance to the closest point on the mesh renderer
    /// </summary>
    private float distanceToSurface { get { return (_mMesh.bounds.ClosestPoint(this.transform.position) - this.transform.position).magnitude; } }

    private float angleBetweenWindowRays { get{ return Mathf.Atan2(distanceBetweenWindows, distanceToSurface); } }

    private GameObject GetWindow((Vector3, Vector3) pointData)
    {
        if (pointData.Item1.y < Mathf.PerlinNoise(pointData.Item1.x, pointData.Item1.z) * 10f)
        {
            return brokenWindow;
        }
        else { return window; };
    }
  
    // Start is called before the first frame update
    void Start()
    {
        parent = GetComponentInParent<GameObject>();

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
        if (inUse)
        {
            foreach ((Vector3, Vector3) pointData in CalculateWindowPositionsAndNormals())
            {

                Quaternion rot = Quaternion.LookRotation(pointData.Item2, Vector3.up);
                Vector3 pos = pointData.Item1 + (0.05f * pointData.Item2);
                Vector3 localPos = pos - transform.position;

                GameObject newWindow = Instantiate(GetWindow(pointData), pos, rot, this.transform);

                newWindow.transform.localScale = scaleOfWindows;



            }
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
   

    private List<(Vector3, Vector3)> CalculateWindowPositionsAndNormals()
    {
        List<(Vector3, Vector3)> positions = new List<(Vector3, Vector3)>();




        float gapBetweenFloors = meshLims.y * 2 / (numberOfFloors);

        int floor = 0;
        while (floor < numberOfFloors)
        {
            float heightOfFloor = (0.5f * gapBetweenFloors) + (floor * gapBetweenFloors);

            int iteration = 0;
            RaycastHit HitInstance;
            while (iteration < numOfWindowsInRow)
            {
                //Shift the window a distance away from the previous
               Vector3 raycastPos =  transform.position+( transform.right * distanceBetweenWindows*iteration);
                raycastPos.y += heightOfFloor;
                if (Physics.Raycast(raycastPos, transform.forward, out HitInstance))
                {
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

    private void pointTowardsMesh()
    {
        if(rotateToFaceMesh)
        // Rotate the WindowPositioner to look at the mesh
        transform.LookAt(_mMesh.bounds.ClosestPoint(transform.position));

    }

    private void OnDrawGizmosSelected()
    {
        if (inUse)
        {
            foreach ((Vector3, Vector3) pointData in CalculateWindowPositionsAndNormals())
            {
                Gizmos.color = Color.magenta;

                Gizmos.DrawSphere(pointData.Item1, 0.2f);
                


            }
            Gizmos.DrawRay(transform.position, roughDirectionToMesh);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 0.4f);
        }
    }
}
