using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turtle : MonoBehaviour
{
    Rigidbody2D rb;
    public int _health;
    public int _damage;
    public float _speed;

    public AudioSource Hit1;
    public AudioSource Hit2;
    public float _soundDistance;
    public Transform PlayerPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector3(_speed, 0, 0);
    }

    private void FixedUpdate()
    {
        if (rb.velocity.x >= -1 && rb.velocity.x <= 1)
        {
            if (rb.velocity.x >= 0)
            {
                rb.velocity = new Vector3(-_speed, 0, 0);
            }
            else if (rb.velocity.x < 0)
            {
                rb.velocity = new Vector3(_speed, 0, 0);
            }
        }


        if (Vector3.Distance(PlayerPos.position, transform.position) <= _soundDistance)
        {
            Hit1.volume = ((_soundDistance - Vector3.Distance(PlayerPos.position, transform.position)) / _soundDistance) / 3;
            Hit2.volume = ((_soundDistance - Vector3.Distance(PlayerPos.position, transform.position)) / _soundDistance) / 3;
        }
        else
        {
            Hit1.volume = 0;
            Hit2.volume = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground" || collision.tag == "Wall" || collision.tag == "Player")
        {
            if (rb.velocity.x >= 0)
            {
                rb.velocity = new Vector3(-_speed, 0, 0);
                Hit1.Play();
                GetComponent<SpriteRenderer>().flipX = false;
            }
            else if (rb.velocity.x < 0)
            {
                rb.velocity = new Vector3(_speed, 0, 0);
                Hit2.Play();
                GetComponent<SpriteRenderer>().flipX = true;
            }

            if (collision.gameObject.tag == "Player")
            {
                collision.GetComponent<CorePlayerScript>().TakeDamage(_damage, gameObject.layer);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }
}
