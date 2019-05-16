using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BasicWeaponBehaviour : MonoBehaviour
{
    #region References
    [SerializeField]
    Transform bulletSpawnPoint;
    [SerializeField]
    GameObject _bullet;
    [SerializeField]
    Text _weaponModeGUI;
    public Camera _fpscam;
    public Transform _notarget;
    [SerializeField]
    Text _ammoInfo;
    [SerializeField]
    Image _hudBullet01;
    [SerializeField]
    Image _hudBullet02;
    [SerializeField]
    Text _reloadWarning;
    [SerializeField]
    Image _reloadCircle;
    Animator _anim;
    public ParticleSystem muzzleFlashParticle;
    public GameObject _impactMetal, _impactConcrete, _impactBlood;
    [SerializeField]
    GameObject _bulletPrefab;
    LayerMask ignoreRaycast;
    GameObject _player;
    #endregion
    #region Bullet damage & crit
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
    #endregion
    #region Recoil
    [SerializeField]
    float verticalRecoil = 2f;
    [SerializeField]
    float horizontalRecoil = 1f;
    #endregion
    #region Offsets
    Vector3 offset = new Vector3(0f, 1f, 0f);
    #endregion
    #region Shooting (Auto & Semi)
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
    #endregion
    #region Reload and Magazine
    [SerializeField]
    int maxAmmo = 90;
    [SerializeField]
    int maxMagazineAmmo = 30;
    int magazineAmmo;
    int actualAmmo;
    bool isReloading = false;
    [SerializeField]
    float emptyReloadTime = 1f;
    #endregion
    #region BossRestrictions
    public GameObject[] _targets;
    #endregion
    #region Sounds
    public GameObject _gunShoot;
    public AudioSource _magazineOut;
    public AudioSource _magazineIn;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //_anim = transform.GetChild(3).GetComponent<Animator>();

        weaponMode = WeaponMode.Semi;
        actualAmmo = maxAmmo;
        magazineAmmo = maxMagazineAmmo;

        ignoreRaycast = ~0 - LayerMask.GetMask("RaycastIgnore");

        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadRequest();
        }
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
        _ammoInfo.text = "Current Ammo: " + magazineAmmo.ToString() + " / " + actualAmmo.ToString();
        if (weaponMode == WeaponMode.Semi)
        {
            Color c;
            c = _hudBullet01.color;
            c.a = 0.1f;
            _hudBullet01.color = c;
            _hudBullet02.color = c;
        }
        if (weaponMode == WeaponMode.Auto)
        {
            Color c;
            c = _hudBullet01.color;
            c.a = 255;
            _hudBullet01.color = c;
            _hudBullet02.color = c;
        }
        if (magazineAmmo > 0)
        {
            _reloadWarning.enabled = false;
        }
        else
        {
            _reloadWarning.enabled = true;
        }
    }

    #region Reload Request, called when pressin "R"
    // Gives the reload time for calling the visual part of the reload in PlayerHUDBehaviour
    void ReloadRequest()
    {
        if (magazineAmmo < maxMagazineAmmo)
        {
            IEnumerator coroutine;
            float time = 0f;
            if (magazineAmmo == 0)
            {
                time = emptyReloadTime;
            }
            else
            {
                time = emptyReloadTime + magazineAmmo / 10f;
            }
            //coroutine = Reload(time);
            //StartCoroutine(coroutine);
            _magazineOut.Play();
            _player.GetComponent<PlayerHUDBehaviour>().ReloadCircleHUD(time);
        }
    }
    #endregion

    #region Reload Method Called by the PlayerHUDBehaviour
    // Executes the non visual part of the reload.
    public void ReloadWeapon()
    {
        _magazineIn.Play();
        isReloading = true;
        if (actualAmmo >= maxMagazineAmmo)
        {
            if (magazineAmmo == 0)
            {
                magazineAmmo = maxMagazineAmmo;
                actualAmmo -= maxMagazineAmmo;
                isReloading = false;
            }
            else
            {
                actualAmmo -= (maxMagazineAmmo - magazineAmmo);
                magazineAmmo = maxMagazineAmmo;
                isReloading = false;
            }
        }
        else
        {
            if (actualAmmo + magazineAmmo <= 30)
            {
                magazineAmmo = actualAmmo + magazineAmmo;
                actualAmmo = 0;
                isReloading = false;
            }
            else
            {
                int difference = magazineAmmo - maxMagazineAmmo;
                magazineAmmo = maxMagazineAmmo;
                actualAmmo += difference;
                isReloading = false;
            }
        }
    }
    #endregion

    #region Deprecated Reload Coroutine Method
    IEnumerator Reload(float t)
    {
        isReloading = true;
        yield return new WaitForSeconds(t);
        if (actualAmmo >= maxMagazineAmmo)
        {
            if (magazineAmmo == 0)
            {
                magazineAmmo = maxMagazineAmmo;
                actualAmmo -= maxMagazineAmmo;
                isReloading = false;
            }
            else
            {
                actualAmmo -= (maxMagazineAmmo - magazineAmmo);
                magazineAmmo = maxMagazineAmmo;
                isReloading = false;
            }
        }
        else
        {
            magazineAmmo = actualAmmo + magazineAmmo;
            actualAmmo = 0;
            isReloading = false;
        }
    }
    #endregion

    #region Shoot Function. Called in PlayerBehaviour
    // Shoots depending on weapon mode (Semi, Auto, Burst).
    public void Shoot()
    {
        switch (weaponMode)
        {
            case WeaponMode.Semi:
                if ((semiCanShoot && magazineAmmo > 0) && !isReloading)
                {
                    SemiModeShoot();
                    semiCanShoot = false;
                    magazineAmmo--;
                }
                break;
            case WeaponMode.Auto:
                if ((Time.time >= nextTimeToFire && magazineAmmo > 0) && !isReloading)
                {
                    nextTimeToFire = Time.time + 1f / fireRate;
                    SemiModeShoot();
                    magazineAmmo--;
                }
                break;
            case WeaponMode.Burst:
                break;
        }
    }
    #endregion

    #region The shooting itself. Does everything from visual to raycasting and seeing if it hits a valid target.
    void SemiModeShoot()
    {
        Instantiate(_gunShoot, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        muzzleFlashParticle.Play();
        RaycastHit hit;
        Debug.DrawLine(_fpscam.transform.position, _fpscam.transform.forward * 100f, Color.yellow, 2.5f);
        var spawnedBullet = Instantiate(_bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        _bullet = spawnedBullet;
        if (Physics.Raycast(_fpscam.transform.position, _fpscam.transform.forward, out hit, 100f, ignoreRaycast))
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
                CallHitImpact(hit);
            }
            if (hit.transform.gameObject.CompareTag("Boss"))
            {
                if (_targets[0].transform.GetChild(0).GetComponent<TargetScript>().state == TargetScript.State.down && _targets[1].transform.GetChild(0).GetComponent<TargetScript>().state == TargetScript.State.down && _targets[2].transform.GetChild(0).GetComponent<TargetScript>().state == TargetScript.State.down)
                {
                    bool isCrit = Random.value < critChance;
                    if (isCrit)
                    {
                        int cdmg = Random.Range(bulletMinCritDmg, bulletMaxCritDmg);
                        hit.transform.gameObject.GetComponent<BossBehaviour>().ApplyDamage(cdmg);
                        Debug.Log("Enemy has been hitted with a Critical Hit!!! And Received a damage of: " + cdmg);
                        DamagePopup.Create(hit.transform.position + offset, cdmg, true);
                    }
                    else
                    {
                        int ndmg = Random.Range(bulletMinDmg, bulletMaxDmg);
                        hit.transform.gameObject.GetComponent<BossBehaviour>().ApplyDamage(ndmg);
                        Debug.Log("Enemy has been hitted with a Normal Hit! And Received the damage of: " + ndmg);
                        DamagePopup.Create(hit.transform.position + offset, ndmg, false);
                    }
                    _bullet.gameObject.GetComponent<BulletBehaviour>().BulletHole(hit);
                    CallHitImpact(hit);
                }
                else
                {
                    DamagePopup.Create(hit.transform.position + offset, 0, true);
                }
            }
            if (hit.transform.gameObject.CompareTag("Wall"))
            {
                _bullet.gameObject.GetComponent<BulletBehaviour>().BulletHole(hit);
                CallHitImpact(hit);
            }
            if (hit.transform.gameObject.CompareTag("Target"))
            {
                if (hit.transform.gameObject.GetComponent<TargetScript>().canGoDown)
                {
                    _bullet.gameObject.GetComponent<BulletBehaviour>().BulletHole(hit);
                }
                CallHitImpact(hit);
            }
        }
        _fpscam.GetComponent<FPSCameraBehavior>().AnimateCameraFOV();
        _player.GetComponent<PlayerHUDBehaviour>().AnimateCrosshair();
        Recoil();
    }
    #endregion

    #region Spawn the Hit Impact
    // Based on the time that would take for the bullet to hit the target
    private void CallHitImpact(RaycastHit hit)
    {
        IEnumerator coroutine = HitImpact(hit);
        StartCoroutine(coroutine);
    }

    IEnumerator HitImpact(RaycastHit hit)
    {
        yield return new WaitForSeconds(hit.distance / _bulletPrefab.GetComponent<BulletBehaviour>().bulletSpeed);
        if (hit.transform.gameObject.CompareTag("Target"))
            Instantiate(_impactMetal, hit.point, Quaternion.LookRotation(hit.normal));
        else if (hit.transform.gameObject.CompareTag("Enemies") || hit.transform.gameObject.CompareTag("Boss"))
            Instantiate(_impactBlood, hit.point, Quaternion.LookRotation(hit.normal));
        else if (hit.transform.gameObject.CompareTag("Wall"))
            Instantiate(_impactConcrete, hit.point, Quaternion.LookRotation(hit.normal));
    }
    #endregion

    #region Spawn Bullet
    // Intantiate a bullet at the edge of the gun barrel. Atm it's using instantiate, but would be better to use a pool.
    private void SpawnBullet()
    {
        Instantiate(_bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }
    #endregion

    #region Point Gun Barrel to the center of the Aim based on distance
    // Do a raycast that get the hit.point for the gun to look at.
    void PointGunToCenter()
    {
        RaycastHit target;
        Debug.DrawLine(_fpscam.transform.position, _fpscam.transform.forward * 100f, Color.green, Time.deltaTime);
        if (Physics.Raycast(_fpscam.transform.position, _fpscam.transform.forward, out target, 100f, ignoreRaycast) && target.distance > 5f)
        {
            transform.LookAt(target.point);
        }
        else
        {
            transform.LookAt(_notarget);
        }
    }
    #endregion

    #region Apply Recoil Method
    // Acess the FPSCameraBehaviour and apply recoil based on given values and weapon shooting mode.
    void Recoil()
    {
        if (weaponMode == WeaponMode.Auto || weaponMode == WeaponMode.Burst)
        {
            bool leftOrRight = Random.value > 0.5f;
            _fpscam.GetComponent<FPSCameraBehavior>().ApplyRecoil(verticalRecoil, horizontalRecoil, leftOrRight);
        }
        else
        {
            bool leftOrRight = Random.value > 0.5f;
            _fpscam.GetComponent<FPSCameraBehavior>().ApplyRecoil(verticalRecoil, verticalRecoil / 25, leftOrRight);
        }
    }
    #endregion

    #region Add Ammo
    public void AddAmmo(int amount)
    {
        actualAmmo += amount;
    }
    #endregion

}
