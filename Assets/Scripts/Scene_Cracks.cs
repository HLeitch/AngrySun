using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene_Cracks : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] CrackLibrary;

    public Transform[] Positions;



    TerrainGeneration tg;
    int[] _lseed { get { return tg.GetSeed(); } }

    void Start()
    {
       // placeCracks();
    }
    void placeCracks()
    {
        int counter = 0;
        foreach(Transform t in Positions)
        {
            int chosenCrack = 3;
            
            Instantiate(CrackLibrary[chosenCrack], t);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
