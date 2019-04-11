using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTest : MonoBehaviour
{

    float critChance = 0.7f;

    // Start is called before the first frame update
    void Start()
    {
        TestRandom();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TestRandom()
    {
        for (int i = 0; i < 100; i++)
        {
            bool isCrit = Random.value > critChance;
            Debug.Log("Critou: " + isCrit);
        }
    }
}
