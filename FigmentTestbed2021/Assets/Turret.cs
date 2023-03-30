using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public ParticleSystem Bullet;
    public Transform _bulletSpawn;

    private bool _reset = true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Button" && _reset == true)
        {
            _reset = false;
            StartCoroutine(Shoot());
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
        _reset = true;
    }
}
