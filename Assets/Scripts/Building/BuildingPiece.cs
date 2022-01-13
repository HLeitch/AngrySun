using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BuildingPiece : MonoBehaviour
{

    bool isEntrance = false;
    public WIndowPositioner[] windowPositioners;
    /// <summary>
    /// Place a building piece with only windows if appropriate
    /// </summary>
    public GameObject multiLevelPiece;
    /// <summary>
    /// Can have multiple levels
    /// </summary>
    public bool multiLevel = true;

    public Mesh _mMesh;
    [SerializeField]
    private MeshRenderer _mMeshRenderer;
    // Start is called before the first frame update

    void Awake()
    {
        //_mMeshRenderer = this.GetComponent<MeshRenderer>();
        multiLevelPiece = this.gameObject;



        

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

       */
    }

// Update is called once per frame
    void Update()
    {
        
    }
    public void GrowBuilding(int heightInPieces)
    {

        if (multiLevel)
        {
            int counter = 0;
            while (counter < heightInPieces)
            {
                counter++;
                Vector3 newPieceTransform = new Vector3(transform.position.x, transform.position.y, transform.position.z);

                //A piece will be placed at a multiple of the height of base piece.
                newPieceTransform.y = (counter * _mMeshRenderer.bounds.size.y);

                GameObject newLevel = Instantiate(multiLevelPiece, newPieceTransform, this.transform.rotation);

                //prevents recursive building growth
                newLevel.GetComponent<BuildingPiece>().multiLevel = false;

            }
        }


    }


}
