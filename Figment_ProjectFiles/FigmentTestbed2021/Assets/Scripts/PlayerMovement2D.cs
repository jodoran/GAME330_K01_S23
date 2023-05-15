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

    public AudioSource LandSoundEffect;
    public AudioSource JumpSoundEffect;
    public GameObject Camera;
    public Transform _respawnPoint;
    public ParticleSystem JumpParticle;

    Rigidbody2D rb;
    bool _collisionCheck;
    bool _switchGrounded;
    SpriteRenderer _playerSprite;
    Animator _playerAnimation;

    public LayerMask ground;
    Vector2 offset;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _playerSprite = GetComponent<SpriteRenderer>();
        _playerAnimation = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = 0;

        // Rotate the player by pressing left or right
        if (FigmentInput.GetButton(FigmentInput.FigmentButton.LeftButton))
        {
            _playerSprite.flipX = true;
            if (_rightMovement && !_grounded) { rb.AddForce(new Vector2(-_changeSpeed, 0) * Time.deltaTime); }
            _playerAnimation.SetBool("Walking", true);

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
            _playerAnimation.SetBool("Walking", true);

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
            _playerAnimation.SetBool("Walking", false);
        }



        Vector2 velocity = new Vector2 (horizontal, 0);

        if (Mathf.Abs(rb.velocity.x) < _speedLimit)
        {
            rb.AddForce(velocity * Time.deltaTime);
        }
    }

    void Update()
    {
        if (FigmentInput.GetButtonDown(FigmentInput.FigmentButton.ActionButton))
        {
            Vector2 movement = new Vector2(0, 0);
            _playerAnimation.SetTrigger("Jump");
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

                JumpSoundEffect.pitch = Random.Range(0.5f, 1f);
                JumpSoundEffect.Play();
                JumpParticle.Play();
                _grounded = false;
            }
        }

        if (!_grounded && rb.velocity.y > 0)
        {
            offset = new Vector2(offset.x, transform.localPosition.y + _cameraOffsetVertical);
        }
        else if (!_grounded && rb.velocity.y < 0)
        {
            offset = new Vector2(offset.x, transform.localPosition.y - (_cameraOffsetVertical * Time.deltaTime) + rb.velocity.y / 2);
        }

        Camera.transform.localPosition = Vector2.Lerp(Camera.transform.localPosition, offset, 3f * Time.deltaTime);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && _switchGrounded == false 
            || collision.gameObject.tag == "Wall" && _switchGrounded == false)
        {
            LandSoundEffect.Play();
            _switchGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Wall")
        {
            _switchGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Wall" && collision.tag != "AboveWater")
        {
            _grounded = true;
            _collisionCheck = false;
        }
        else if (collision.tag == "AboveWater")
        {
            _jumpEnabled = false;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Enemy" && collision.tag != "Wall" && collision.tag != "AboveWater")
        {
            _collisionCheck = true;
            StartCoroutine(SafeJump());
        }
        else if (collision.tag == "AboveWater")
        {
            _jumpEnabled = true;
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
