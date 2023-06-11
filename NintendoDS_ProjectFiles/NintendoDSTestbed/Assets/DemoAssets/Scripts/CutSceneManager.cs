using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour, IDSTapListener
{
    public List<GameObject> Scene;
    public GameObject TaskManager;
    public Image BGTop;
    public Image BGBottom;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnScenes(0));
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            TaskManager.SetActive(true);
            foreach (GameObject x in Scene)
            {
                x.SetActive(false);
            }
            BGTop.color = Color.white;
            BGBottom.color = Color.white;
        }
    }

    public void OnScreenTapDown(Vector2 tapPosition) { }

    // Called when continously as Screen is dragged on
    public void OnScreenDrag(Vector2 tapPosition) { }

    // Called Once when the tap is lifted up from the Screen 
    public void OnScreenTapUp(Vector2 tapPosition)
    {
        TaskManager.SetActive(true);
        foreach (GameObject x in Scene)
        {
            x.SetActive(false);
        }
        BGTop.color = Color.white;
        BGBottom.color = Color.white;
    }

    IEnumerator SpawnScenes(int SceneCount)
    {
        Scene[SceneCount].SetActive(true);
        yield return new WaitForSeconds(1f);
        if (Scene.Count - 1 > SceneCount)
        {
            SceneCount++;
            if (BGTop.color != Color.white)
            {
                StartCoroutine(SpawnScenes(SceneCount));
            }
        }
        else
        {
            TaskManager.SetActive(true);
            foreach (GameObject x in Scene)
            {
                x.SetActive(false);
            }
            BGTop.color = Color.white;
            BGBottom.color = Color.white;
        }
    }
}
