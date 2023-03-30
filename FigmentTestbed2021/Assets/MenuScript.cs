using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class MenuScript : MonoBehaviour
{
    public UnityEvent EnterLevelScreen;
    public UnityEvent ExitLevelScreen;

    public List<GameObject> _levels = new List<GameObject>();

    private bool _levelSelect = false;
    private int _levelSet = 0;

    float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (_levelSelect == true)
        {
            if (FigmentInput.GetButtonUp(FigmentInput.FigmentButton.LeftButton))
            {
                if (_levelSet > 0)
                {
                    _levels[_levelSet].transform.GetChild(0).gameObject.SetActive(false);
                    _levelSet -= 1;
                    _levels[_levelSet].transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    ExitLevelScreen.Invoke();
                    _levelSelect = false;
                }
            }
            else if (FigmentInput.GetButtonUp(FigmentInput.FigmentButton.RightButton))
            {
                if (_levelSet < _levels.Count -1)
                {
                    _levels[_levelSet].transform.GetChild(0).gameObject.SetActive(false);
                    _levelSet += 1;
                    _levels[_levelSet].transform.GetChild(0).gameObject.SetActive(true);
                }
            }
            else if (FigmentInput.GetButtonDown(FigmentInput.FigmentButton.ActionButton) && timer >= 1)
            {
                var level = _levelSet + 1;
                SceneManager.LoadScene("Level" + level);
            }
        }
        else
        {
            if (FigmentInput.GetButtonDown(FigmentInput.FigmentButton.ActionButton))
            {
                EnterLevelScreen.Invoke();
                _levelSelect = true;

                timer = 0;
            }
        }
    }
}
