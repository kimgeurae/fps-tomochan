using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{

    private CharacterController _controller;
    private Vector3 _velocity;
    private Transform _groundChecker;
    private LayerMask groundLayer;
    private Camera _fpscam;
    public GameObject _gun;
    private bool _canDash;
    private bool _isGrounded;
    public Transform _notarget;
    public float speed;
    public float groundDistance;
    public float jumpHeight;
    public int dashCooldown;
    public float dashDistance;
    public Vector3 drag;
    public float dragDuration;

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
        _controller = GetComponent<CharacterController>();
        groundLayer = LayerMask.GetMask("Ground");
        _canDash = true;
        _groundChecker = transform.GetChild(0);
        Cursor.lockState = CursorLockMode.Locked;
        _fpscam = transform.GetChild(1).GetComponent<Camera>();
        topPos = _topCrosshair.rectTransform.position;
        botPos = _botCrosshair.rectTransform.position;
        leftPos = _leftCrosshair.rectTransform.position;
        rightPos = _rightCrosshair.rectTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Gravity();
        Jump();
        Dash();
        ApplyDrag();
        PlayerShoot();
        PointGunToCenter();
    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 forwardMovement = transform.forward * verticalInput;
        Vector3 strafeMovement = transform.right * horizontalInput;
        _controller.Move((forwardMovement + strafeMovement) * Time.deltaTime * speed);
        /*
        if (move != Vector3.zero)
        {
            transform.forward = move;
        }
        */
    }

    private void Gravity()
    {
        _velocity.y += Physics.gravity.y * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
        _isGrounded = Physics.CheckSphere(_groundChecker.position, groundDistance, groundLayer, QueryTriggerInteraction.Ignore);
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = 0f;
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _velocity.y += Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        }
    }

    private void Dash()
    {
        if (Input.GetButtonDown("Dash") && _canDash)
        {
            Debug.Log("Dash Triggered");
            _velocity += Vector3.Scale(transform.forward, dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * drag.x + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * drag.z + 1)) / -Time.deltaTime)));
            ActiveDashTimer();
        }        
    }

    void ActiveDashTimer()
    {
        StopCoroutine("DashTimer");
        StartCoroutine("DashTimer");
    }

    void ApplyDrag()
    {
        _velocity.x /= 1 + drag.x * Time.deltaTime;
        _velocity.y /= 1 + drag.y * Time.deltaTime;
        _velocity.z /= 1 + drag.z * Time.deltaTime;
    }

    IEnumerator DashTimer()
    {
        _canDash = false;
        yield return new WaitForSeconds(dragDuration);
        _canDash = true;
        yield break;
    }

    void UnlockCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void PlayerShoot()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        Debug.DrawLine(_fpscam.transform.position, _fpscam.transform.forward * 100f, Color.yellow, 2.5f);
        if (Physics.Raycast(_fpscam.transform.position, _fpscam.transform.forward, out hit, 100f))
        {
            if (hit.transform.gameObject.CompareTag("Enemies"))
            {
                Debug.Log("Acertou inimigo");
            }
        }
        _gun.gameObject.GetComponent<BasicWeaponBehaviour>().ShootBullet();
        AnimateCameraFOV();
        AnimateCrosshair();
        Recoil();
    }

    void PointGunToCenter()
    {
        RaycastHit target;
        Debug.DrawLine(_fpscam.transform.position, _fpscam.transform.forward * 100f, Color.green, .5f);
        if (Physics.Raycast(_fpscam.transform.position, _fpscam.transform.forward, out target, 100f))
        {
            _gun.transform.LookAt(target.point);
        }
        else
        {
            _gun.transform.LookAt(_notarget);
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
        _fpscam.GetComponent<FPSCameraBehavior>().ApplyRecoil(2f);
    }

}
