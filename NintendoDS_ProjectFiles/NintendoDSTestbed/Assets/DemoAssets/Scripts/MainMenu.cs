using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour, IDSTapListener
{
    //Used to get the positions of the Buttons
    public RectTransform PlayButton;
    public RectTransform QuitButton;

    public AudioSource ButtonClickAudio;

    // Called Once when Screen is tapped down on
    public void OnScreenTapDown(Vector2 tapPosition)
    {
        //Checks the position of the button and compares it to the tapped position
        if (DSTapRouter.RectangleContainsDSPoint(PlayButton, tapPosition))
        {
            ButtonClickAudio.Play();
            Play(); //Calls Play if Play Button is tapped on
        }
        else if (DSTapRouter.RectangleContainsDSPoint(QuitButton, tapPosition))
        {
            ButtonClickAudio.Play();
            Quit(); //Calls Quit if Quit Button is tapped on
        }
    }

    // Called when continously as Screen is dragged on
    public void OnScreenDrag(Vector2 tapPosition) { }

    // Called Once when the tap is lifted up from the Screen 
    public void OnScreenTapUp(Vector2 tapPosition) { }



    // Load Scene
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Quit Game
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player Has Quit The Game");
    }
}
