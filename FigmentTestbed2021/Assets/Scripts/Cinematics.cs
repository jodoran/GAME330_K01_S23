using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cinematics : MonoBehaviour
{
    public List<GameObject> Scenes = new List<GameObject>();
    public GameObject Player;

    public bool _startScene;

    public bool End;

    private void Start()
    {
        if (_startScene) { StartCoroutine(PlayScenes()); }
    }

    private void Update()
    {
        if (FigmentInput.GetButtonDown(FigmentInput.FigmentButton.LeftButton))
        {
            EndScene();
        }
        else if (FigmentInput.GetButtonDown(FigmentInput.FigmentButton.RightButton))
        {
            EndScene();
        }
        else if (FigmentInput.GetButtonDown(FigmentInput.FigmentButton.ActionButton))
        {
            EndScene();
        }
    }

    void EndScene()
    {
        Player.GetComponent<PlayerMovement2D>().enabled = true;
        gameObject.SetActive(false);
        End = true;
        this.transform.parent.gameObject.SetActive(false);
    }

    public IEnumerator PlayScenes()
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
        End = true;
    }
}
