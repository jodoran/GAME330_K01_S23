using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BrokenLevel : MonoBehaviour
{
    private void Update()
    {

        if (FigmentInput.GetButtonUp(FigmentInput.FigmentButton.LeftButton))
        {
            SceneManager.LoadScene("Menu");
        }
        else if (FigmentInput.GetButtonUp(FigmentInput.FigmentButton.RightButton))
        {
            SceneManager.LoadScene("Menu");
        }
        else if (FigmentInput.GetButtonDown(FigmentInput.FigmentButton.ActionButton))
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
