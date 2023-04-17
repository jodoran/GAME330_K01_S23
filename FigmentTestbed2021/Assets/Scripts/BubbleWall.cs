using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleWall : MonoBehaviour
{
    public int _neededCollectables;

    public AudioSource WallSound;
    public float _soundDistance;
    public Transform PlayerPos;

    private void FixedUpdate()
    {
        if (Vector3.Distance(PlayerPos.position, transform.position) <= _soundDistance)
        {
            WallSound.volume = ((_soundDistance - Vector3.Distance(PlayerPos.position, transform.position)) / _soundDistance) / 4;
        }
        else
        {
            WallSound.volume = 0;
        }
    }

    public void SetBubbleWall(float _collectables)
    {
        if(_collectables == _neededCollectables)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            GetComponent<ParticleSystem>().Stop();
            GetComponent<Collider2D>().enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GetComponent<ParticleSystem>().Play();
            GetComponent<Collider2D>().enabled = true;
        }
    }
}
