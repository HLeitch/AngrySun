using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraOnTrack : MonoBehaviour
{
    public float dollyCartSpeed = 0.5f;
    public float holdTime = 5.0f;
    public float stdHeight = 1.6f;

    public CinemachineVirtualCamera cam;
    private CinemachineTrackedDolly dolly;
    public CinemachineSmoothPath track;
    public Transform lookAtTarget;

    public bool activeCamera = false;


    private List<CinemachineSmoothPath.Waypoint> _trackNodeList = new List<CinemachineSmoothPath.Waypoint>();
    // Start is called before the first frame update
    void Start()
    {
        dolly = cam.GetCinemachineComponent<CinemachineTrackedDolly>();
        dolly.m_PositionUnits = CinemachinePathBase.PositionUnits.Distance;
        lookAtTarget.position = _trackNodeList[0].position;

        StartCoroutine(MoveCamera());

    }

    // Update is called once per frame
    void Update()
    {




    }

    public void MakeActiveCamera()
    {

        activeCamera = true;
    }

    /// <summary>
    /// Adds a node to the smoothpath the camera follows. if stdHeight is false the yvalue from the vector will be used.
    /// </summary>
    /// <param name="newNodePos"></param>
    /// <param name="stdHeight"></param>
    public void addTrackNode(Vector3 newNodePos, bool stdHeight = true)
    {
        CinemachineSmoothPath.Waypoint newWaypoint = new CinemachineSmoothPath.Waypoint();
        newWaypoint.position = newNodePos;
        _trackNodeList.Add(newWaypoint);

        track.m_Waypoints = _trackNodeList.ToArray();
    }
    /// <summary>
    ///  Adds multiple nodes to the smoothpath the camera follows. if stdHeight is false the yvalue from the vector will be used.
    /// </summary>
    /// <param name="newNodes"></param>
    public void addTrackNodes(Vector3[] newNodes, bool _bStdHeight = true)
    {
        if (newNodes is null)
        {
            throw new System.ArgumentNullException(nameof(newNodes));
        }

        foreach (Vector3 nodePos in newNodes)
        {
            Vector3 _nodeEditable = nodePos;
            if (_bStdHeight) { _nodeEditable.y = stdHeight; }
            CinemachineSmoothPath.Waypoint newWaypoint = new CinemachineSmoothPath.Waypoint();
            newWaypoint.position = _nodeEditable;
            _trackNodeList.Add(newWaypoint);
        }
        track.m_Waypoints = _trackNodeList.ToArray();

        Debug.Log($"number of points = {_trackNodeList.Count}");
    }

    /// <summary>
    /// Returns after "time" seconds in real time
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator HoldPosition(float time)
    {
        Debug.Log($"Camera Hold for {time}s");

        yield return new WaitForSecondsRealtime(time);
        dolly.m_PathPosition = 0;

    }
    IEnumerator MoveCamera()
    {
        while (activeCamera)
        {
            //move dolly across position
            dolly.m_PathPosition += (dollyCartSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            if (dolly.m_PathPosition >= dolly.m_Path.PathLength)
            {
                StartCoroutine(HoldPosition(holdTime));
                activeCamera = false;

            }
        }
    }
}
