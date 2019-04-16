using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float bulletSpeed = 100f;
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
        Destroy(this.gameObject, 10f);
        /*        
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100f))
        {
            Destroy(this.gameObject, hit.distance / bulletSpeed);
        }
        else
        {
            Destroy(this.gameObject, destroyTime);
        }
        */
    }

    void BulletMovement()
    {
        transform.Translate(new Vector3(0f, 0f, bulletSpeed) * Time.deltaTime);
    }

    // Formula i have to use distance/velocity = time.
    // distance = hit.distance, velocity = bulletSpeed & time = x.
    public void BulletHole(RaycastHit hit)
    {
        //StopCoroutine("CallBulletHole");
        IEnumerator coroutine = CallBulletHole(hit);
        StartCoroutine(coroutine);
    }

    IEnumerator CallBulletHole(RaycastHit hit)
    {
        yield return new WaitForSeconds(hit.distance/bulletSpeed);
        if (hit.transform.gameObject.CompareTag("Target"))
        {
            //Instantiate(_bulletHole, hit.point, Quaternion.FromToRotation(Vector3.back, hit.normal));
            //var myObj = Instantiate(_bulletHole, hit.point, Quaternion.FromToRotation(Vector3.back, hit.normal));
            Instantiate(_bulletHole, hit.point, Quaternion.FromToRotation(Vector3.back, hit.normal), hit.transform);
            Debug.Log("Bullet Hole was sucefully generate");
            //myObj.transform.parent = hit.transform;
            if (hit.transform.gameObject.CompareTag("Target"))
            {
                hit.transform.gameObject.GetComponent<TargetScript>().SetTargetDown();
                //myObj.transform.parent = hit.transform;
            }
            Destroy(this.gameObject, hit.distance / bulletSpeed);
        }
        if (hit.transform.gameObject.CompareTag("Wall"))
        {
            Instantiate(_bulletHole, hit.point, Quaternion.FromToRotation(Vector3.back, hit.normal));
            Destroy(this.gameObject, hit.distance / bulletSpeed);
        }
        if (hit.transform.gameObject.CompareTag("Enemies"))
        {
            Instantiate(_bloodbulletHole, hit.point, Quaternion.FromToRotation(Vector3.back, hit.normal), hit.transform);
            Destroy(this.gameObject, hit.distance / bulletSpeed);
            Debug.Log("Blood Hole was sucefully generate");
        }
    }

}
