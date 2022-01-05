using UnityEngine;

public struct BuildingTileInfo
{
    public (int, int) gridLocation;
    public Vector3 position;
    public Vector4 rotation;

    public BuildingTileInfo((int, int) gridLoc, Vector3 pos, Vector4 rot)
    {
        this.gridLocation = gridLoc;
        this.position = pos;
        this.rotation = rot;
    }
}
