using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{

    public enum Type { health, ammo, key, };
    public Type type;

    public int amount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        switch(type)
        {
            case Type.ammo:
                if (other.CompareTag("Player"))
                {
                    other.transform.GetChild(1).transform.GetChild(1).transform.GetChild(0).GetComponent<BasicWeaponBehaviour>().AddAmmo(amount);
                    Destroy(this.gameObject);
                }
                break;
            case Type.health:
                if (other.CompareTag("Player"))
                {
                    other.GetComponent<PlayerBehaviour>().AddHealth(amount);
                    Destroy(this.gameObject);
                }
                break;
            case Type.key:

                break;
        }
    }

}
