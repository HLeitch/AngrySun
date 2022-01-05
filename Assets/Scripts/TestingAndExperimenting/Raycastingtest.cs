using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycastingtest : MonoBehaviour
{
    // Start is called before the first frame update

    public Vector3 rayvec;
    public float distance = 1000;
    RaycastHit rayHit;
    void Start()
    {
        rayvec = Vector3.down * distance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void projectRay()
    {
       
        if (Physics.Raycast(this.transform.position, rayvec, out rayHit))
        {
            Debug.DrawRay(this.transform.position, rayvec, Color.green);

        }
        else
            Debug.DrawRay(this.transform.position, rayvec, Color.red);
    }
    private void OnDrawGizmos()
    {
        projectRay();
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(rayHit.point, 2);
    }
}
