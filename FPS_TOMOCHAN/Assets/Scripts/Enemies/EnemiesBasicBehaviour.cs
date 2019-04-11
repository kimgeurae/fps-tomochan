using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesBasicBehaviour : MonoBehaviour
{

    [SerializeField]
    float health;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

}
