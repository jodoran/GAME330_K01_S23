using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CorePlayerScript : MonoBehaviour
{
    public int _health = 8;
    public float _immunityTime = 3f;
    public List<GameObject> _healthObject = new List<GameObject>();
    public GameObject DeathScreen;
    public GameObject PlayerCamera;
    public AudioSource MainMusic;

    public Text CollectCountNum;
    public int _collectableCount;
    public int _maxCollectable;
    public List<GameObject> _collectGates = new List<GameObject>();

    public int _levelNum;

    private bool _immune;

    private void Start()
    {
        SetCollectedCount();
    }

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
            int _elay = collision.gameObject.layer;
            StartCoroutine(ImmunityTimer(_elay));
            this.GetComponent<Animator>().SetTrigger("Immune");
        }
    }

    public void TakeDamage(int DamageTaken)
    {
        if (_immune == false)
        {
            _immune = true;
            _health -= DamageTaken;

            for (int i = 0; i < _healthObject.Count; i++)
            {
                _healthObject[i].SetActive(i <= _health);
            }

            if (_health <= 0)
            {
                Death();

            }
        }
    }

    public void SetCollectedCount()
    {
        CollectCountNum.text = _collectableCount.ToString() + "/" + _maxCollectable.ToString();
        CollectCountNum.transform.GetChild(0).GetComponent<Text>().text = _collectableCount.ToString() + "/" + _maxCollectable.ToString();

        if (_collectableCount == _maxCollectable)
        {
            var _levelsComplete = PlayerPrefs.GetInt("LevelsComplete", 0);
            if (_levelsComplete < _levelNum)
            {
                PlayerPrefs.SetInt("LevelsComplete", _levelNum);
            }
            SceneManager.LoadScene("Menu");
        }
        else
        {
            foreach(GameObject gates in _collectGates)
            {
                gates.GetComponent<BubbleWall>().SetBubbleWall(_collectableCount);
            }
        }
    }

    void Death()
    {
        MainMusic.Stop();
        DeathScreen.SetActive(true);
        this.GetComponent<PlayerMovement2D>().enabled = false;
    }

    IEnumerator ImmunityTimer(int _elay)
    {
        Physics2D.IgnoreLayerCollision(layer1: gameObject.layer, layer2: _elay, true);
        yield return new WaitForSeconds(_immunityTime);
        Physics2D.IgnoreLayerCollision(layer1: gameObject.layer, layer2: _elay, false);
        _immune = false;
    }
}
