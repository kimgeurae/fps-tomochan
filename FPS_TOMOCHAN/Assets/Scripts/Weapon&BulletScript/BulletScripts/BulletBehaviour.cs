using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField]
    int bulletMinDmg;
    [SerializeField]
    int bulletMaxDmg;
    [SerializeField]
    int bulletMinCritDmg;
    [SerializeField]
    int bulletMaxCritDmg;
    [SerializeField]
    float bulletSpeed;
    [SerializeField]
    float destroyTime;
    [Range(0.0f, 1.0f)]
    public float critChance;
    Vector3 offset = new Vector3(0f, 1f, 0f);
    public GameObject _bulletHole;

    // Start is called before the first frame update
    void Start()
    {
        SelfDestruction();
    }

    // Update is called once per frame
    void Update()
    {
        BulletMovement();
    }

    void SelfDestruction()
    {
        Destroy(this.gameObject, destroyTime);
    }

    void BulletMovement()
    {
        transform.Translate(new Vector3(0f, 0f, bulletSpeed) * Time.deltaTime);
    }

    void BulletHole()
    {
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, 100f))
        {
            Debug.Log(hit);
            if (hit.transform.gameObject.CompareTag("Wall"))
            {
                Instantiate(_bulletHole, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemies"))
        {
            bool isCrit = Random.value < critChance;
            if (isCrit)
            {
                int cdmg = Random.Range(bulletMinCritDmg, bulletMaxCritDmg);
                other.gameObject.GetComponent<EnemiesBasicBehaviour>().ApplyDamage(cdmg);
                Debug.Log("Enemy has been hitted with a Critical Hit!!! And Received a damage of: " + cdmg);
                DamagePopup.Create(other.transform.position + offset, cdmg, true);
                Destroy(this.gameObject);
            }
            else
            {
                int ndmg = Random.Range(bulletMinDmg, bulletMaxDmg);
                other.gameObject.GetComponent<EnemiesBasicBehaviour>().ApplyDamage(ndmg);
                Debug.Log("Enemy has been hitted with a Normal Hit! And Received the damage of: " + ndmg);
                DamagePopup.Create(other.transform.position + offset, ndmg, false);
                Destroy(this.gameObject);
            }
        }
        if (other.gameObject.CompareTag("Wall"))
        {
            BulletHole();
            Debug.Log("Acerto");
            Destroy(this.gameObject);
        }
    }
}
