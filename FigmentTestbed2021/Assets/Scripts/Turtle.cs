using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turtle : MonoBehaviour
{
    Rigidbody2D rb;
    public int _health;
    public int _damage;
    public float _speed;

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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground" || collision.tag == "Wall")
        {
            if (rb.velocity.x >= 0)
            {
                rb.velocity = new Vector3(-_speed, 0, 0);
            }
            else if (rb.velocity.x < 0)
            {
                rb.velocity = new Vector3(_speed, 0, 0);
            }

            if (collision.gameObject.tag == "Player")
            {
                collision.GetComponent<CorePlayerScript>().TakeDamage(_damage);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }
}
