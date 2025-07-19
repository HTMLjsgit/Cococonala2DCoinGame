using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPCoinScript : MonoBehaviour
{
    PlayerStatus player_status;
    public bool AlreadyPassed;
    AudioSource my_source;
    [SerializeField] private float HP = 3;
    // Start is called before the first frame update
    void Start()
    {
        player_status = GameObject.FindWithTag("Player").GetComponent<PlayerStatus>();
        my_source = this.gameObject.GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!AlreadyPassed)
            {
                my_source.PlayOneShot(my_source.clip);
                player_status.HPSet(HP);
                Destroy(this.gameObject, 0.5f);
                AlreadyPassed = true;
            }
        }
    }

}
