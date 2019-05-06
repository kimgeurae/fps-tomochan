using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{

    Animator _anim;
    [SerializeField]
    float blendValue;
    [SerializeField]
    float blendWaitTime;
    [SerializeField]
    float timeToResetState;
    public bool canGoDown = false;
    [SerializeField]
    float getBlendValue;
    [HideInInspector]
    public enum State { up, down, };
    [HideInInspector]
    public State state;

    // Start is called before the first frame update
    void Start()
    {
        state = State.up;
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_anim.GetFloat("Blend") == 0f)
        {
            canGoDown = true;
        }
        else
        {
            canGoDown = false;
        }
        getBlendValue = _anim.GetFloat("Blend");
    }

    void SetTargetUp()
    {
        StopCoroutine("SetTargetDown");
        StartCoroutine("MakeTargetGoUp");
    }

    public void SetTargetDown()
    {
        StopCoroutine("SetTargetUp");
        StartCoroutine("MakeTargetGoDown");
        Invoke("SetTargetUp", timeToResetState);
    }

    IEnumerator MakeTargetGoUp()
    {
        while (_anim.GetFloat("Blend") > 0)
        {
            _anim.SetFloat("Blend", Mathf.Clamp(_anim.GetFloat("Blend") - blendValue, 0f, 1f));
            yield return new WaitForSeconds(blendWaitTime);
        }
    }

    IEnumerator MakeTargetGoDown()
    {
        while (_anim.GetFloat("Blend") < 1)
        {
            _anim.SetFloat("Blend", Mathf.Clamp(_anim.GetFloat("Blend") + blendValue, 0f, 1f));
            yield return new WaitForSeconds(blendWaitTime);
        }
    }

    public void SetStateDown()
    {
        state = State.down;
    }

    public void SetStateUp()
    {
        state = State.up;
    }

}
