using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDBehaviour : MonoBehaviour
{

    #region Main References
    GameObject _gunSlot;
    //GameObject[] _weapons = new GameObject[];
    List<GameObject> _weapons = new List<GameObject>();
    GameObject _activeWeapon;
    #endregion

    #region Crosshair Variables
    public Image _leftCrosshair;
    public Image _rightCrosshair;
    public Image _topCrosshair;
    public Image _botCrosshair;
    private Vector2 topPos;
    private Vector2 botPos;
    private Vector2 leftPos;
    private Vector2 rightPos;
    private GameObject _crosshair;

    [SerializeField]
    float crosshairSpreadAmount = 30f;
    [SerializeField]
    float crosshairSpreadDuration = 0.2f;
    #endregion

    #region Reload Indicator Variables
    private Image _reloadCircle;
    #endregion

    private void Start()
    {

        #region Load Main References
        //Debug.Log(_gunSlot.transform.childCount.ToString());
        _gunSlot = GameObject.FindGameObjectWithTag("GunSlot");
        for (int i = 0; i < _gunSlot.transform.childCount; i++)
        {
            _weapons.Add(_gunSlot.transform.GetChild(i).gameObject);
        }
        foreach (GameObject weapon in _weapons)
        {
            if (weapon.activeSelf)
            {
                _activeWeapon = weapon;
                Debug.Log(_activeWeapon.gameObject.name.ToString());
            }
        }
        Debug.Log("________________________________");
        Debug.Log(_weapons.Count);
        Debug.Log(_weapons);
        Debug.Log("_________________________________");
        #endregion

        #region Load Crosshair References
        _crosshair = GameObject.FindGameObjectWithTag("HUD").transform.GetChild(0).gameObject;
        _topCrosshair = _crosshair.transform.GetChild(0).GetComponent<Image>();
        _botCrosshair = _crosshair.transform.GetChild(1).GetComponent<Image>();
        _leftCrosshair = _crosshair.transform.GetChild(2).GetComponent<Image>();
        _rightCrosshair = _crosshair.transform.GetChild(3).GetComponent<Image>();
        topPos = _topCrosshair.rectTransform.position;
        botPos = _botCrosshair.rectTransform.position;
        leftPos = _leftCrosshair.rectTransform.position;
        rightPos = _rightCrosshair.rectTransform.position;
        #endregion

        #region Load Reload Indicator References
        _reloadCircle = GameObject.FindGameObjectWithTag("HUD").transform.GetChild(3).transform.GetChild(0).gameObject.GetComponent<Image>();
        #endregion

    }

    public void AnimateCrosshair()
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

    public void ReloadCircleHUD(float f)
    {
        IEnumerator coroutine;
        coroutine = ReloadProgressCircle(f);
        StopCoroutine("ReloadProgressCircle");
        StartCoroutine(coroutine);
    }

    IEnumerator ReloadProgressCircle(float time)
    {
        //_anim.speed = (1f / 2f);
        //_anim.SetBool("isReloading", true);
        _reloadCircle.gameObject.SetActive(true);
        _reloadCircle.fillAmount = 0f;
        Debug.Log("Coroutine ReloadProgressCircleCheck == OK");
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(time / 100);
            _reloadCircle.fillAmount += 0.01f;
        }
        _activeWeapon.GetComponent<BasicWeaponBehaviour>().ReloadWeapon();
        _reloadCircle.gameObject.SetActive(false);
    }

}
