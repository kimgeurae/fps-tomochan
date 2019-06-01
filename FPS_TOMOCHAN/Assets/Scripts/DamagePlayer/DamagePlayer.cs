using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{

    private float damageCooldown;
    private float damageTimer;
    public int dmg;

    // Start is called before the first frame update
    void Start()
    {
        damageTimer = 0;
        damageCooldown = .7f;
    }

    // Update is called once per frame
    void Update()
    {
        damageTimer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (damageTimer > damageCooldown)
            {
                other.gameObject.GetComponent<PlayerBehaviour>().RemoveHealth(dmg);
                damageTimer = 0;
            }
        }
    }
}
