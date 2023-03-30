using System;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        try
        {
            collision.transform.position = collision.GetComponent<PlayerMovement2D>()._respawnPoint.position;
        }
        catch (NullReferenceException)
        {

        }
    }
}
