using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField]
    float bulletSpeed;
    [SerializeField]
    float destroyTime;
    public GameObject _bulletHole;
    public GameObject _bloodbulletHole;

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

    // Formula i have to use distance/velocity = time.
    // distance = hit.distance, velocity = bulletSpeed & time = x.
    public void BulletHole(RaycastHit hit)
    {
        if (hit.transform.gameObject.CompareTag("Wall"))
        {
            Instantiate(_bulletHole, hit.point, Quaternion.FromToRotation(Vector3.back, hit.normal));
        }
        if (hit.transform.gameObject.CompareTag("Enemies"))
        {
            Instantiate(_bloodbulletHole, hit.point, Quaternion.FromToRotation(Vector3.back, hit.normal));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemies"))
        {
            Destroy(this.gameObject);
        }
        if (other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Acerto");
            Destroy(this.gameObject);
        }
        if (other.gameObject.CompareTag("Target"))
        {
            Destroy(this.gameObject);
        }
    }
}
