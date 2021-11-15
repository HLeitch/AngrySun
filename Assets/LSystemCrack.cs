using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystemCrack : MonoBehaviour
{
    public Transform propergatePoint;

    public LSystemCrack parent = null;
    public CrackGenerator _gGenerator;

    public bool _canBeResized = true;

    [SerializeField]
    private MeshCollider _mCollider;

    [SerializeField]
    protected GameObject _crack;

    public float _propergationAngleMax = 50f;

    public float TerminationChance = 0.25f;

    public Transform _myScale;

    public float length { get { return Mathf.Abs(propergatePoint.localPosition.magnitude); } }

    public int Level;
    public Mesh _mMesh;

    private MeshRenderer _mMeshRenderer;


    // Start is called before the first frame update
    void Start()
    {
      /*  _mMesh = gameObject.GetComponentInChildren<MeshFilter>().mesh;

        _mMeshRenderer = this.GetComponent<MeshRenderer>();
        vertices = _mMesh.vertices;*/



      

    }

    

protected void Propergate()
    {
        //Decides if branch is terminated or spawned
        float roll = Random.Range(0.0f, 1.0f);
        if ((roll < TerminationChance * Level))
        {

            Debug.Log("Leaf branch");
        }

        else
        {
            float extraAngle = (100 * roll) % (5*(Level+1));


            float angleProp = 45f; //+ extraAngle;
            //spawns new branches at equal opposing angles
            spawnBranch(angleProp);
            spawnBranch(-angleProp);
        }
    }

    public void BeginPropergation()
    {
            Propergate();
    }

    void spawnBranch(float angle)
    {
     
        GameObject newCrack =  Instantiate(_crack,this.transform);

        newCrack.transform.position = propergatePoint.position;
        newCrack.transform.Rotate(0, angle, 0);

        LSystemCrack newCrackData = newCrack.GetComponent<LSystemCrack>();

        newCrackData.Level = this.Level + 1;
        newCrackData._gGenerator = this._gGenerator;
        newCrackData.parent = this;
        newCrackData._crack = this._crack;
    }
    


    // Update is called once per frame
    void Update()
    {

    }


    private void Resize(float xScale)
    {



    }

    //This should be called before the resumption of the CrackGenerator Coroutine

    private void OnTriggerEnter(Collider other)

    {
        LSystemCrack otherCrack;
        other.gameObject.TryGetComponent<LSystemCrack>(out otherCrack);

        if (_canBeResized)
        {
            if (otherCrack == true)
            {
                otherCrack._canBeResized = false;

                //find the nearest point on the line to the propergation point(end of the line)
                Vector3 closestPoint = other.ClosestPoint(propergatePoint.position);

                //find distance between this and the propergation point
                float distance = (closestPoint - propergatePoint.position).magnitude;

                //Find the percentage of length between the origin of the crack and the PropergatePoint
                //and the origin and the closestpoint
                float percDiff = distance / length;


                //change the value of the local x-scale such that the crack ends where it meets the collided crack
                _myScale.localScale.Set(1 - percDiff, 1f, 1f);

            }

        }
    }



}
