using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FbaTestScript : MonoBehaviour
{
    Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();    
    }

    void Inputs()
    {
        float inputX = Input.GetAxisRaw("Vertical");
        float inputY = Input.GetAxisRaw("Horizontal");
        UpdateAnimator(inputX, inputY);
    }

    void UpdateAnimator(float x, float y)
    {
        _anim.SetFloat("moveXvelocity", x);
        _anim.SetFloat("moveYvelocity", y);
    }

}
