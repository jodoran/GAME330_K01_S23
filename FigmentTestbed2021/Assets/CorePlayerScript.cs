using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorePlayerScript : MonoBehaviour
{
    public int _health = 9;
    public float _immunityTime = 3f;
    public List<GameObject> _healthIcons = new List<GameObject>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            int _elay = collision.gameObject.layer;
            StartCoroutine(ImmunityTimer(_elay));
            this.GetComponent<Animator>().SetTrigger("Immune");
        }
    }
    
    public void TakeDamage(int DamageTaken)
    {
        _health -= DamageTaken;
        _healthIcons[_health].SetActive(false);
    }

    IEnumerator ImmunityTimer(int _elay)
    {
        
        Physics2D.IgnoreLayerCollision(layer1: gameObject.layer, layer2: _elay, true);
        yield return new WaitForSeconds(_immunityTime);
        Physics2D.IgnoreLayerCollision(layer1: gameObject.layer, layer2: _elay, false);
    }
}
