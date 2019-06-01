using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattleTrigger : MonoBehaviour
{

    [SerializeField]
    GameObject _boss;
    public GameObject _fireWall;

    public AudioSource _bossMusic;
    public AudioSource _generalMusic;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _boss.SetActive(true);
            _fireWall.SetActive(true);
            //this.gameObject.SetActive(false);
            _bossMusic.Play();
            _generalMusic.Stop();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            
        }
    }

}
