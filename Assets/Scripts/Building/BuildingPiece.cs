using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BuildingPiece : MonoBehaviour
{

    bool isEntrance = false;
    public WIndowPositioner[] windowPositioners;

    public Mesh _mMesh;

    private MeshRenderer _mMeshRenderer;
    // Start is called before the first frame update

    void Start()
    {
        foreach(WIndowPositioner w in windowPositioners)
        {

            if (w.inUse == true) { w.GenerateWindows(); }

        }




      /*  MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;

            ///Adding meshes to combined mesh at true positions/rotations.
            Transform meshTransform = meshFilters[i].gameObject.transform;
            combine[i].transform = Matrix4x4.TRS(meshTransform.position - gameObject.transform.position, meshTransform.rotation, meshTransform.lossyScale);
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }


        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);

        Debug.Log($"mesh Assignment {this} : " + Time.realtimeSinceStartup);
        _mMesh = gameObject.GetComponentInChildren<MeshFilter>().mesh;

        _mMeshRenderer = this.GetComponent<MeshRenderer>();*/
    }

// Update is called once per frame
void Update()
    {
        
    }
}
