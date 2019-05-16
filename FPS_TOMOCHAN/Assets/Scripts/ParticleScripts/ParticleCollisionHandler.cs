using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionHandler : MonoBehaviour
{

    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;
    private float applyDamageCooldown = 2f;
    private float applyDamageCount = 4f;


    // Start is called before the first frame update
    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        applyDamageCount += Time.deltaTime;
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (applyDamageCount > applyDamageCooldown) {
                other.gameObject.GetComponent<PlayerBehaviour>().RemoveHealth(10);
                applyDamageCount = 0;
            }
        }
    }

}
