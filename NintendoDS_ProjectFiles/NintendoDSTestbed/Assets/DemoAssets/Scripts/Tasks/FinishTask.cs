using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinishTask : MonoBehaviour, IDSTapListener
{
    public Slider ProgressBar;
    public AudioSource WinSound;
    public AudioSource LoseSound;
    public Text WinText;
    public RectTransform MenuButtonSuccess;
    public RectTransform MenuButtonFail;
    public List<GameObject> Stars;

    public List<GameObject> LoseScreens;
    public List<GameObject> WinScreens;

    private float Delay;

    private void Start()
    {
        if (ProgressBar.value > 0)
        {
            WinScreens[0].SetActive(true);
            WinScreens[1].SetActive(true);
            WinSound.Play();
            if (ProgressBar.value >= 1) {
                Stars[0].SetActive(true);
            }
            if (ProgressBar.value >= .75f)
            {
                Stars[1].SetActive(true);
            }
            if (ProgressBar.value >= .35f)
            {
                Stars[2].SetActive(true);
            }

            float time = GameObject.Find("TaskManager").GetComponent<TaskManager>().Timer;
            time = (Mathf.Round(time * 100)) / 100;
            WinText.text = "Cake Complete!\n" + "Time: " + time.ToString();
        }
        else
        {
            WinSound.Stop();
            LoseScreens[0].SetActive(true);
            LoseScreens[1].SetActive(true);
        }
    }

    private void Update()
    {
        Delay += Time.deltaTime;

        if (Input.anyKeyDown)
        {
            if (Delay >= 1.5f)
            {
                ChangeScene(); 
            }
        }
    }

    public void OnScreenTapDown(Vector2 tapPosition) { }

    // Called when continously as Screen is dragged on
    public void OnScreenDrag(Vector2 tapPosition) { }

    // Called Once when the tap is lifted up from the Screen 
    public void OnScreenTapUp(Vector2 tapPosition) 
    {
        //Checks the position of the button and compares it to the tapped position
        if (DSTapRouter.RectangleContainsDSPoint(MenuButtonSuccess, tapPosition)
            || DSTapRouter.RectangleContainsDSPoint(MenuButtonFail, tapPosition))
        {
            if (Delay >= 1.5f)
            {
                ChangeScene(); //Calls Play if Play Button is tapped on
            }
        }
    }

    // Load Scene
    public void ChangeScene()
    {
        SceneManager.LoadScene(0);
    }
}
