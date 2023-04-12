using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleWall : MonoBehaviour
{
    public int _neededCollectables;

    public void SetBubbleWall(float _collectables)
    {
        if(_collectables == _neededCollectables)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            GetComponent<ParticleSystem>().Stop();
            GetComponent<Collider2D>().enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponent<ParticleSystem>().Play();
        GetComponent<Collider2D>().enabled = true;
    }
}
