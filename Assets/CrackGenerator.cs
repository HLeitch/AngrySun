using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackGenerator : MonoBehaviour
{

    public LSystemCrack startPoint;
    public int _levels = 4;
    Vector3[] vertices;
    public Mesh _mMesh;

    private MeshRenderer _mMeshRenderer;

    public LSystemCrack[] cracks {get{ return gameObject.GetComponentsInChildren<LSystemCrack>(); } }

    bool generated = true;

    // Start is called before the first frame update
    void Start()
    {
        if (!generated) { StartCoroutine("MassPropergation"); }

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);

        _mMesh = gameObject.GetComponentInChildren<MeshFilter>().mesh;

        _mMeshRenderer = this.GetComponent<MeshRenderer>();
        vertices = _mMesh.vertices;

        CrackDeformation();
    }
    public void CrackDeformation()
    {
        for (var i = 0; i < vertices.Length; i++)
        {
            vertices[i].z += Mathf.PerlinNoise(vertices[i].x / 5f, vertices[i].z / 5f) * 2;
            vertices[i].x += Mathf.PerlinNoise(vertices[i].x / 5f, vertices[i].z / 5f) * 2;


            Debug.Log($"Vertex y pos {vertices[i].y} {vertices[i].x} {vertices[i].z}");
        }

        _mMesh.vertices = vertices;
        _mMesh.RecalculateBounds();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator MassPropergation()
    {
        int loops = 0;
        while(loops< _levels)
        {
            foreach(LSystemCrack c in cracks)
            {
                if(c.Level == loops)
                {
                    c.BeginPropergation();
                }
                

            }
            loops++;
            yield return null;

        }


    }
}
