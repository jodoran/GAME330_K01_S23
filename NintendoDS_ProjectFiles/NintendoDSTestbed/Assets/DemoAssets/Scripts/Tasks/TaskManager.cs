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

public class TaskManager : MonoBehaviour, IDSTapListener
{
    public bool FailEnabled;
    public bool HealingEnabled;

    public List<Task> Tasks;
    public List<GameObject> Stars;
    public Slider ProgressBar;

    public GameObject MainAudioTrack;
    public GameObject IntermissionScreen;

    [HideInInspector]
    public bool TaskComplete;
    [HideInInspector]
    public float Timer;

    public string LevelDifficulty;
    private bool WaitingForInteraction;
    private int TaskNum = 0;

    private void Start()
    {
        ProgressBar.gameObject.SetActive(true);
        Tasks[TaskNum].TopScreen.SetActive(true);
        IntermissionScreen.SetActive(true);
        WaitingForInteraction = true;
    }

    public void OnScreenTapDown(Vector2 tapPosition)
    {
        if (WaitingForInteraction)
        {
            Tasks[TaskNum].BottomScreen.SetActive(true);

            if (TaskNum >= Tasks.Count - 1)
            {
                MainAudioTrack.SetActive(false);
                PlayerPrefs.SetInt(LevelDifficulty + "Complete", 1);
            }
            WaitingForInteraction = false;
            IntermissionScreen.SetActive(false);
        }
    }

    public void OnScreenDrag(Vector2 tapPosition) { }

    public void OnScreenTapUp(Vector2 tapPosition) { }


    public void ChangeTask()
    {
        Tasks[TaskNum].TopScreen.SetActive(false);
        Tasks[TaskNum].BottomScreen.SetActive(false);
        TaskNum++;
        Tasks[TaskNum].TopScreen.SetActive(true);
        IntermissionScreen.SetActive(true);
        WaitingForInteraction = true;
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
            if (FailEnabled)
            {
                JumpToEnd();
            }
        }

        if (WaitingForInteraction)
        {
            if (Input.anyKeyDown)
            {
                Tasks[TaskNum].BottomScreen.SetActive(true);

                if (TaskNum >= Tasks.Count)
                {
                    MainAudioTrack.SetActive(false);
                }
                else
                {
                    WaitingForInteraction = false;
                    IntermissionScreen.SetActive(false);
                }
            }
        }
    }
}
