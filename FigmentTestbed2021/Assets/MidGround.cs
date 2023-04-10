using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidGround : MonoBehaviour
{
    public Collider2D BlockCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "GroundChecker")
        {
            BlockCollider.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "GroundChecker")
        {
            BlockCollider.enabled = false;
        }
    }
    
}
