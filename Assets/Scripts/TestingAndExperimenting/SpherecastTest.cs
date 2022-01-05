using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherecastTest : MonoBehaviour
{
    public float sizeOfSphere = 10;
    public float sizeOfHitSphere = 3;
    RaycastHit[] hits;
    Ray myRay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDrawGizmos()
    {
        hits = Physics.SphereCastAll(myRay,sizeOfSphere);
        foreach (RaycastHit h in hits)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(h.point, sizeOfHitSphere);
        }
           
    }
}
