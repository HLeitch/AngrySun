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

    public bool generated;
    public bool deformed = false;

    // Start is called before the first frame update
    void Start()
    {
        //Call Propergation and combines/deforms meshes
        if (!generated) { StartCoroutine("MassPropergation");
                          }

        //combining/deforming mesh
        else {
        }
    }

    private void CombineChildMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;

            ///Adding meshes to combined mesh at true positions/rotations.
            Transform meshTransform = meshFilters[i].gameObject.transform;
            combine[i].transform = Matrix4x4.TRS(meshTransform.position - gameObject.transform.position, meshTransform.rotation, meshTransform.lossyScale);
            i++;
        }


        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);

        Debug.Log($"mesh Assignment {this} : " + Time.realtimeSinceStartup);
        _mMesh = gameObject.GetComponentInChildren<MeshFilter>().mesh;

        _mMeshRenderer = this.GetComponent<MeshRenderer>();

        vertices = _mMesh.vertices;

        foreach (Transform child in this.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void CrackDeformation()
    {
        for (var i = 0; i < vertices.Length; i++)
        {
            vertices[i].z += Mathf.PerlinNoise(vertices[i].x / 3f, vertices[i].z / 3f) * 2;
            vertices[i].x += Mathf.PerlinNoise(vertices[i].x / 3f, vertices[i].z / 3f) * 2;
        }

        _mMesh.vertices = vertices;
        _mMesh.RecalculateBounds();
        _mMesh.RecalculateNormals();
        
    }

    struct CrackDeformerJob : Unity.Jobs.IJob
    {
        Vector3[] vertices;
        Mesh _mMesh;

        public void Execute()
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
    }


    // Update is called once per frame
    void Update()
    {
        if(generated && !deformed)
        {
            CombineChildMeshes();
            CrackDeformation();
            deformed = true;
        }
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
        generated = true;

    }
}
