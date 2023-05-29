using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct Task
{
    public GameObject TopScreen;
    public GameObject BottomScreen;
}

public class TaskManager : MonoBehaviour
{
    public List<Task> Tasks;
    public List<GameObject> Stars;
    public Slider ProgressBar;
    public GameObject MainAudioTrack;
    [HideInInspector]
    public bool TaskComplete;
    [HideInInspector]
    public float Timer;
    private int TaskNum = 0;

    private void Start()
    {
        ProgressBar.gameObject.SetActive(true);
        Tasks[TaskNum].TopScreen.SetActive(true);
        Tasks[TaskNum].BottomScreen.SetActive(true);
    }

    public void ChangeTask()
    {
        Tasks[TaskNum].TopScreen.SetActive(false);
        Tasks[TaskNum].BottomScreen.SetActive(false);
        TaskNum++;
        Tasks[TaskNum].TopScreen.SetActive(true);
        Tasks[TaskNum].BottomScreen.SetActive(true);

        if(TaskNum >= Tasks.Count)
        {
            MainAudioTrack.SetActive(false);
        }
    }

    public void JumpToEnd()
    {
        Tasks[TaskNum].TopScreen.SetActive(false);
        Tasks[TaskNum].BottomScreen.SetActive(false);
        TaskNum = Tasks.Count - 1;
        Tasks[TaskNum].TopScreen.SetActive(true);
        Tasks[TaskNum].BottomScreen.SetActive(true);
        this.enabled = false;
    }

    private void Update()
    {
        Timer += Time.deltaTime;

        if (TaskComplete)
        {
            ChangeTask();
            TaskComplete = false;
        }

        if (ProgressBar.value > 0)
        {
            if (ProgressBar.value >= 1)
            {
                Stars[0].SetActive(true);
                Stars[1].SetActive(true);
                Stars[2].SetActive(true);
            }
            else if (ProgressBar.value >= .75f)
            {
                Stars[0].SetActive(false);
                Stars[1].SetActive(true);
                Stars[2].SetActive(true);
            }
            else if (ProgressBar.value >= .35f)
            {
                Stars[0].SetActive(false);
                Stars[1].SetActive(false);
                Stars[2].SetActive(true);
            }
            else
            {
                Stars[0].SetActive(false);
                Stars[1].SetActive(false);
                Stars[2].SetActive(false);
            }
        }
        else
        {
            JumpToEnd();
        }
    }
}
