using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectables : MonoBehaviour
{
    public float _maxY;
    public float _minY;
    bool UpDown;

    public AudioSource CollectedSound;

    private void Update()
    {
        if(transform.position.y >= _maxY)
        {
            UpDown = false;
        }
        else if (transform.position.y <= _minY)
        {
            UpDown = true;
        }

        if(UpDown == true)
        {
            transform.Translate((Vector3.up * Time.deltaTime) / 2, Space.Self);
        }
        else if (UpDown == false)
        {
            transform.Translate((Vector3.down * Time.deltaTime) / 2, Space.Self);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.GetComponent<CorePlayerScript>()._collectableCount++;
            collision.GetComponent<CorePlayerScript>().SetCollectedCount();
            CollectedSound.Play();
            Destroy(gameObject);
        }
    }
}
