using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public float _jumpHeight;
    public float _jumpTimeForce;
    public float _jumpPower;

    public float _changeSpeed;
    public float _groundSpeed;
    public float _swimSpeed;
    public float _speedLimit;

    public float _fallForgiveness;
    public float _cameraOffsetVertical;
    public float _cameraOffsetHorizontal;

    public bool _turretActive;

    bool _grounded;
    bool _jumpEnabled = true;
    bool _rightMovement;
    bool _leftMovement;

    public AudioSource JumpSoundEffect;
    public GameObject Camera;
    public Transform _respawnPoint;

    Rigidbody2D rb;
    bool _collisionCheck;
    SpriteRenderer _playerSprite;

    public LayerMask ground;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _playerSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = 0;
        Vector2 offset = new Vector2(0,0);

        // Rotate the player by pressing left or right
        if (FigmentInput.GetButton(FigmentInput.FigmentButton.LeftButton))
        {
            _playerSprite.flipX = true;
            if (_rightMovement && !_grounded) { rb.AddForce(new Vector2(-_changeSpeed, 0) * Time.deltaTime); }

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

            offset = new Vector2(transform.localPosition.x - _cameraOffsetHorizontal, transform.localPosition.y + 2f);
        }
        else if (FigmentInput.GetButton(FigmentInput.FigmentButton.RightButton))
        {
            _playerSprite.flipX = false;
            if (_leftMovement && !_grounded) { rb.AddForce(new Vector2(_changeSpeed, 0) * Time.deltaTime); }

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
            offset = new Vector2(transform.localPosition.x + _cameraOffsetHorizontal, transform.localPosition.y + 2f);
        }
        else
        {
            offset = new Vector2(transform.localPosition.x, transform.localPosition.y + 2f);
        }

        if (FigmentInput.GetButtonDown(FigmentInput.FigmentButton.ActionButton))
        {
            Vector2 movement = new Vector2(0, 0);
            JumpSoundEffect.Play();
            if (_jumpEnabled)
            {
                rb.AddForce(Vector2.up * _jumpHeight);
                if (_rightMovement)
                {
                    movement = new Vector2(_jumpPower, 0);
                    rb.velocity = movement * Time.deltaTime;
                }
                else if (_leftMovement)
                {
                    movement = new Vector2(_jumpPower, 0);

                    rb.velocity = -movement * Time.deltaTime;
                }

                _grounded = false;
            }
        }

        Vector2 velocity = new Vector2 (horizontal, 0);

        if (Mathf.Abs(rb.velocity.x) < _speedLimit)
        {
            rb.AddForce(velocity * Time.deltaTime);
        }

        if (!_grounded && rb.velocity.y > 0)
        {
            offset = new Vector2(offset.x, transform.localPosition.y + _cameraOffsetVertical);
        }
        else if (!_grounded && rb.velocity.y < 0)
        {
            offset = new Vector2(offset.x, transform.localPosition.y - _cameraOffsetVertical);
        }

       Camera.transform.localPosition = Vector2.Lerp(Camera.transform.localPosition, offset, 1.5f * Time.deltaTime);

       if(Physics2D.Linecast(transform.position, Vector2.down * .1f, ground))
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
        Camera.transform.position = _respawnPoint.position;
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
