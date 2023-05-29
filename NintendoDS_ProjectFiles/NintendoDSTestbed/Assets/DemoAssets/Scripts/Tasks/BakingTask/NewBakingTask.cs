using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBakingTask : MonoBehaviour, IDSTapListener
{
    public GameObject OvenOPEN;
    public GameObject OvenOFF;
    public GameObject OvenON;

    public Slider ProgressBar;
    public GameObject TaskManager;
    public Text Instructions;
    public Slider BakeTimeBar;
    public RectTransform Plate;
    public RectTransform Pan;
    public RectTransform Cake;
    public float _maxBake;
    public float _minBake;
    int TaskNum = 1;
    bool _increase = true;
    bool _decrease = false;

    private void Start()
    {
        Instructions.text = "Click to open the oven";
    }

    private void Update()
    {
        if (TaskNum == 3)
        {
            if (_increase)
            {
                BakeTimeBar.value += Time.deltaTime;
            }
            else if (_decrease)
            {
                BakeTimeBar.value -= Time.deltaTime;
            }

            if (BakeTimeBar.value >= 1)
            {
                _decrease = true;
                _increase = false;
            }
            if (BakeTimeBar.value <= 0)
            {
                _decrease = false;
                _increase = true;
            }
        }
    }

    public void OnScreenTapDown(Vector2 tapPosition)
    {

    }

    // Called when continously as Screen is dragged on
    public void OnScreenDrag(Vector2 tapPosition)
    {
        if (TaskNum == 2)
        {
            if (DSTapRouter.RectangleContainsDSPoint(Pan, tapPosition))
            {
                MovePlateWithCursor(Pan, tapPosition, 120, 90);
            }
        }
        else if (TaskNum == 5)
        {
            if (DSTapRouter.RectangleContainsDSPoint(Cake, tapPosition))
            {
                MovePlateWithCursor(Cake, tapPosition, 120, 90);
            }
        }
    }

    // Called Once when the tap is lifted up from the Screen 
    public void OnScreenTapUp(Vector2 tapPosition)
    {
        if (TaskNum == 1)
        {
            Instructions.text = "Drag the pan into the oven";
            OvenOFF.SetActive(false);
            OvenOPEN.SetActive(true);
            Pan.gameObject.SetActive(true);
            TaskNum++;
        }
        else if (TaskNum == 2)
        {
            if (rectOverlaps(Pan, OvenOPEN.GetComponent<RectTransform>()))
            {
                Pan.gameObject.SetActive(false);
                TaskNum++;
                OvenON.SetActive(true);
                OvenOPEN.SetActive(false);
                Instructions.text = "Click when the line is in the green";
            }
        }
        else if (TaskNum == 3)
        {
            if (BakeTimeBar.value <= _maxBake &&
                BakeTimeBar.value >= _minBake)
            {
                OvenON.SetActive(false);
                OvenOFF.SetActive(true);
                TaskNum++;
                Instructions.text = "Click to open the oven";
            }
            else
            {
                ProgressBar.value -= 0.25f;
            }
        }
        else if (TaskNum == 4)
        {
            OvenOFF.SetActive(false);
            OvenOPEN.SetActive(true);
            Cake.gameObject.SetActive(true);
            Plate.gameObject.SetActive(true);
            Instructions.text = "Drag the cake onto the plate";
            TaskNum++;
        }
        else if (TaskNum == 5)
        {
            if (rectOverlaps(Cake, Plate.GetComponent<RectTransform>()))
            {
                Cake.gameObject.SetActive(false);
                Plate.gameObject.SetActive(false);
                Instructions.text = "";
                TaskNum++;
                OvenOFF.SetActive(true);
                OvenOPEN.SetActive(false);
                TaskManager.GetComponent<TaskManager>().TaskComplete = true;
            }
        }
    }


    void MovePlateWithCursor(RectTransform plate, Vector3 cursorPosition, float Xoffset, float Yoffset) //Sets plate to position of the cursor
    {
        // Gets Mouse position
        Vector3 currPosition = plate.anchoredPosition;
        currPosition.x += (cursorPosition.x - (currPosition.x + Xoffset));
        currPosition.y += (cursorPosition.y - (currPosition.y + Yoffset));

        // Sets plate rect transform to that position
        plate.anchoredPosition = currPosition;
    }

    bool rectOverlaps(RectTransform rectTrans1, RectTransform rectTrans2) //Gets if the 1st RectTransform is overlapping the 2nd RectTransform
    {
        // recreates both RectTransforms poitions current position and size
        Rect rect1 = new Rect(rectTrans1.anchoredPosition.x, rectTrans1.anchoredPosition.y, rectTrans1.rect.width, rectTrans1.rect.height);
        Rect rect2 = new Rect(rectTrans2.anchoredPosition.x, rectTrans2.anchoredPosition.y, rectTrans2.rect.width, rectTrans2.rect.height);

        // uses the recreated RectTransforms positions and sizes to check if they are overlapping
        return rect1.Overlaps(rect2);
    }
}
