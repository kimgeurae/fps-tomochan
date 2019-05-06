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
    private MeshFilter _meshFilter;
    private Mesh _defaultMesh;
    public Mesh _circleMesh;
    private enum State
    {
        Stand,
        Crouch,
        Prone,
        Slide,
        Hook,
    }
    private State state;
    public int maxHealth;
    private int health;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        groundLayer = LayerMask.GetMask("Ground");
        _canDash = true;
        _groundChecker = transform.GetChild(0);
        Cursor.lockState = CursorLockMode.Locked;
        _fpscam = transform.GetChild(1).GetComponent<Camera>();
        _meshFilter = GetComponent<MeshFilter>();
        _defaultMesh = GetComponent<MeshFilter>().mesh;
        state = State.Stand;
        health = maxHealth;
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
        Crouch();
    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 forwardMovement = transform.forward * verticalInput;
        Vector3 strafeMovement = transform.right * horizontalInput;
        switch (state)
        {
            case State.Stand:
                _controller.Move((forwardMovement + strafeMovement) * Time.deltaTime * speed);
                break;
            case State.Crouch:
                _controller.Move((forwardMovement + strafeMovement) * Time.deltaTime * speed / 2);
                break;
        }
        /*
        if (move != Vector3.zero)
        {
            transform.forward = move;
        }
        */
    }

    private void Crouch()
    {
        if (Input.GetKey(KeyCode.C))
        {
            _controller.height = 1f;
            Vector3 offset = new Vector3(0f, -0.5f, 0f);
            _controller.center = offset;
            _fpscam.transform.localPosition = new Vector3(0f, 0f, 0.2f);
            _meshFilter.mesh = _circleMesh;
            state = State.Crouch;
        }
        else
        {
            _controller.height = 2;
            _controller.center = Vector3.zero;
            _fpscam.transform.localPosition = new Vector3(0f, 0.5f, 0.2f);
            _meshFilter.mesh = _defaultMesh;
            state = State.Stand;
        }
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
            switch (state)
            {
                case State.Stand:
                    _velocity.y += Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
                    break;
                case State.Crouch:
                    _velocity.y += Mathf.Sqrt(jumpHeight * -2f/2 * Physics.gravity.y);
                    break;
            }
        }
    }

    private void Dash()
    {
        if (Input.GetButtonDown("Dash") && _canDash)
        {
            Debug.Log("Dash Triggered");
            switch (state)
            {
                case State.Stand:
                    _velocity += Vector3.Scale(transform.forward, dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * drag.x + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * drag.z + 1)) / -Time.deltaTime)));
                    break;
                case State.Crouch:
                    _velocity += (Vector3.Scale(transform.forward, dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * drag.x + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * drag.z + 1)) / -Time.deltaTime))))/2;
                    break;
            }
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
        if (Input.GetButton("Fire1"))
        {
            RequestShoot();
        }
    }

    void RequestShoot()
    {
        _gun.gameObject.GetComponent<BasicWeaponBehaviour>().Shoot();
    }

    public void AddHealth(int amount)
    {
        if (health + amount < 100)
        {
            health += amount;
        }
        else
        {
            health = 100;
        }
    }

    public void RemoveHealth(int amount)
    {
        if (health - amount > 0)
        {
            health -= amount;
            Debug.Log(health);
        }
        else
        {
            health = 0;
        }
    }
}
