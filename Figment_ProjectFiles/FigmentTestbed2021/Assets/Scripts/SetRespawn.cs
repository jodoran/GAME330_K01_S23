using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRespawn : MonoBehaviour
{
    public GameObject RespawnPoint;
    public Sprite PressedButton;
    public GameObject RespawnNotice;
    public AudioSource ActivatedSound;

    bool _activated;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && _activated == false)
        {
            RespawnPoint.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
            GetComponent<SpriteRenderer>().sprite = PressedButton;
            ActivatedSound.Play();
            RespawnNotice.SetActive(true);
            _activated = true;
        }
    }
}
