using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    public GeneratorManager manager;

    public int gridResolution = 8;

    private int _transitionDistance = 2;


    public GameObject Terrain_Ground;
    public GameObject Terrain_Road;
    public GameObject Terrain_Road_T_Junction;
    public GameObject Terrain_Road_Cross;
    public GameObject Terrain_Building;
    private Vector3 TerrainPlaneSize;
    
    [SerializeField]
    private TerrainType[,] _TerrainTypeArray;
    public Vector3[,] _terrainTileLocations;

    private int _TerrainFeatureCount = 4;

    /// <summary>
    /// \\Input a 9 digit seed. Unplaced digits will be assigned 0.
    /// </summary>
    private int[] _gSeed;

    private Seed seedObject;



    void Awake()
    {
        seedObject = FindObjectOfType<Seed>();
        _gSeed = seedObject.GetSeed();
    }

    /// <summary>
    /// Returns the world space coordinates of all terrains of type.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public List<Vector3> GetAllLocationsOfType(TerrainType t)
    {
        List<Vector3> locs = new List<Vector3>();
        int _counterX = 0;
        int _counterZ = 0;

        while (_counterX < gridResolution)
        {
            while (_counterZ < gridResolution)
            {

                if(_TerrainTypeArray[_counterX,_counterZ] == t)
                {
                    locs.Add(_terrainTileLocations[_counterX, _counterZ]);

                }
                _counterZ++;
            }
            _counterZ = 0;
            _counterX++;
        }
        return (locs);
    }

    /// <summary>
    /// Returns the world space coordinates and the rotation of the tile to face the road.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public(List<Vector3>,List<Vector4>) GetAllLocationsAndRotationsOfType(TerrainType t)
    {
        List<Vector3> locs = GetAllLocationsOfType(t);
        List<Vector4> rots = new List<Vector4>();
        int _counterX = 0;
        int _counterZ = 0;

        while (_counterX < gridResolution)
        {
            while (_counterZ < gridResolution)
            {

                if (_TerrainTypeArray[_counterX, _counterZ] == t)
                {
                    rots.Add(FindRoadRotation(_counterX, _counterZ));
                }
                _counterZ++;
            }
            _counterZ = 0;
            _counterX++;
        }
        return (locs, rots);
    }

    
    public List<BuildingTileInfo> GetAllBuildingTileInfo()
    {
        List<BuildingTileInfo> bTI = new List<BuildingTileInfo>();
        int _counterX = 0;
        int _counterZ = 0;

        while (_counterX < gridResolution)
        {
            while (_counterZ < gridResolution)
            {

                if (_TerrainTypeArray[_counterX, _counterZ] == TerrainType.Building)
                {
                    Vector3 pos = _terrainTileLocations[_counterX, _counterZ];

                    Vector4 rot = FindRoadRotation(_counterX, _counterZ);

                    (int, int) gridPos = (_counterX, _counterZ);

                    bTI.Add(new BuildingTileInfo(gridPos, pos, rot));

                }
                _counterZ++;
            }
            _counterZ = 0;
            _counterX++;
        }
        return bTI;
    }

    /// <summary>
    /// creates a terrain at the origin of the scene
    /// </summary>
    public void CreateTerrain()
    {
        CreateTerrainArray();

        _DistributeRoads(_gSeed);
        _DistributeBuldingTerrain();

        Debug.Log(_TESTTerrainType(_TerrainTypeArray));

        _PlaceTerrain();
    }
    private void Update()
    {
    }

    /// <summary>
    /// Create a blank TerrainType array and TerrainTileLocation array.
    /// First step to creating terrain of scene
    /// </summary>
    private void CreateTerrainArray()
    {
        //Find size of Terrain prefab
        TerrainPlaneSize = Terrain_Ground.GetComponent<MeshRenderer>().bounds.size;

        //Set all in array to 
        _TerrainTypeArray = new TerrainType[gridResolution, gridResolution];
        _terrainTileLocations = new Vector3[gridResolution, gridResolution];


        //Set all terrain sections to base level (ground)
        int _counterX = 0;
        int _counterY = 0;
        while (_counterX < gridResolution)
        {
            while (_counterY < gridResolution)
            {

                _TerrainTypeArray[_counterX, _counterY] = TerrainType.Ground;
                _terrainTileLocations[_counterX, _counterY] = gameObject.transform.position + new Vector3(_counterX * TerrainPlaneSize.x, 0, _counterY * TerrainPlaneSize.z);

                _counterY++;
            }
            _counterX++;
            _counterY = 0;
        }
    }
  


    private void _DistributeRoads(int[] seed)
    {
        int[] _lseed = seed;
        //First Placement for Main Road

        //Prevent edges from spawning main road
        int firstX = (int)Mathf.PingPong((float)_lseed[0], (float)gridResolution - 8);
        firstX += gridResolution / 3;

        int firstZ = (int)Mathf.PingPong((float)_lseed[1], (float)gridResolution - 8);
        firstZ += gridResolution / 3;

        _TerrainTypeArray[firstX, firstZ] = TerrainType.Road;

        bool horizontal;
        //Designate a second road chunk above or below the first. 
        if (firstZ % 2 == 0)
        {
            _TerrainTypeArray[firstX + 1, firstZ] = TerrainType.Road;
            horizontal = false;
        }
        else
        {
            _TerrainTypeArray[firstX, firstZ - 1] = TerrainType.Road;
            horizontal = true;
        }

        //Spread the road into the nearby chunks
        ExtendRoads(1);

        //Distribute Junctions
        DistributeJunctions(_lseed, firstX, firstZ, horizontal);


    }
    //Place additional road points above or beside the main road. DistributeRoads called within this function
    private void DistributeJunctions(int[] _lseed, int firstX, int firstZ, bool horizontal)
    {
        int numberOfJunctions = (_lseed[2] + 3) % 10;
        numberOfJunctions = Mathf.Clamp(numberOfJunctions, 2, (int)(0.2*gridResolution));
        int counter = 1;
        while (counter <= numberOfJunctions)
        {


            float jp = ((counter * (float)gridResolution) / numberOfJunctions);
            int junctionPosition = (int)jp;
            if (horizontal)
            {
                if (_chunkIsValid(firstX - 1, junctionPosition)) { _TerrainTypeArray[firstX - 1, junctionPosition] = TerrainType.Road; }

            }
            //If main road is virtically positioned
            else
            {
                if (_chunkIsValid(junctionPosition, firstZ - 1)) { _TerrainTypeArray[junctionPosition, firstZ - 1] = TerrainType.Road; }
            }
            counter++;
        }

        ExtendRoads(1);
    }

    //Extends ALL roads from the _TerrainTypeArray
    private void ExtendRoads(int number)
    {
        int _counterX = 0;
        int _counterZ = 0;

        while (_counterX < gridResolution)
        {
            while (_counterZ < gridResolution)
            {
                _ExtendRoad(_counterX, _counterZ);

                _counterZ++;
            }
            _counterX++;
            _counterZ = 0;
        }
    }

    private void _ExtendRoad(int _counterX, int _counterZ)
    {
        //if the current chunk is a road and next to a road
        if (_TerrainTypeArray[_counterX, _counterZ] == TerrainType.Road)
        {
            if (CountTilesOfSurroundingType(TerrainType.Road, _counterX, _counterZ) == 1)
            {

                //returns the chunk opposite the chunk with the type of "Road"
                int[] nextRoadPlacement = _LocationOfTerrainTypeNextTo(TerrainType.Road, _counterX, _counterZ, true);

                if (_TerrainTypeArray[nextRoadPlacement[0], nextRoadPlacement[1]] != TerrainType.Road)
                {
                    _TerrainTypeArray[nextRoadPlacement[0], nextRoadPlacement[1]] = TerrainType.Road;
                    _ExtendRoad(nextRoadPlacement[0], nextRoadPlacement[1]);
                }
            }
            if (CountTilesOfSurroundingType(TerrainType.Road, _counterX, _counterZ) == 3)
            {
                //Turns some junctions into a cross junction
                if (((_counterX + _counterZ) % 3) == 0) { CreateCrossJunction(_counterX, _counterZ); }

            }

        }
    }

    private void CreateCrossJunction(int _counterX, int _counterZ)
    {
        _TerrainTypeArray[_counterX - 1, _counterZ] = TerrainType.Road;
        _TerrainTypeArray[_counterX + 1, _counterZ] = TerrainType.Road;
        _TerrainTypeArray[_counterX, _counterZ - 1] = TerrainType.Road;
        _TerrainTypeArray[_counterX, _counterZ + 1] = TerrainType.Road;
    }

    private void _DistributeBuldingTerrain()
    {
        int _counterX = 0;
        int _counterZ = 0;

        while (_counterX < gridResolution)
        {
            while (_counterZ < gridResolution)
            {

                if (_TerrainTypeArray[_counterX, _counterZ] == TerrainType.Ground)
                {
                    if (_NextToTerrainType(TerrainType.Road, _counterX, _counterZ))
                    {
                        _TerrainTypeArray[_counterX, _counterZ] = TerrainType.Building;
                    }

                }

                _counterZ++;


            }

            _counterX++;
            _counterZ = 0;
        }

    }

    /// <summary>
    /// Returns true if a terrain chunk (x,z) is DIRECTLY next to a terrian chunk of the passed in type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="xLoc"></param>
    /// <param name="Zloc"></param>
    private bool _NextToTerrainType(TerrainType _tType, int xLoc, int zLoc)
    {
        //Analyse chunks around desired chunk
        foreach (int xoffset in new int[] { -1, 0, 1 })
        {
            foreach (int zoffset in new int[] { -1, 0, 1 })
            {
                //only directly next to analysed chunk (not diagonal or selfj)
                if (xoffset == 0 ^ zoffset == 0)
                {
                    //Cannot anlayse chunks which do not exist
                    if (_chunkIsValid(xLoc + xoffset, zLoc + zoffset))
                    {
                        if (_TerrainTypeArray[xLoc + xoffset, zLoc + zoffset] == _tType)
                        {
                            return true;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Returns the relative location of the first terrain object of desired type in a 1X2 array [x,z]. 
    /// _oppositeSide will flip the coordinates to give exact opposite relative location.
    /// Returns inputted location if none of desired type found
    /// </summary>
    /// <param name="_tType"></param>
    /// <param name="xLoc"></param>
    /// <param name="zLoc"></param>
    /// <param name="opositeSide"></param>
    /// <returns></returns>
    private int[] _LocationOfTerrainTypeNextTo(TerrainType _tType, int xLoc, int zLoc, bool _oppositeSide = false)
    {
        //Analyse chunks around desired chunk
        foreach (int xoffset in new int[] { -1, 0, 1 })
        {
            foreach (int zoffset in new int[] { -1, 0, 1 })
            {
                //only directly next to analysed chunk (not diagonal or self)
                if (xoffset == 0 ^ zoffset == 0)
                {
                    //Cannot anlayse chunks which do not exist
                    if (_chunkIsValid(xLoc + xoffset, zLoc + zoffset))
                    {
                        if (_TerrainTypeArray[xLoc + xoffset, zLoc + zoffset] == _tType)
                        {
                            if (_oppositeSide)
                            {
                                //if opposite chunk is valid
                                if (_chunkIsValid((xLoc + (xoffset * -1)), (zLoc + (zoffset * -1))))
                                {
                                    int[] ret = { (xLoc + (xoffset * -1)), (zLoc + (zoffset * -1)) };
                                    return ret;
                                }
                                else { continue; }
                            }
                            else
                            {
                                int[] ret = { xLoc + xoffset, zLoc + zoffset };
                                return ret;
                            }
                        }
                        //if terrain is not equal to desired type
                        else
                        {
                            continue;
                        }
                    }

                }



            }
        }
        //if there are none of desired type around
        int[] defaultRet = { xLoc, zLoc };
        return defaultRet;
    }

    private int[] RelativeLocationOfTerrainTypeNextTo(TerrainType _tType, int xLoc, int zLoc)
    {
        int[] relativePosition = _LocationOfTerrainTypeNextTo(_tType, xLoc, zLoc);

        //subtract analysed location from the location of desired terrain type to find relative location
        relativePosition[0] -= xLoc;
        relativePosition[1] -= zLoc;
        return relativePosition;
    }


    //Returns the number of tiles DIRECTLY next to specifed tile that are of type _tType
    public int CountTilesOfSurroundingType(TerrainType _tType, int xLoc, int zLoc)
    {
        int countFound = 0;
        //Analyse chunks around desired chunk
        foreach (int xoffset in new int[] { -1, 0, 1 })
        {
            foreach (int zoffset in new int[] { -1, 0, 1 })
            {
                //only directly next to analysed chunk (not diagonal or self)
                if (xoffset == 0 ^ zoffset == 0)
                {
                    if (_chunkIsValid(xLoc + xoffset, zLoc + zoffset) && _TerrainTypeArray[xLoc + xoffset, zLoc + zoffset] == _tType)
                    {
                        countFound++;

                    }
                }
            }
        }

        return countFound;
    }
    /// <summary>
    /// Returns true if the position given corresponds to an existing chunk
    /// </summary>
    /// <param name="xpos"></param>
    /// <param name="zpos"></param>
    /// <returns></returns>
    private bool _chunkIsValid(int xpos, int zpos)
    {
        if (xpos >= 0 && xpos < gridResolution)
        {
            if (zpos >= 0 && zpos < gridResolution)
            {
                return true;
            }

        }
        return false;

    }

    /// <summary>
    /// Pushes a debug statement with grid showing terrain type of each tile
    /// </summary>
    /// <param name="TerrArray"></param>
    /// <returns></returns>
    private string _TESTTerrainType(TerrainType[,] TerrArray)
    {
        string returnString = "";
        int counter = 0;
        foreach (TerrainType t in TerrArray)
        {
            returnString += $"{t},";
            counter++;

            if (counter >= gridResolution)
            {
                returnString += "\n";
                counter = 0;
            }
        }

        return returnString;
    }

    /// <summary>
    /// Call a debug statement showing the current state of the terrain array.
    /// </summary>
    public string TestTerrainTypeArray()
    {
        return this._TESTTerrainType(_TerrainTypeArray);
    }

    /// <summary>
    /// generates terrain into level. Uses _TerrainTypeArray
    /// </summary>
    private void _PlaceTerrain()
    {
        int _counterX = 0;
        int _counterZ = 0;
        Vector3 unitSize = TerrainPlaneSize;
        unitSize.y = 0;


        while (_counterX < gridResolution)
        {
            while (_counterZ < gridResolution)
            {

                //Set location of terrain piece
                Vector3 origin = this.transform.position;

                Vector3 loc = origin + new Vector3(unitSize.x * _counterX, 0, unitSize.x * _counterZ);
                GameObject newTile;
                //Change height based on type of terrain
                if (_TerrainTypeArray[_counterX, _counterZ] == TerrainType.Ground) { newTile = Instantiate(Terrain_Ground, loc, Quaternion.Euler(0, 0, 0), this.transform); }
                else if (_TerrainTypeArray[_counterX, _counterZ] == TerrainType.Road)
                { newTile = RoadTileInstancing(_counterX, _counterZ, loc); }
                else if (_TerrainTypeArray[_counterX, _counterZ] == TerrainType.Building) { newTile = Instantiate(Terrain_Building, loc, Quaternion.Euler(0, 0, 0), this.transform); }
                else
                {
                    newTile = Instantiate(Terrain_Ground, loc, Quaternion.Euler(0, 0, 0));
                }
                
                HL_TerrainTile tilecomp = newTile.GetComponent<HL_TerrainTile>();
                tilecomp.positionX = _counterX;
                tilecomp.positionZ = _counterZ;

                _counterZ++;
            }

            _counterX++;
            _counterZ = 0;
        }
        // Update is called once per frame

    }

    private GameObject RoadTileInstancing(int _counterX, int _counterZ, Vector3 loc)
    {
        int surroundingRoadCount = CountTilesOfSurroundingType(TerrainType.Road, _counterX, _counterZ);

        GameObject tile = null;
        if (surroundingRoadCount == 2 || surroundingRoadCount == 1)
        {
            //rotate the road based on surrounding pieces
            Vector3 rotation = FindRoadRotation(_counterX, _counterZ);
           tile = Instantiate(Terrain_Road, loc, Quaternion.Euler(0, rotation.y, 0), this.transform);
        }
        else if (surroundingRoadCount == 3)
        {
            Vector3 rotation = FindRoadRotation(_counterX, _counterZ);
           tile =  Instantiate(Terrain_Road_T_Junction, loc, Quaternion.Euler(0, rotation.y, 0), this.transform);
        }
        else if (surroundingRoadCount == 4)
        {
            tile = Instantiate(Terrain_Road_Cross, loc, Quaternion.Euler(0, 0, 0), this.transform);
        }
        return tile;
    }


    private Vector4 _T_Junction_Rotation(int xLoc, int zLoc)
    {

        //Determines which of the surrounding tiles is not a road and rotates T-junction accordingly
        int[] relPos = RelativeLocationOfTerrainTypeNextTo(TerrainType.Road, xLoc, zLoc);
        switch (relPos[0])
        {
            case 1:
                return RotationsToTile[1];
            case -1:
                return RotationsToTile[3];

            //no space in x direction
            case 0:
                switch (relPos[1])
                {
                    case 1:
                        return RotationsToTile[2];
                    case -1:
                        return RotationsToTile[0];
                    default:
                        Debug.LogError("FindRoadRotation produced a falure state for 1 or 2 road tiles nearby.");
                        return new Vector4(0, 90, 0, 0);
                }

            default:
                {
                    Debug.LogError("FindRoadRotation produced a falure state for 1 or 2 road tiles nearby.");
                    return new Vector4(0, 90, 0, 0);
                }
        }
    }

    /// <summary>
    /// Finds the rotation needed to make the current tile face the road tile next to it. 
    /// </summary>
    /// <param name="xLoc"></param>
    /// <param name="zLoc"></param>
    /// <returns></returns>
    /// 

    Vector4[] RotationsToTile = new Vector4[] { new Vector4(0, 0, 0, 0), new Vector4(0, -90, 0, 0), new Vector4(0, 180, 0, 0), new Vector4(0, 90, 0, 0) };


    public Vector4 FindRoadRotation(int xLoc, int zLoc)
    {
        //Find count of road tiles around
        int countOfRoadTiles = CountTilesOfSurroundingType(TerrainType.Road, xLoc, zLoc);

        //if the road is part of a straight
        if (countOfRoadTiles == 2 || countOfRoadTiles == 1)
        {
            //Tile is above/below 
            int[] relPos = RelativeLocationOfTerrainTypeNextTo(TerrainType.Road, xLoc, zLoc);

            //nested switch case. If the relative position is 0 in x the z coordinate is used to determine the desired rotation.
            //both default cases are error outputs.
            switch(relPos[0])
            {
                case 1:
                    return RotationsToTile[0];
                case -1:
                    return RotationsToTile[2];

                    //no space in x direction
                case 0:
                    switch(relPos[1])
                    {
                        case 1:
                            return RotationsToTile[1];
                        case -1:
                            return RotationsToTile[3];
                        default:
                            Debug.LogError("FindRoadRotation produced a falure state for 1 or 2 road tiles nearby.");
                            return new Vector4(0, 90, 0, 0);
                    }

                default:
                    {
                        Debug.LogError("FindRoadRotation produced a falure state for 1 or 2 road tiles nearby.");
                        return new Vector4(0, 90, 0, 0);
                    }
            }
        }
        if (countOfRoadTiles == 3)
        {
           return( _T_Junction_Rotation(xLoc, zLoc));

        }
        else
        {
            return new Vector4(0, 0, 0, 0);
        }
    }

}
