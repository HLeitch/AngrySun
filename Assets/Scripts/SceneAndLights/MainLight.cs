using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLight : MonoBehaviour
{

    public Light thisLight;

    /// <summary>
    /// Points in exactly the opposite direction to the main light, Should be considerably weaker.
    /// </summary>
    public Light oppositeLight;

    [SerializeField]
    Vector3 rotationOfMainLightOnStart = new Vector3(40,0,0);


    // Start is called before the first frame update
    void Start()
    {
        //thisLight = GetComponent<Light>();
        //oppositeLight = GetComponentInChildren<Light>();

        SetRotationOfMainLight(rotationOfMainLightOnStart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Sets rotation of the main light.
    public void SetRotationOfMainLight(Vector3 rot)
    {
        Quaternion newrot = Quaternion.Euler(rot);
        thisLight.gameObject.transform.rotation = newrot;
        Debug.Log("mainlight rotation set to " + rot);
    }
    public Quaternion GetRotationOfMainLight()
    {
        return thisLight.gameObject.transform.rotation;
    }
}
