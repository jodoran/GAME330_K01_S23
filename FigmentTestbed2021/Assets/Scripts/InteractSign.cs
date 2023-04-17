using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractSign : MonoBehaviour
{
    public GameObject InteractText;
    public GameObject SignScreen;
    public string _instructions;

    private bool _active;

    private void Update()
    {
        if (FigmentInput.GetButtonDown(FigmentInput.FigmentButton.ActionButton) && _active == true)
        {
            if (!SignScreen.activeSelf)
            {
                SignScreen.SetActive(true);
                SignScreen.transform.GetComponentInChildren<Text>().text = _instructions;
            }
            else
            {
                SignScreen.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            InteractText.SetActive(true);
            _active = true;
            collision.GetComponent<PlayerMovement2D>().DisableJump();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            InteractText.SetActive(false);
            SignScreen.SetActive(false);
            _active = false;
            collision.GetComponent<PlayerMovement2D>().EnableJump();
        }
    }
}
