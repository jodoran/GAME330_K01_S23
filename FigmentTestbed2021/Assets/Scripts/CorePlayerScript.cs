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
    public GameObject WinScreen;
    public GameObject PlayerCamera;
    public GameObject EndCinematic;

    public AudioSource MainMusic;
    public AudioSource DeathSound;
    public AudioSource WinSound;
    public AudioSource HitSound;

    public Text CollectCountNum;
    public int _collectableCount;
    public int _maxCollectable;
    public List<GameObject> _collectGates = new List<GameObject>();

    public int _levelNum;

    private bool _gameEnd;
    private bool _immune = false;
    Animator _playerAnimation;

    private void Start()
    {
        SetCollectedCount();
        _playerAnimation = GetComponent<Animator>();
    }

    private void Update()
    {
        if (FigmentInput.GetButtonDown(FigmentInput.FigmentButton.ActionButton) && _health <= 0 ||
            FigmentInput.GetButtonDown(FigmentInput.FigmentButton.ActionButton) && _gameEnd == true)
        {
            if(EndCinematic == null)
            {
                SceneManager.LoadScene("Menu");
            }
            else if (EndCinematic.GetComponent<Cinematics>().End)
            { 
                SceneManager.LoadScene("Menu"); 
            }
            
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "KillZone")
        {
            int _elay = collision.gameObject.layer;
            TakeDamage(collision.gameObject.GetComponent<KillZone>()._damage, _elay);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "KillZone")
        {
            int _elay = collision.gameObject.layer;
            TakeDamage(collision.gameObject.GetComponent<KillZone>()._damage, _elay);
        }
    }


    public void TakeDamage(int DamageTaken, int _elay)
    {
        if (_immune == false)
        {
            _immune = true;
            _health -= DamageTaken;
            HitSound.Play();

            for (int i = 0; i < _healthObject.Count; i++)
            {
                _healthObject[i].SetActive(i <= _health);
            }

            _playerAnimation.SetTrigger("Damaged");
            StartCoroutine(ImmunityTimer(_elay));

            if (_health <= 0)
            {
                Death();
                DeathSound.Play();
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
            WinScreen.SetActive(true);
            WinSound.Play();
            MainMusic.Stop();

            if (EndCinematic != null)
            {
                EndCinematic.transform.parent.gameObject.SetActive(true);
                StartCoroutine(EndCinematic.GetComponent<Cinematics>().PlayScenes());
                StartCoroutine(EndCinematic.GetComponent<Cinematics>().SetSkip());
            }

            _gameEnd = true;
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
        _gameEnd = true;
        EndCinematic.GetComponent<Cinematics>().End = true;
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
