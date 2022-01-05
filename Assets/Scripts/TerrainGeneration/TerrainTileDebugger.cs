using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(HL_TerrainTile))]
public class TerrainTileDebugger : MonoBehaviour
{
    HL_TerrainTile _myTile;
    TerrainGeneration tg;
    private Vector4 rotToRoad;
    private void Start()
    {
        _myTile = GetComponent<HL_TerrainTile>();
        tg = FindObjectOfType<TerrainGeneration>();
       rotToRoad = tg.FindRoadRotation(_myTile.positionX, _myTile.positionZ);
    }
    // Start is called before the first frame update
    private void OnDrawGizmosSelected()
    {
        Handles.Label(transform.position, ($"({_myTile.positionX}, {_myTile.positionZ})"));
          //  $"\n Rotation To nearest road = {rotToRoad}"); ;);
    }
}
