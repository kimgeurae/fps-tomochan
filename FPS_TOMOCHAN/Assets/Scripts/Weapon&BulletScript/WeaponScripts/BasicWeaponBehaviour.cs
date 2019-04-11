using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWeaponBehaviour : MonoBehaviour
{
    [SerializeField]
    Transform bulletSpawnPoint;
    [SerializeField]
    GameObject _bullet;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShootBullet()
    {
        SpawnBullet();
        MuzzleFlash();
        Recoil();
    }

    private void Recoil()
    {

    }

    private void SpawnBullet()
    {
        Instantiate(_bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }

    private void MuzzleFlash()
    {

    }

}
