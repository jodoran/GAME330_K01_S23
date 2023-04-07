using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyFish : MonoBehaviour
{
    Rigidbody2D rb;
    public int _health;
    public int _damage;
    public float _maxY;
    public float _minY;
    public float _movementSpeed;

    private bool _downward;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector3(0, 25, 0);
    }

    private void FixedUpdate()
    {
        if (transform.localPosition.y >= _maxY)
        {
            _downward = false;
        }
        else if (transform.localPosition.y <= _minY)
        {
            _downward = true;
        }

        if (_downward == false)
        {
            StartCoroutine(DownUPAnimation());
        }
        else
        {
            rb.velocity = new Vector3(0, 5, 0);
        }
    }

    IEnumerator DownUPAnimation()
    {
        rb.velocity = new Vector3(0, 2f, 0);
        yield return new WaitForSeconds(_movementSpeed);
        rb.velocity = new Vector3(0, -5f, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.GetComponent<CorePlayerScript>().TakeDamage(_damage);
        }
    }
}
