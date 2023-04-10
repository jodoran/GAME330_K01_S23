using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class MenuScript : MonoBehaviour
{
    public List<GameObject> LevelButtons = new List<GameObject>();
    public Sprite LockIcon;
    public Sprite LevelIcon;

    public UnityEvent EnterLevelScreen;
    public UnityEvent ExitLevelScreen;

    public AudioSource StartAudio;
    public AudioSource SelectAudio;

    public List<GameObject> _levels = new List<GameObject>();

    public GameObject PlayButton;
    public GameObject QuitButton;

    private bool _levelSelect = false;
    private int _levelSet = 1;
    private bool _playORquit = true;

    float timer;

    private void Start()
    {
        int _levelsComplete = PlayerPrefs.GetInt("LevelsComplete", 0);
        for (int i = 0; i < LevelButtons.Count; i++)
        {
            if(i > _levelsComplete)
            {
                LevelButtons[i].GetComponent<Mask>().enabled = false;
                LevelButtons[i].GetComponent<Image>().color = Color.clear;
                LevelButtons[i].transform.GetChild(0).GetComponent<Image>().sprite = LockIcon;
                _levels[i].transform.GetChild(0).gameObject.SetActive(false);
                _levels.RemoveAt(i);
            }
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (_levelSelect == true)
        {
            if (FigmentInput.GetButtonUp(FigmentInput.FigmentButton.LeftButton))
            {
                if (_levelSet > 1)
                {
                    _levels[_levelSet - 1].transform.GetChild(0).gameObject.SetActive(false);
                    _levelSet -= 1;
                    _levels[_levelSet - 1].transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    ExitLevelScreen.Invoke();
                    _levelSelect = false;
                }

                SelectAudio.Play();
            }
            else if (FigmentInput.GetButtonUp(FigmentInput.FigmentButton.RightButton))
            {
                if (_levelSet < _levels.Count)
                {
                    _levels[_levelSet - 1].transform.GetChild(0).gameObject.SetActive(false);
                    _levelSet += 1;
                    _levels[_levelSet - 1].transform.GetChild(0).gameObject.SetActive(true);
                    SelectAudio.Play();
                }

            }
            else if (FigmentInput.GetButtonDown(FigmentInput.FigmentButton.ActionButton) && timer >= 0.1f)
            {
                var level = _levelSet;
                SceneManager.LoadScene("Level" + level);
            }
        }
        else
        {
            if (FigmentInput.GetButtonUp(FigmentInput.FigmentButton.RightButton))
            {
                if (_playORquit == true)
                {
                    PlayButton.transform.GetChild(0).gameObject.SetActive(false);
                    QuitButton.transform.GetChild(0).gameObject.SetActive(true);

                    _playORquit = false;
                    SelectAudio.Play();
                }
            }
            else if (FigmentInput.GetButtonUp(FigmentInput.FigmentButton.LeftButton))
            {
                if (_playORquit == false)
                {
                    PlayButton.transform.GetChild(0).gameObject.SetActive(true);
                    QuitButton.transform.GetChild(0).gameObject.SetActive(false);

                    _playORquit = true;
                    SelectAudio.Play();
                }
            }
            else if (FigmentInput.GetButtonDown(FigmentInput.FigmentButton.ActionButton))
            {
                if (_playORquit == true)
                {
                    EnterLevelScreen.Invoke();
                    _levelSelect = true;
                    StartAudio.Play();
                    timer = 0;
                }
                else if (_playORquit == false)
                {
                    Application.Quit();
                }
            }
        }
    }
}
