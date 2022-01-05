using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [RequireComponent(typeof(Mesh))]
public class Terrain_Generation : MonoBehaviour
{
    public Mesh _mMesh;
    private MeshRenderer _mMeshRenderer;
    Vector3[] vertices;

    // Start is called before the first frame update
    void Start()
    {
        _mMesh = this.GetComponent<MeshFilter>().mesh;

        _mMeshRenderer = this.GetComponent<MeshRenderer>();
        vertices = _mMesh.vertices;

        

        terrianCreation();

    }

    // Update is called once per frame
    void Update()
    { 

    }

     public void terrianCreation()
    {

        /*for(var i = 0; i< vertices.Length; i++)
        {
            vertices[i].y = Mathf.PerlinNoise(vertices[i].x/10,vertices[i].z/10)*10;


            Debug.Log($"Vertex y pos {vertices[i].y} {vertices[i].x} {vertices[i].z}");
        }*/
        
        ///THIS WORKS MAKE IT MORE EXAGGERATED OR SOMETHING
/*
        for (int i = 0; i < vertices.Length; i++)
        {
            float height = Random.Range(1, 20);
            vertices[i].y = height;
        }

            _mMesh.vertices = vertices;
        _mMesh.RecalculateBounds();*/

    }
}
