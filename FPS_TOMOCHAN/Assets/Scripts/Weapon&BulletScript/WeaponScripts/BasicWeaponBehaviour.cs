using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BasicWeaponBehaviour : MonoBehaviour
{
    [SerializeField]
    Transform bulletSpawnPoint;
    [SerializeField]
    GameObject _bullet;
    [SerializeField]
    Text _weaponModeGUI;
    public Camera _fpscam;
    public Transform _notarget;
    [SerializeField]
    int bulletMinDmg;
    [SerializeField]
    int bulletMaxDmg;
    [SerializeField]
    int bulletMinCritDmg;
    [SerializeField]
    int bulletMaxCritDmg;
    [Range(0.0f, 1.0f)]
    public float critChance;
    [SerializeField]
    float verticalRecoil = 2f;
    [SerializeField]
    float horizontalRecoil = 1f;
    Vector3 offset = new Vector3(0f, 1f, 0f);
    public enum WeaponMode
    {
        Semi,
        Auto,
        Burst,
    }
    public WeaponMode weaponMode;
    [SerializeField]
    float fireRate;
    float nextTimeToFire = 0f;
    bool semiCanShoot = true;

    public Image _leftCrosshair;
    public Image _rightCrosshair;
    public Image _topCrosshair;
    public Image _botCrosshair;
    private Vector2 topPos;
    private Vector2 botPos;
    private Vector2 leftPos;
    private Vector2 rightPos;

    public float crosshairSpreadAmount = 30f;
    public float crosshairSpreadDuration = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        weaponMode = WeaponMode.Semi;

        topPos = _topCrosshair.rectTransform.position;
        botPos = _botCrosshair.rectTransform.position;
        leftPos = _leftCrosshair.rectTransform.position;
        rightPos = _rightCrosshair.rectTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        PointGunToCenter();
        UpdateGUI();
        if (Input.GetButtonUp("Fire1"))
        {
            semiCanShoot = true;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (weaponMode == WeaponMode.Semi)
                weaponMode = WeaponMode.Auto;
            else if (weaponMode == WeaponMode.Auto)
                weaponMode = WeaponMode.Semi;
        }
    }

    void UpdateGUI()
    {
        _weaponModeGUI.text = weaponMode.ToString();
    }

    public void Shoot()
    {
        switch (weaponMode)
        {
            case WeaponMode.Semi:
                if (semiCanShoot)
                {
                    SemiModeShoot();
                    semiCanShoot = false;
                }
                break;
            case WeaponMode.Auto:
                if (Time.time >= nextTimeToFire)
                {
                    nextTimeToFire = Time.time + 1f / fireRate;
                    SemiModeShoot();
                }
                break;
            case WeaponMode.Burst:
                break;
        }
    }

    void SemiModeShoot()
    {
        RaycastHit hit;
        Debug.DrawLine(_fpscam.transform.position, _fpscam.transform.forward * 100f, Color.yellow, 2.5f);
        if (Physics.Raycast(_fpscam.transform.position, _fpscam.transform.forward, out hit, 100f))
        {
            if (hit.transform.gameObject.CompareTag("Enemies"))
            {
                bool isCrit = Random.value < critChance;
                if (isCrit)
                {
                    int cdmg = Random.Range(bulletMinCritDmg, bulletMaxCritDmg);
                    hit.transform.gameObject.GetComponent<EnemiesBasicBehaviour>().ApplyDamage(cdmg);
                    Debug.Log("Enemy has been hitted with a Critical Hit!!! And Received a damage of: " + cdmg);
                    DamagePopup.Create(hit.transform.position + offset, cdmg, true);
                }
                else
                {
                    int ndmg = Random.Range(bulletMinDmg, bulletMaxDmg);
                    hit.transform.gameObject.GetComponent<EnemiesBasicBehaviour>().ApplyDamage(ndmg);
                    Debug.Log("Enemy has been hitted with a Normal Hit! And Received the damage of: " + ndmg);
                    DamagePopup.Create(hit.transform.position + offset, ndmg, false);
                }
                _bullet.gameObject.GetComponent<BulletBehaviour>().BulletHole(hit);
            }
            if (hit.transform.gameObject.CompareTag("Wall"))
            {
                _bullet.gameObject.GetComponent<BulletBehaviour>().BulletHole(hit);
            }
            if (hit.transform.gameObject.CompareTag("Target"))
            {
                if (hit.transform.gameObject.GetComponent<TargetScript>().canGoDown)
                {
                    _bullet.gameObject.GetComponent<BulletBehaviour>().BulletHole(hit);
                    hit.transform.gameObject.GetComponent<TargetScript>().SetTargetDown();
                }
            }
        }
        SpawnBullet();
        MuzzleFlash();
        AnimateCameraFOV();
        AnimateCrosshair();
        Recoil();
    }

    private void SpawnBullet()
    {
        Instantiate(_bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }

    private void MuzzleFlash()
    {

    }

    void PointGunToCenter()
    {
        RaycastHit target;
        Debug.DrawLine(_fpscam.transform.position, _fpscam.transform.forward * 100f, Color.green, .01f);
        if (Physics.Raycast(_fpscam.transform.position, _fpscam.transform.forward, out target, 100f) && target.distance > 5f)
        {
            transform.LookAt(target.point);
        }
        else
        {
            transform.LookAt(_notarget);
            // Vector3.Scale(_fpscam.transform.position, new Vector3(1f, 1f, 100f))
        }
    }

    void AnimateCameraFOV()
    {
        StopCoroutine("CameraFOVEffect");
        StartCoroutine("CameraFOVEffect");
    }

    IEnumerator CameraFOVEffect()
    {
        _fpscam.fieldOfView = 60;
        for (int i = 0; i < 5; i++)
        {
            _fpscam.fieldOfView = _fpscam.fieldOfView - 1;
            yield return new WaitForSeconds(0.0005f);
        }
        for (int g = 0; g < 5; g++)
        {
            _fpscam.fieldOfView = _fpscam.fieldOfView + 1;
            yield return new WaitForSeconds(0.0005f);
        }
    }

    void AnimateCrosshair()
    {
        StopCoroutine("AnimateCrosshairCoroutine");
        StartCoroutine("AnimateCrosshairCoroutine");
    }

    IEnumerator AnimateCrosshairCoroutine()
    {
        // Reset to original position.
        _topCrosshair.rectTransform.position = topPos;
        _botCrosshair.rectTransform.position = botPos;
        _leftCrosshair.rectTransform.position = leftPos;
        _rightCrosshair.rectTransform.position = rightPos;

        _topCrosshair.rectTransform.position = new Vector2(_topCrosshair.rectTransform.position.x, _topCrosshair.rectTransform.position.y + crosshairSpreadAmount);
        _botCrosshair.rectTransform.position = new Vector2(_botCrosshair.rectTransform.position.x, _botCrosshair.rectTransform.position.y - crosshairSpreadAmount);
        _leftCrosshair.rectTransform.position = new Vector2(_leftCrosshair.rectTransform.position.x - crosshairSpreadAmount, _leftCrosshair.rectTransform.position.y);
        _rightCrosshair.rectTransform.position = new Vector2(_rightCrosshair.rectTransform.position.x + crosshairSpreadAmount, _rightCrosshair.rectTransform.position.y);
        yield return new WaitForSeconds(crosshairSpreadDuration);
        _topCrosshair.rectTransform.position = new Vector2(_topCrosshair.rectTransform.position.x, _topCrosshair.rectTransform.position.y - crosshairSpreadAmount);
        _botCrosshair.rectTransform.position = new Vector2(_botCrosshair.rectTransform.position.x, _botCrosshair.rectTransform.position.y + crosshairSpreadAmount);
        _leftCrosshair.rectTransform.position = new Vector2(_leftCrosshair.rectTransform.position.x + crosshairSpreadAmount, _leftCrosshair.rectTransform.position.y);
        _rightCrosshair.rectTransform.position = new Vector2(_rightCrosshair.rectTransform.position.x - crosshairSpreadAmount, _rightCrosshair.rectTransform.position.y);
        yield return null;
    }

    void Recoil()
    {
        if (weaponMode == WeaponMode.Auto || weaponMode == WeaponMode.Burst)
        {
            bool leftOrRight = Random.value > 0.5f;
            _fpscam.GetComponent<FPSCameraBehavior>().ApplyRecoil(verticalRecoil, horizontalRecoil, leftOrRight);
        }
        else
        {
            bool leftOrRight = Random.value > 0.9f;
            _fpscam.GetComponent<FPSCameraBehavior>().ApplyRecoil(verticalRecoil, verticalRecoil / 25, leftOrRight);
        }
    }

}
