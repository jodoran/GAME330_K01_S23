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
    [HideInInspector]
    public bool TaskComplete;
    [HideInInspector]
    public float Timer;
    private int TaskNum = 0;

    private void Start()
    {
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
    }

    public void JumpToEnd()
    {
        Tasks[TaskNum].TopScreen.SetActive(false);
        Tasks[TaskNum].BottomScreen.SetActive(false);
        TaskNum = Tasks.Count - 1;
        Tasks[TaskNum].TopScreen.SetActive(true);
        Tasks[TaskNum].BottomScreen.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (TaskComplete)
        {
            ChangeTask();
            TaskComplete = false;
        }

        if (ProgressBar.value > 0)
        {
            switch (ProgressBar.value)
            {
                case 1:
                    Stars[0].SetActive(true);
                    Stars[1].SetActive(true);
                    Stars[2].SetActive(true);
                    break;
                case 0.75f:
                    Stars[0].SetActive(false);
                    Stars[1].SetActive(true);
                    Stars[2].SetActive(true);
                    break;
                case 0.35f:
                    Stars[0].SetActive(false);
                    Stars[1].SetActive(false);
                    Stars[2].SetActive(true);
                    break;
            }
        }
        else
        {
            JumpToEnd();
        }
    }

    private void Update()
    {
        Timer += Time.deltaTime;
    }
}
