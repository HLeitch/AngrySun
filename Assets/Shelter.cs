using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelter : MonoBehaviour
{

    public Transform raycastTransform;
    [SerializeField]
    List<GameObject> ShelterPieces;

    Ray raycastCenterPoint;
    Ray raycastFurthestPoint;
    public LayerMask raycastLayerMask;

    public bool showDebugRays;

    public int minShelterPieces;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public bool Generate(int numberOfRaycasts, Vector3 directionToLight)
    {

        Debug.Log("Shelter Generating");
        float angleToRotate = 360 / numberOfRaycasts;

        int ShelterGenerated = 0;


        //raycast points in a circle around the campfire, then instantiate shelter depending on if in light or if colliding with anything else. since trees should come before shelter
        //use actual physical location to determine which shelters will be placed where.
        int count = 0;
        RaycastHit rayHit;
        while (count < numberOfRaycasts)
        {
            int peicesSpawned = 0;

            if(RaycastFromTransform(out rayHit))
            {
                ShelterGenerated++;

                //spawn shelter piece and rotate towards fire
                GameObject spawned = Instantiate(ShelterPieces[peicesSpawned], rayHit.point,Quaternion.identity);
                spawned.transform.LookAt(this.gameObject.transform);

                peicesSpawned++;
                if(peicesSpawned == ShelterPieces.Count) { peicesSpawned = minShelterPieces; }
                //Rotates around y axis in world space.
                raycastTransform.Rotate(new Vector3(0, angleToRotate, 0), Space.World);
            }
            count++;
        }

        if(ShelterGenerated == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    private bool RaycastFromTransform(out RaycastHit rayHit)
    {
        raycastCenterPoint.origin = raycastTransform.position;
        raycastCenterPoint.direction = raycastTransform.forward;

        //raycast the furthest point parallel from a short distance above the closest point raycast.
        raycastFurthestPoint.origin = raycastTransform.position + new Vector3(0,0.5f,0);
        raycastFurthestPoint.direction = raycastTransform.forward;

        if (Physics.Raycast(raycastCenterPoint,out rayHit, 200f, raycastLayerMask,QueryTriggerInteraction.Collide))
        {
            if (rayHit.collider.tag == "Ground")
            {
                RaycastHit secondRayHit;


                if (Physics.Raycast(raycastFurthestPoint, out secondRayHit, 200f, raycastLayerMask, QueryTriggerInteraction.Collide))
                {
                    //only true if both raycasts hit an object marked as ground

                    if (secondRayHit.collider.tag == "Ground")
                    {
                        return true;
                    }
                    else { return false; }
                }

                else { return false; }
            }
            else { return false; }
        }
        else { return false; }

    }
    private void OnDrawGizmos()
    {
        if (showDebugRays)
        {
            RaycastHit blank;
            if (RaycastFromTransform(out blank))
            {
                raycastTransform.Rotate(new Vector3(0, 7, 0), Space.World);
                Debug.DrawRay(raycastCenterPoint.origin, raycastCenterPoint.direction * 200, Color.green);
                Debug.DrawRay(raycastFurthestPoint.origin, raycastCenterPoint.direction * 200, Color.green);
            }
            else
            {
                raycastTransform.Rotate(new Vector3(0, 7, 0), Space.World);
                Debug.DrawRay(raycastCenterPoint.origin, raycastCenterPoint.direction * 200, Color.red);
                Debug.DrawRay(raycastFurthestPoint.origin, raycastCenterPoint.direction * 200, Color.red);
            }
        }
    }
}
