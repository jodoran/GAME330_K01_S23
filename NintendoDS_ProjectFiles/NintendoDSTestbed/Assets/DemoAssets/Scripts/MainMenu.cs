using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour, IDSTapListener
{
    public AudioSource ButtonClickAudio;

    public List<GameObject> MenuButtons;
    public List<GameObject> OptionButtons;
    public List<GameObject> LevelButtons;
    public List<GameObject> LevelStars;
    public GameObject ControlMenu;
    public GameObject CreditMenu;

    private GameObject Main;
    private GameObject Option;
    private GameObject Level;

    float SelectedButton = 0;
    string _activeScreen = "CreditMenu";
    bool _sliderActive = false;

    private void Start()
    {
        MenuButtons[0].GetComponent<Image>().color = Color.white;
        LoadOptions();

        Main = MenuButtons[0].transform.parent.gameObject;
        Option = OptionButtons[0].transform.parent.gameObject;
        Level = LevelButtons[0].transform.parent.gameObject;

        if (PlayerPrefs.GetInt("NormalComplete", 0) == 1) { LevelButtons[3].SetActive(true); }
        if (PlayerPrefs.GetInt("EasyComplete", 0) == 1) { LevelButtons[2].SetActive(true); }

        StartCoroutine(SwitchOffCredits());
    }

    // Called Once when Screen is tapped down on
    public void OnScreenTapDown(Vector2 tapPosition)
    {
        if (_activeScreen == "Main")
        {
            //Checks the position of the button and compares it to the tapped position
            if (DSTapRouter.RectangleContainsDSPoint(MenuButtons[0].GetComponent<RectTransform>(), tapPosition))
            {
                ButtonClickAudio.Play();
                Play(); //Calls Play if Play Button is tapped on
            }
            else if (DSTapRouter.RectangleContainsDSPoint(MenuButtons[2].GetComponent<RectTransform>(), tapPosition))
            {
                ButtonClickAudio.Play();
                Quit(); //Calls Quit if Quit Button is tapped on
            }
            else if (DSTapRouter.RectangleContainsDSPoint(MenuButtons[1].GetComponent<RectTransform>(), tapPosition))
            {
                ButtonClickAudio.Play();
                OpenOptionsMenu();
            }
        }
        else if (_activeScreen == "Option")
        {
            if (DSTapRouter.RectangleContainsDSPoint(OptionButtons[0].GetComponent<RectTransform>(), tapPosition))
            {
                ButtonClickAudio.Play();
                Back();
                _sliderActive = false;
                //Calls Quit if Quit Button is tapped on
            }
            if (DSTapRouter.RectangleContainsDSPoint(OptionButtons[1].GetComponent<RectTransform>(), tapPosition))
            {
                OptionsSetting(true);
                _sliderActive = true;
                SelectedButton = 1;
                OptionsSetting(false);
                // Gets Mouse position
                Vector3 currPosition = OptionButtons[1].GetComponent<RectTransform>().position;
                currPosition.x += (tapPosition.x - (currPosition.x + 120));

                float xDifference = OptionButtons[1].transform.GetChild(1).position.x - OptionButtons[1].transform.GetChild(0).position.x;
                
                float finaltapPosition = currPosition.x - OptionButtons[1].transform.GetChild(0).position.x;

                float endValue = xDifference - finaltapPosition;
                float endPercent = endValue / xDifference;
                endValue = endPercent + 618;
                endPercent = +endValue / 19;
                OptionButtons[1].GetComponent<Slider>().value = endPercent * -1;
            }
            if (DSTapRouter.RectangleContainsDSPoint(OptionButtons[2].GetComponent<RectTransform>(), tapPosition))
            {
                OptionsSetting(true);
                _sliderActive = true;
                SelectedButton = 2;
                OptionsSetting(false);
                // Gets Mouse position
                Vector3 currPosition = OptionButtons[2].GetComponent<RectTransform>().position;
                currPosition.x += (tapPosition.x - (currPosition.x + 120));

                float xDifference = OptionButtons[2].transform.GetChild(1).position.x - OptionButtons[2].transform.GetChild(0).position.x;

                float finaltapPosition = currPosition.x - OptionButtons[1].transform.GetChild(0).position.x;

                float endValue = xDifference - finaltapPosition;
                float endPercent = endValue / xDifference;
                endValue = endPercent + 618;
                endPercent = +endValue / 19;
                OptionButtons[2].GetComponent<Slider>().value = endPercent * -1;
            }
            if (DSTapRouter.RectangleContainsDSPoint(OptionButtons[3].GetComponent<RectTransform>(), tapPosition))
            {
                OptionsSetting(true);
                _sliderActive = false;
                SelectedButton = 3;
                OptionsSetting(false);
                bool _checkOn = OptionButtons[3].GetComponent<Toggle>().isOn;
                _checkOn = !_checkOn;
                OptionButtons[3].GetComponent<Toggle>().isOn = _checkOn;
                if (_checkOn == true) { PlayerPrefs.SetInt("AudioToggle", 1); }
                else { PlayerPrefs.SetInt("AudioToggle", 0); }
            }
        }
        else if (_activeScreen == "ControlMenu")
        {
            LevelSelectSwitch();
        }
        else if (_activeScreen == "CreditMenu")
        {
            _activeScreen = "Main";
            CreditMenu.SetActive(false);
            Main.SetActive(true);
        }
        else if (_activeScreen == "Level")
        {
            if (DSTapRouter.RectangleContainsDSPoint(LevelButtons[3].GetComponent<RectTransform>(), tapPosition))
            {
                if (PlayerPrefs.GetInt("NormalComplete", 0) == 1)
                {
                    SwitchScene(3);
                }
            }
            if (DSTapRouter.RectangleContainsDSPoint(LevelButtons[2].GetComponent<RectTransform>(), tapPosition))
            {
                if (PlayerPrefs.GetInt("EasyComplete", 0) == 1)
                {
                    SwitchScene(2);
                }
            }
            if (DSTapRouter.RectangleContainsDSPoint(LevelButtons[1].GetComponent<RectTransform>(), tapPosition))
            {
                SwitchScene(1);
            }
            if (DSTapRouter.RectangleContainsDSPoint(LevelButtons[0].GetComponent<RectTransform>(), tapPosition))
            {
                ButtonClickAudio.Play();
                Back();
            }
        }
    }

    // Called when continously as Screen is dragged on
    public void OnScreenDrag(Vector2 tapPosition) 
    {
        if (_sliderActive && SelectedButton == 1)
        {
            // Gets Mouse position
            Vector3 currPosition = OptionButtons[1].GetComponent<RectTransform>().position;
            currPosition.x += (tapPosition.x - (currPosition.x + 120));

            float xDifference = OptionButtons[1].transform.GetChild(1).position.x - OptionButtons[1].transform.GetChild(0).position.x;

            float finaltapPosition = currPosition.x - OptionButtons[1].transform.GetChild(0).position.x;

            float endValue = xDifference - finaltapPosition;
            float endPercent = endValue / xDifference;
            endValue = endPercent + 618;
            endPercent = +endValue / 19;
            OptionButtons[1].GetComponent<Slider>().value = endPercent * -1;
            PlayerPrefs.SetFloat("MusicVolume", OptionButtons[1].GetComponent<Slider>().value);
        }
        else if (_sliderActive && SelectedButton == 2)
        {
            // Gets Mouse position
            Vector3 currPosition = OptionButtons[2].GetComponent<RectTransform>().position;
            currPosition.x += (tapPosition.x - (currPosition.x + 120));

            float xDifference = OptionButtons[2].transform.GetChild(1).position.x - OptionButtons[2].transform.GetChild(0).position.x;

            float finaltapPosition = currPosition.x - OptionButtons[2].transform.GetChild(0).position.x;

            float endValue = xDifference - finaltapPosition;
            float endPercent = endValue / xDifference;
            endValue = endPercent + 618;
            endPercent = +endValue / 19;
            OptionButtons[2].GetComponent<Slider>().value = endPercent * -1;
            PlayerPrefs.SetFloat("SFXVolume", OptionButtons[2].GetComponent<Slider>().value);
        }
    }

    // Called Once when the tap is lifted up from the Screen 
    public void OnScreenTapUp(Vector2 tapPosition) { }

    private void Update()
    {
        if (!_sliderActive)
        {
            ButtonMovementSystem();
            ButtonInvokes();
        }
        else
        {
            StartCoroutine(SetActiveSlider(OptionButtons[(int)SelectedButton].GetComponent<Slider>()));
        }
    }

    void ButtonMovementSystem()
    {
        float axisValue = Input.GetAxisRaw("Horizontal");
        bool rightInput = false;
        bool leftInput = false;
        rightInput = axisValue > 0.1f;
        leftInput = axisValue < -0.1f;

        if (Input.GetButtonDown("Horizontal"))
        {
            if (rightInput)
            {
                if (_activeScreen == "Main")
                {
                    SetActiveButton(MenuButtons, -1f, MenuButtons.Count - 1, 1);
                }
                else if (_activeScreen == "Option")
                {
                    SetActiveOption(1f, 0);
                }
                else if (_activeScreen == "Level")
                {
                    if (PlayerPrefs.GetInt("NormalComplete", 0) == 1) { SetActiveButton(LevelButtons, 1f, 0, 1); }
                    else if (PlayerPrefs.GetInt("EasyComplete", 0) == 1) { SetActiveButton(LevelButtons, 1f, 0, 2); }
                    else { SetActiveButton(LevelButtons, 1f, 0, 3); }
                }
            }
            if (leftInput)
            {
                if (_activeScreen == "Main")
                {
                    SetActiveButton(MenuButtons, 1f, 0, 1);
                }
                else if (_activeScreen == "Option")
                {
                    SetActiveOption(-1f, OptionButtons.Count - 1);
                }
                else if (_activeScreen == "Level")
                {
                    if (PlayerPrefs.GetInt("NormalComplete", 0) == 1) { SetActiveButton(LevelButtons, -1f, 3, 1); }
                    else if (PlayerPrefs.GetInt("EasyComplete", 0) == 1) { SetActiveButton(LevelButtons, -1f, 2, 1); }
                    else { SetActiveButton(LevelButtons, -1f, 1, 1); }
                }
            }
        }
    }

    void ButtonInvokes()
    {
        if (Input.GetButtonDown("Fire2")){

            if (_activeScreen == "Main")
            {
                switch (SelectedButton)
                {
                    case 0:
                        Play();
                        break;
                    case 1:
                        OpenOptionsMenu();
                        break;
                    case 2:
                        Quit();
                        break;
                }
            }
            else if (_activeScreen == "Option")
            {
                switch (SelectedButton)
                {
                    case 0:
                        Back();
                        break;
                    case 1:
                        _sliderActive = true;
                        break;
                    case 2:
                        _sliderActive = true;
                        break;
                    case 3:
                        bool _checkOn = OptionButtons[3].GetComponent<Toggle>().isOn;
                        _checkOn = !_checkOn;
                        OptionButtons[3].GetComponent<Toggle>().isOn = _checkOn;
                        if (_checkOn == true) { PlayerPrefs.SetInt("AudioToggle", 1); }
                        else { PlayerPrefs.SetInt("AudioToggle", 0); }
                        break;
                }
            }
            else if (_activeScreen == "ControlMenu")
            {
                LevelSelectSwitch();
            }
            else if (_activeScreen == "Level")
            {
                switch (SelectedButton)
                {
                    case 0:
                        Back();
                        break;
                    case 1:
                        SwitchScene(1);
                        break;
                    case 2:
                        SwitchScene(2);
                        break;
                    case 3:
                        SwitchScene(3);
                        break;
                }
            }
        }
        if (Input.anyKeyDown)
        {
            if (_activeScreen == "CreditMenu")
            {
                _activeScreen = "Main";
                CreditMenu.SetActive(false);
                Main.SetActive(true);
            }
        }
    }

    IEnumerator SetActiveSlider(Slider ActiveSlider)
    {
        float axisValue = Input.GetAxisRaw("Horizontal");
        bool rightInput = false;
        bool leftInput = false;
        rightInput = axisValue > 0.1f;
        leftInput = axisValue < -0.1f;
        yield return new WaitForSeconds(0.1f);
        if (Input.GetButton("Horizontal"))
        {
            if (rightInput)
            {
                ActiveSlider.value += 0.01f;
            }
            if (leftInput)
            {
                ActiveSlider.value -= 0.01f;
            }
            PlayerPrefs.SetFloat(ActiveSlider.name, ActiveSlider.value);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            _sliderActive = false;
        }
    }

    void SetActiveButton(List<GameObject> Buttons, float ChangenNum, int ResetValue, int LimitRangeNum)
    {
        if (SelectedButton < Buttons.Count - LimitRangeNum && SelectedButton >= 0 && ChangenNum > 0)
        {
            Buttons[((int)SelectedButton)].GetComponent<Image>().color = Color.grey;
            SelectedButton += ChangenNum;
            Buttons[((int)SelectedButton)].GetComponent<Image>().color = Color.white;
        }
        else if (SelectedButton < Buttons.Count && SelectedButton > 0 && ChangenNum < 0)
        {
            Buttons[((int)SelectedButton)].GetComponent<Image>().color = Color.grey;
            SelectedButton += ChangenNum;
            Buttons[((int)SelectedButton)].GetComponent<Image>().color = Color.white;
        }
        else
        {
            Buttons[((int)SelectedButton)].GetComponent<Image>().color = Color.grey;
            SelectedButton = ResetValue;
            Buttons[((int)SelectedButton)].GetComponent<Image>().color = Color.white;
        }

        if ((int)SelectedButton != 0 && _activeScreen == "Level")
        {
            SetLevelStars((int)SelectedButton);
        }
        else if (_activeScreen == "Level")
        {
            LevelStars[0].SetActive(false);
            LevelStars[1].SetActive(false);
            LevelStars[2].SetActive(false);
        }
    }

    void SetActiveOption(float ChangenNum, int ResetValue)
    {
        if (SelectedButton < OptionButtons.Count - 1 && SelectedButton >= 0 && ChangenNum > 0)
        {
            OptionsSetting(true);
            SelectedButton += ChangenNum;
            OptionsSetting(false);
        }
        else if (SelectedButton < OptionButtons.Count && SelectedButton > 0 && ChangenNum < 0) 
        {
            OptionsSetting(true);
            SelectedButton += ChangenNum;
            OptionsSetting(false);
        }
        else
        {
            OptionsSetting(true);
            SelectedButton = ResetValue;
            OptionsSetting(false);
        }
    }

    void OptionsSetting(bool ActiveOrDeactive)
    {
        if (SelectedButton == 3)
        {
            OptionButtons[((int)SelectedButton)].GetComponent<Toggle>().enabled = ActiveOrDeactive;
        }
        else if (SelectedButton == 0)
        {
            if (ActiveOrDeactive == false)
            {
                OptionButtons[((int)SelectedButton)].GetComponent<Image>().color = Color.white;
            }
            else
            {
                OptionButtons[((int)SelectedButton)].GetComponent<Image>().color = Color.grey;
            }
        }
        else
        {
            OptionButtons[((int)SelectedButton)].GetComponent<Slider>().enabled = ActiveOrDeactive;
        }
    }

    void SetLevelStars(int LevelNum)
    {
        int StarsComplete = PlayerPrefs.GetInt(LevelNum + "Stars", 0);
        if(StarsComplete >= 1)
        {
            LevelStars[0].SetActive(true);
        }
        else
        {
            LevelStars[0].SetActive(false);
        }
        if (StarsComplete >= 2)
        {
            LevelStars[1].SetActive(true);
        }
        else
        {
            LevelStars[1].SetActive(false);
        }
        if (StarsComplete >= 3)
        {
            LevelStars[2].SetActive(true);
        }
        else
        {
            LevelStars[2].SetActive(false);
        }
    }

    IEnumerator SwitchOffCredits()
    {
        yield return new WaitForSeconds(3f);
        if(_activeScreen == "CreditMenu")
        {
            _activeScreen = "Main";
            CreditMenu.SetActive(false);
            Main.SetActive(true);
        }
    }

    void LoadOptions()
    {
        OptionButtons[1].GetComponent<Slider>().value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        OptionButtons[2].GetComponent<Slider>().value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        int i = PlayerPrefs.GetInt("AudioToggle", 1);
        if (i == 1) { OptionButtons[3].GetComponent<Toggle>().isOn = true; }
        else { OptionButtons[3].GetComponent<Toggle>().isOn = false; }
    }

    public void Play()
    {
        _activeScreen = "ControlMenu";
        Main.SetActive(false);
        ControlMenu.SetActive(true);
        MenuButtons[((int)SelectedButton)].GetComponent<Image>().color = Color.grey;
    }

    public void LevelSelectSwitch()
    {
        _activeScreen = "Level";
        ControlMenu.SetActive(false);
        Level.SetActive(true);
        LevelButtons[((int)SelectedButton)].GetComponent<Image>().color = Color.white;
        LevelStars[0].SetActive(false);
        LevelStars[1].SetActive(false);
        LevelStars[2].SetActive(false);
    }

    public void OpenOptionsMenu()
    {
        _activeScreen = "Option";
        Main.SetActive(false);
        Option.SetActive(true);
        MenuButtons[((int)SelectedButton)].GetComponent<Image>().color = Color.grey;
        SetActiveOption(0, 0);
    }

    public void Back()
    {
        LevelButtons[((int)SelectedButton)].GetComponent<Image>().color = Color.grey;

        if(SelectedButton == 3)
        {
            OptionButtons[((int)SelectedButton)].GetComponent<Toggle>().enabled = true;
        }
        else if (SelectedButton == 0)
        {
            OptionButtons[((int)SelectedButton)].GetComponent<Image>().color = Color.white;
        }
        else
        {
            OptionButtons[((int)SelectedButton)].GetComponent<Slider>().enabled = true;
        }

        _activeScreen = "Main";
        Main.SetActive(true);
        Level.SetActive(false);
        Option.SetActive(false);

        SelectedButton = 0;
        MenuButtons[((int)SelectedButton)].GetComponent<Image>().color = Color.white;
    }

    public void SwitchScene(int DifficultyLevel)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + DifficultyLevel);
    }

    // Quit Game
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player Has Quit The Game");
    }
}
