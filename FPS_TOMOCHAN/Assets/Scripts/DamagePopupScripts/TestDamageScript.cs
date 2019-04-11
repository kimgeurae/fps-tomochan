using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class TestDamageScript : MonoBehaviour
{

    GameObject _player;

    private void Start()
    {
        //DamagePopup.Create(Vector3.zero + new Vector3(0f, 0f, 10f), 300);  
        _player = GameObject.FindGameObjectWithTag("PlayerAlt");
    }

    private void Update()
    {
        //TestMethod();
    }

    private void TestMethod()
    {
        if (Input.GetMouseButtonDown(0))
        {
            bool isCrit = Random.value < 0.3f;
            DamagePopup.Create(_player.transform.position, 300, isCrit);
        }
    }

}
