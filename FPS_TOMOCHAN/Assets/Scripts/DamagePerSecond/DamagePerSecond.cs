using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePerSecond : MonoBehaviour
{
    [Tooltip("The damage that the fire does per second.")]
    public int dmgPerSecond;
    private float nextTimeToDamage;

    // Start is called before the first frame update
    void Start()
    {
        nextTimeToDamage = Time.time + 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Colidiu com algo");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colidiu com o jogador");
            if (nextTimeToDamage < Time.time)
            {
                Debug.Log("Aplicou Dano");
                other.GetComponent<PlayerBehaviour>().RemoveHealth(dmgPerSecond);
                nextTimeToDamage = Time.time + 1;
            }
        }
    }
}
