using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public float _jumpHeight;
    public float _movementSpeed;
    public float _speedLimit;
    public bool _grounded;

    public Transform _respawnPoint;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float Horizontal = 0;

        // Rotate the player by pressing left or right
        if (FigmentInput.GetButton(FigmentInput.FigmentButton.LeftButton))
        {
            Horizontal = -_movementSpeed;
        }
        else if (FigmentInput.GetButton(FigmentInput.FigmentButton.RightButton))
        {
            Horizontal = _movementSpeed;
        }

        // If we press the action button, move forward
        if (FigmentInput.GetButtonDown(FigmentInput.FigmentButton.ActionButton))
        {
            if (_grounded)
            {
                rb.AddForce(Vector2.up * _jumpHeight);
            }
        }

        Vector2 velocity = new Vector2 (Horizontal, 0);

        if(Mathf.Abs(rb.velocity.x) < _speedLimit)
        {
            rb.AddForce(velocity * Time.deltaTime);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _grounded = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _grounded = false;
    }

}
