﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BuildingPiece : MonoBehaviour
{

    bool isEntrance = false;
    public WIndowPositioner[] windowPositioners;
    // Start is called before the first frame update

    void Start()
    {
        foreach(WIndowPositioner w in windowPositioners)
        {

            w.GenerateWindows();

        }




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
        Debug.Log("# meshes in building mesh");
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
    }

// Update is called once per frame
void Update()
    {
        
    }
}
