using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour
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

    IEnumerator SpawnScenes(int SceneCount)
    {
        Scene[SceneCount].SetActive(true);
        yield return new WaitForSeconds(1f);
        if (Scene.Count-1 > SceneCount)
        {
            SceneCount++;
            StartCoroutine(SpawnScenes(SceneCount));
        }
        else
        {
            TaskManager.SetActive(true);
            foreach(GameObject x in Scene)
            {
                x.SetActive(false);
            }
            BGTop.color = Color.white;
            BGBottom.color = Color.white;
        }
    }
}
