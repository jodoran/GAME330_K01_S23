using System;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    public int _damage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        try
        {
            collision.transform.position = collision.gameObject.GetComponent<PlayerMovement2D>()._respawnPoint.position;
        }
        catch (NullReferenceException)
        {

        }
    }
}
