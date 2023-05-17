using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MixingTask : MonoBehaviour, IDSTapListener
{
    public Image Point;

    public Vector3 MixingPoint;
    List<Vector3> PointList = new List<Vector3>();
    public int PointCount = 2;
    public int CircleRadius = 5;
    public int Points;
    int PointsClicked = 0;
    float Progress = 0;

    private void Start()
    {
        float angle = 0;
        float AngleIntervals = 360/ Points ;
        while (PointList.Count < Points)
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

    public void OnScreenTapDown(Vector2 tapPosition)
    {

        if (DSTapRouter.RectangleContainsDSPoint(GetComponent<RectTransform>(), tapPosition))
        {
            if (PointsClicked < PointList.Count)
            {
                SetPoint();
            }
            else
            {
                Point.enabled = false;
            }
        }
        
    }

    public void OnScreenDrag(Vector2 tapPosition)
    {
        OnScreenTapDown(tapPosition);
    }

    public void OnScreenTapUp(Vector2 tapPosition)
    {
        Debug.Log("ScreenTapUp at " + tapPosition);
    }


    void SetPoint()
    {
        Vector3 currPosition = Point.rectTransform.anchoredPosition;
        currPosition.x += (PointList[PointsClicked].x - currPosition.x);
        currPosition.y += (PointList[PointsClicked].y - currPosition.y);
        Point.rectTransform.anchoredPosition = currPosition;
        PointsClicked++;
        Progress += PointList.Count / 100;
    }
}
