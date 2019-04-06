using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{

    private CharacterController _controller;
    private Vector3 _velocity;
    private Transform _groundChecker;
    private LayerMask groundLayer;
    public Camera fpscam;
    private bool _canDash;
    private bool _isGrounded;
    public float speed;
    public float groundDistance;
    public float jumpHeight;
    public int dashCooldown;
    public float dashDistance;
    public Vector3 drag;
    public float dragDuration;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        groundLayer = LayerMask.GetMask("Ground");
        _canDash = true;
        _groundChecker = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Gravity();
        Jump();
        Dash();
        ApplyDrag();
        Rotate();
    }

    private void Movement()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        _controller.Move(move * Time.deltaTime * speed);
        if (move != Vector3.zero)
        {
            transform.forward = move;
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

    void Rotate()
    {
        float rotx = Input.GetAxis("Mouse X");
        transform.Rotate(0f, rotx, 0f);
        //fpscam.transform.Rotate(transform.rotation.)
    }

}
