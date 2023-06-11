using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MixingTask : MonoBehaviour, IDSTapListener
{
    public Slider QualityBar;
    public Slider ProgressBar;
    public AudioSource MixSoundEffect;
    public AudioSource MissSound;

    List<Vector3> PointList = new List<Vector3>();

    public int Cycles = 1;
    public int CircleRadius = 5;
    public int Points;
    public float MinQuality;

    private Image Point;
    private int PointsClicked = 0;
    private float Progress = 1;
    private float Quality = 0;
    private GameObject TaskManager;
    private bool _stiring;
    private float _timer;

    private void Start()
    {
        Point = this.GetComponent<Image>();
        TaskManager = GameObject.Find("TaskManager");

        float angle = 0;
        float AngleIntervals = 360/ Points;
        while (PointList.Count < Points * Cycles)
        {
            Vector3 nextPoistion;
            float x = CircleRadius * Mathf.Cos(angle);
            float y = CircleRadius * Mathf.Sin(angle);
            nextPoistion = new Vector3(x * CircleRadius, y * CircleRadius, 0);
            PointList.Add(nextPoistion);
            angle += AngleIntervals;
        }

        SetPoint();
    }

    private void Update()
    {
        if (Quality > 0)
        {
            Quality -= Time.deltaTime / 5f;
            QualityBar.value = Quality;

            if (Quality >= .9f)
            {
                Point.color = Color.green;
            }
            else
            {
                Point.color = Color.red;
            }
        }

        if (_stiring)
        {
            _timer += Time.deltaTime;
            if(_timer >= 3f)
            {
                MixSoundEffect.Stop();
                _stiring = false;
                _timer = 0;
            }
        }
    }

    public void OnScreenTapDown(Vector2 tapPosition)
    {
        if (DSTapRouter.RectangleContainsDSPoint(GetComponent<RectTransform>(), tapPosition))
        {
            if (PointsClicked < PointList.Count)
            {
                SetPoint();
                Quality = 1;
                ModifyProgress();
                if (!_stiring)
                {
                    MixSoundEffect.Play();
                    _stiring = true;
                }
                _timer = 0;
            }
            else if(PointsClicked == PointList.Count)
            {
                ModifyProgress();
                MixSoundEffect.Stop();
                _stiring = false;
                TaskManager.GetComponent<TaskManager>().TaskComplete = true;
            }
        }
    }

    public void OnScreenDrag(Vector2 tapPosition)
    {
        OnScreenTapDown(tapPosition);
    }

    public void OnScreenTapUp(Vector2 tapPosition)
    {
        MixSoundEffect.Stop();
        _stiring = false;
        _timer = 0;
    }

    void ModifyProgress()
    {
        if (QualityBar.value < MinQuality)
        {
            MissSound.Play();
            Progress -= 0.1f;
            ProgressBar.value = Progress;
        }
        else if (TaskManager.GetComponent<TaskManager>().HealingEnabled)
        {
            Progress += 0.05f;
            ProgressBar.value = Progress;
        }
    }

    void SetPoint()
    {
        Vector3 currPosition = Point.rectTransform.anchoredPosition;
        currPosition.x += (PointList[PointsClicked].x - currPosition.x);
        currPosition.y += (PointList[PointsClicked].y - currPosition.y);
        Point.rectTransform.anchoredPosition = currPosition;

        PointsClicked++;
    }
}
