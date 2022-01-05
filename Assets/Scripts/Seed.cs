using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    public int seed = 100000000;


    //Output a 9 int array. //TODO: overload with int
    //MOVE TO SEPERATE FILE
    public int[] GetSeed()
    {
        int[] _gSeed = SeedToIntArr(seed);
        int[] _lseed;
        //unassigned digits are given 0 as a digit.
        if (_gSeed.Length == 9)
        {
            _lseed = _gSeed;

        }
        else
        {
            _lseed = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            _gSeed.CopyTo(_lseed, 0);
        }

        return _lseed;
    }
    public int[] SeedToIntArr(int inputSeed)
    {
        int input = inputSeed;
        int[] seedArray = new int[9];
        int index = 0;
        while (input > 0)
        {
            int digit = input % 10;
            seedArray[index] = digit;
            index++;
            input = input / 10;
        }

        return seedArray;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
