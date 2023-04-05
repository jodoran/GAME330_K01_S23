using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public float _jumpHeight;
    public float _jumpTimeForce;

    public float _groundSpeed;
    public float _swimSpeed;
    public float _speedLimit;

    public float _fallForgiveness;
    public float _cameraOffset;

    public bool _turretActive;

    bool _grounded;
    bool _jumpEnabled = true;
    bool _rightMovement;
    bool _leftMovement;

    public GameObject Camera;
    public Transform _respawnPoint;

    Rigidbody2D rb;
    bool _collisionCheck;

    public LayerMask ground;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = 0;
        Vector2 offset = new Vector2(0,0);

        // Rotate the player by pressing left or right
        if (FigmentInput.GetButton(FigmentInput.FigmentButton.LeftButton))
        {
            _rightMovement = false;
            _leftMovement = true;

            if (_grounded)
            {
                horizontal = -_groundSpeed;
            }
            else
            {
                horizontal = -_swimSpeed;
            }

            offset = new Vector2(transform.localPosition.x - _cameraOffset, transform.localPosition.y);
        }
        else if (FigmentInput.GetButton(FigmentInput.FigmentButton.RightButton))
        {
            _rightMovement = true;
            _leftMovement = false;

            if (_grounded)
            {
                horizontal = _groundSpeed;
            }
            else
            {
                horizontal = _swimSpeed;
            }
            offset = new Vector2(transform.localPosition.x + _cameraOffset, transform.localPosition.y);
        }
        else
        {
            offset = new Vector2(transform.localPosition.x, transform.localPosition.y);
        }

        if (FigmentInput.GetButtonDown(FigmentInput.FigmentButton.ActionButton))
        {
            Vector2 movement = new Vector2(0, 0);

            if (_jumpEnabled)
            {
                rb.AddForce(Vector2.up * _jumpHeight);
                if (_rightMovement)
                {
                    movement = new Vector2(_swimSpeed, 0);
                    rb.velocity = movement * Time.deltaTime;
                }
                else if (_leftMovement)
                {
                    movement = new Vector2(_swimSpeed, 0);

                    rb.velocity = -movement * Time.deltaTime;
                }

                _grounded = false;
            }
        }

        Vector2 velocity = new Vector2 (horizontal, 0);

        if(Mathf.Abs(rb.velocity.x) < _speedLimit)
        {
            rb.AddForce(velocity * Time.deltaTime);
        }

        if (!_grounded && rb.velocity.y > 0)
        {
            offset = new Vector2(offset.x, transform.localPosition.y + _cameraOffset);
        }
        else if (!_grounded && rb.velocity.y < 0)
        {
            offset = new Vector2(offset.x, transform.localPosition.y - _cameraOffset);
        }

       Camera.transform.localPosition = Vector2.Lerp(Camera.transform.localPosition, offset, 1.5f * Time.deltaTime);

       if(Physics2D.Linecast(transform.position, Vector2.down * .6f, ground))
       {
            _grounded = true;
       }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Wall")
        {
            _grounded = true;
            _collisionCheck = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Enemy" && collision.tag != "Wall")
        {
            _collisionCheck = true;
            StartCoroutine(SafeJump());
        }
    }

    IEnumerator SafeJump()
    {
        yield return new WaitForSeconds(_fallForgiveness / 100);
        if (_collisionCheck == true)
        {
            _grounded = false;
        }
    }

    public void Respawn()
    {
        transform.position = _respawnPoint.position;
        _grounded = true;
        _collisionCheck = false;
        rb.velocity = new Vector3(0,0,0);
        Camera.transform.position = transform.position;
    }

    public void DisableJump()
    {
        _jumpEnabled = false;
    }

    public void EnableJump()
    {
        _jumpEnabled = true;
    }
}
