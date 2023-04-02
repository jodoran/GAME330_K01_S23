using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public ParticleSystem Bullet;
    public Transform _turretSeat;

    private bool _reset = false;
    private bool _active = false;

    private void Update()
    {
        if(FigmentInput.GetButtonDown(FigmentInput.FigmentButton.ActionButton))
        {
            if (_active == true && _reset == true)
            {
                StartCoroutine(Shoot());
                _active = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.transform.position = _turretSeat.position;
            collision.GetComponent<PlayerMovement2D>().DisableJump();
            _reset = true;
            _active = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerMovement2D>().EnableJump();
            _reset = false;
        }
    }

    IEnumerator Shoot()
    {
        if (Bullet.gameObject.activeSelf)
        {
            Bullet.Play();
        }
        else
        {
            Bullet.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.25f);
        _active = true;
    }
}
