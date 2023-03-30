using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turtle : MonoBehaviour
{
    Rigidbody2D rb;
    public int _health;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector3(25, 0, 0);
    }

    private void FixedUpdate()
    {
        if (rb.velocity.x >= -1 && rb.velocity.x <= 1)
        {
            if (rb.velocity.x >= 0)
            {
                rb.velocity = new Vector3(-25, 0, 0);
            }
            else if (rb.velocity.x < 0)
            {
                rb.velocity = new Vector3(25, 0, 0);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (rb.velocity.x >= 0)
        {
            rb.velocity = new Vector3(-25, 0, 0);
        }
        else if (rb.velocity.x < 0)
        {
            rb.velocity = new Vector3(25, 0, 0);
        }
    }
}
