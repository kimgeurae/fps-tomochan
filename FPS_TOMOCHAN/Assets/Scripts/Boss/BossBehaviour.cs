using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{

    private Rigidbody _rb;
    [SerializeField]
    GameObject _player;

    float count = 0f;
    Vector3[] point = new Vector3[3];

    bool hasTarget = false;
    [SerializeField]
    float attackCooldown = 2f;

    private enum State
    {
        Awakened,
        Looking,
        Attacking,
        Recharging,
    }
    private State state;

    Vector2[] arenaLimit = { new Vector2(-12.5f, -43.8f), new Vector2(12.5f, 18.2f) };

    float rechargeTime = 2f;
    float rechargeValue = 0f;
    float prepareAttack = 3f;
    float prepareValue = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Behaviour();
        Debug.Log(state);
    }

    private void OnEnable()
    {
        state = State.Awakened;
    }

    void Behaviour()
    {
        switch (state)
        {
            case State.Awakened:
                _rb.isKinematic = false;
                break;
            case State.Recharging:
                _rb.isKinematic = true;
                Recharge();
                break;
            case State.Looking:
                LockTarget();
                break;
            case State.Attacking:
                ArcMotion();
                break;
        }
    }

    void Recharge()
    {
        if (rechargeValue < rechargeTime)
        {
            rechargeValue += Time.deltaTime;
        }
        else
        {
            state = State.Looking;
            rechargeValue = 0f;
        }
    }

    void LockTarget()
    {
        var lookPos = _player.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2f);
        if (prepareValue < prepareAttack)
        {
            prepareValue += Time.deltaTime;
        }
        else
        {
            GetTarget();
            state = State.Attacking;
            prepareValue = 0f;
        }
    }

    void GetTarget()
    {
        point[0] = transform.position;
        point[2] = new Vector3(Mathf.Clamp(_player.transform.position.x, -12.5f, 12.5f), Mathf.Clamp(_player.transform.position.y, 1.483867f, 10f), Mathf.Clamp(_player.transform.position.z, -43.8f, 18.2f));
        point[1] = point[0] + (point[2] - point[0]) / 2 + Vector3.up * 15f;
        count = 0;
    }

    void ArcMotion()
    {
        if (count < 1f)
        {
            count += 1.0f * Time.deltaTime;

            Vector3 m1 = Vector3.Lerp(point[0], point[1], count);
            Vector3 m2 = Vector3.Lerp(point[1], point[2], count);
            transform.position = Vector3.Lerp(m1, m2, count);
        }
        else
        {
            state = State.Recharging;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (state == State.Awakened)
                state = State.Recharging;
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            state = State.Recharging;
        }
    }
}
