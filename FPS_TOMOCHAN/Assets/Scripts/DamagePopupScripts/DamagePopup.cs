using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CodeMonkey.Utils;

public class DamagePopup : MonoBehaviour
{

    // Create a damage popup.
    public static DamagePopup Create(Vector3 position, int damageAmount, bool isCriticalHit)
    {
        Transform damagePopupTransform = Instantiate(CodeMonkey.Assets.i.pfDamagePopup, position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount, isCriticalHit);

        return damagePopup;
    }

    private static int sortingOrder;

    private const float DISAPPEAR_TIMER_MAX = .5f;

    private TextMeshPro textMesh;
    private GameObject _player;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("PlayerAlt");
        transform.LookAt(_player.transform);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(_player.transform);
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;

        if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f)
        {
            // First Half of the popup lifetime.
            float increaseScaleAmount = 1f;
            transform.localScale += new Vector3(1f, 1f, 1f) * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            // Second half of the popup lifetime.
            float decreaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Setup(int damageAmount, bool isCriticalHit)
    {
        textMesh.SetText(damageAmount.ToString());
        if (!isCriticalHit)
        {
            // NormalHit
            textMesh.fontSize = 9;
            textColor = UtilsClass.GetColorFromString("FFCD00");
        }
        else
        {
            // CriticalHit
            textMesh.fontSize = 12;
            textColor = UtilsClass.GetColorFromString("DE1000");
        }
        textMesh.color = textColor;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;

        bool changeSide = Random.value > 0.5f;
        if (changeSide)
            moveVector = new Vector3(.1f, .3f) * 30f;
        else
            moveVector = new Vector3(-.1f, .3f) * 30f;
    }

}
