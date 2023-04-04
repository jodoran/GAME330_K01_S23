using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CorePlayerScript : MonoBehaviour
{
    public int _health = 9;
    public float _immunityTime = 3f;
    public Slider HealthBar;
    public GameObject DeathScreen;
    public GameObject PlayerCamera;

    private void Update()
    {
        if (FigmentInput.GetButtonDown(FigmentInput.FigmentButton.ActionButton) && _health <= 0)
        {
            SceneManager.LoadScene("Menu");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            int _elay = collision.gameObject.layer;
            StartCoroutine(ImmunityTimer(_elay));
            this.GetComponent<Animator>().SetTrigger("Immune");
        }
        else if (collision.gameObject.tag == "KillZone")
        {
            TakeDamage(collision.gameObject.GetComponent<KillZone>()._damage);
        }
    }

    public void TakeDamage(int DamageTaken)
    {
        _health -= DamageTaken;
        HealthBar.value = _health;
        if (_health <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        DeathScreen.SetActive(true);
        this.GetComponent<PlayerMovement2D>().enabled = false;
    }

    IEnumerator ImmunityTimer(int _elay)
    {
        
        Physics2D.IgnoreLayerCollision(layer1: gameObject.layer, layer2: _elay, true);
        yield return new WaitForSeconds(_immunityTime);
        Physics2D.IgnoreLayerCollision(layer1: gameObject.layer, layer2: _elay, false);
    }
}
