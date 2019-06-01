using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesBasicBehaviour : MonoBehaviour
{

    private Rigidbody _rb;
    [SerializeField]
    Transform _player;
    GameObject _target;

    float count = 0f;
    Vector3[] point = new Vector3[3];

    bool hasTarget = false;
    [SerializeField]
    float attackCooldown = .3f;

    private enum State
    {
        Awakened,
        Looking,
        Attacking,
        Recharging,
    }
    private State state;

    float rechargeTime = .3f;
    float rechargeValue = 0f;
    float prepareAttack = .7f;
    float prepareValue = 0f;

    private int health;

    public int maxBossHealth;

    public int bossDmg;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        health = maxBossHealth;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Mathf.Sqrt(Mathf.Pow(_player.position.x - transform.position.x, 2) + Mathf.Pow(_player.position.z - transform.position.z, 2));
        if (distance < 0)
            distance = distance * -1;
        if (distance < 30f)
            _target = _player.gameObject;
        else
            _target = null;
        if (_target != null)
            Behaviour();
        //Debug.Log(state);
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
        // (xM = (xA + xB) / 2, yM = (yA + yB) / 2)
        point[2] = _player.transform.position; ; // Use _player.transform.position; for jumping into the player. This line makes it jumps 1/2 of the distance.
        //point[2] = new Vector3((point[0].x + point[2].x) / 2, _player.transform.position.y, (point[0].z + point[2].z) / 2); // This line is for making it jumps only 1/4 of the distance.
        point[1] = point[0] + (point[2] - point[0]) / 2 + Vector3.up * 7f;
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

    private void OnTriggerEnter(Collider collision)
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
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colidiu inimigo simples");
            collision.gameObject.GetComponent<PlayerBehaviour>().RemoveHealth(bossDmg);
        }
    }

    public void ApplyDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

}
