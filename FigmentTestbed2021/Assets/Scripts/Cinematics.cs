using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cinematics : MonoBehaviour
{
    public List<GameObject> Scenes = new List<GameObject>();
    public GameObject Player;

    private bool End;

    private void Start()
    {
        StartCoroutine(PlayScenes());
    }

    private void Update()
    {
        if (FigmentInput.GetButtonUp(FigmentInput.FigmentButton.LeftButton))
        {
            Player.GetComponent<PlayerMovement2D>().enabled = true;
            this.transform.parent.gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else if (FigmentInput.GetButtonUp(FigmentInput.FigmentButton.RightButton))
        {
            Player.GetComponent<PlayerMovement2D>().enabled = true;
            this.transform.parent.gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else if (FigmentInput.GetButtonDown(FigmentInput.FigmentButton.ActionButton))
        {
            Player.GetComponent<PlayerMovement2D>().enabled = true;
            this.transform.parent.gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    IEnumerator PlayScenes()
    {
        foreach (GameObject scene in Scenes)
        {
            Image _scene = scene.GetComponent<Image>();

            scene.SetActive(true);
            yield return new WaitForSeconds(4f);

            _scene.CrossFadeColor(Color.clear, 2f, false, true);
            yield return new WaitForSeconds(2f);
            scene.SetActive(false);
        }
        Image Background = this.transform.GetComponentInParent<Image>();
        Background.CrossFadeColor(Color.clear, 2f, false, true);
        yield return new WaitForSeconds(2f);
        this.transform.parent.gameObject.SetActive(false);
        Player.GetComponent<PlayerMovement2D>().enabled = true;
    }
}
