using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagazineScript : MonoBehaviour
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
        
    }

    public void ResetReloadAnimationParameter()
    {
        //_anim.SetBool("isReloading", false);
    }

}
