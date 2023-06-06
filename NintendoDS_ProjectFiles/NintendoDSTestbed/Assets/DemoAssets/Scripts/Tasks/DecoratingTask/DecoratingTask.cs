using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecoratingTask : MonoBehaviour, IDSTapListener
{
    public GameObject TaskManager;
    public Slider ProgressBar;
    public Slider FillBar;
    public Text FillPercent;
    public GameObject Cake;
    public GameObject IcingParent;
    public GameObject DecorationTask;
    public List<RectTransform> Decorations;
    public List<RectTransform> DecorationPlacements;
    public float _icingFillArea;
    public float _icingFillSpeed;
    public float _icingFillPortions;

    List<bool> DecorationChecks = new List<bool>();
    List<Vector3> PointList = new List<Vector3>();
    List<bool> PointCheck = new List<bool>();

    Vector3 _originalPos;
    GameObject IcingPlot;
    float _icingFilled;
    bool _icingPlaced;
    string _currentStep = "Icing";

    private void Start()
    {
        float radiusIntervals = 0;
        for (int j = 0; j < 5; j++)
        {
            radiusIntervals++;
            float angle = 0;
            float AngleIntervals = 360 / 5 * radiusIntervals;
            for (int i = 0; i <= (5 * radiusIntervals); i++)
            {
                Vector3 nextPoistion;
                float x = ((radiusIntervals/2.2f) * Mathf.Cos(angle));
                float y = ((radiusIntervals/2.2f) * Mathf.Sin(angle));
                x += Cake.transform.position.x;
                y += Cake.transform.position.y;
                nextPoistion = new Vector3(x, y, 6);
                PointList.Add(nextPoistion);
                PointCheck.Add(false);
                angle += AngleIntervals;
            }
        }
        DrawIcingLineCheck();

        for(int i = 0; i < 4; i++)
        {
            DecorationChecks.Add(false);
        }
    }

    public void OnScreenTapDown(Vector2 tapPosition)
    {
        if(_currentStep == "Icing")
        {
            if (DSTapRouter.RectangleContainsDSPoint(Cake.GetComponent<RectTransform>(), tapPosition))
            {
                GameObject _icing = Resources.Load<GameObject>("Tasks/DecorateTask/Icing");
                _icing = Instantiate(_icing, tapPosition, Quaternion.identity, IcingParent.transform);
                Vector3 currPosition = IcingParent.GetComponent<RectTransform>().anchoredPosition;
                currPosition.x += (tapPosition.x - (currPosition.x + 130));
                currPosition.y += (tapPosition.y - (currPosition.y + 100));
                _icing.GetComponent<RectTransform>().anchoredPosition = currPosition;
                currPosition = new Vector3 (_icing.GetComponent<RectTransform>().position.x, _icing.GetComponent<RectTransform>().position.y, 30);
                _icing.GetComponent<RectTransform>().position = currPosition;
                IcingPlot = _icing;
                _icingPlaced = true;
                StartCoroutine(IcingStep());
            }
        }
        else if(_currentStep == "Decorate")
        {
            for(int i = 0; i < Decorations.Count; i++) 
            { 
                if (DSTapRouter.RectangleContainsDSPoint(Decorations[i], tapPosition))
                {
                    if (DecorationChecks[i] == false)
                    {
                        _originalPos = Decorations[i].anchoredPosition;
                        MovePlateWithCursor(Decorations[i], tapPosition, 120, 90);
                    }
                }
            }
        }
    }

    public void OnScreenDrag(Vector2 tapPosition)
    {
        if (_currentStep == "Decorate")
        {
            for (int i = 0; i < Decorations.Count; i++)
            {
                if (DSTapRouter.RectangleContainsDSPoint(Decorations[i], tapPosition))
                {
                    if (DecorationChecks[i] == false)
                    {
                        MovePlateWithCursor(Decorations[i], tapPosition, 120, 90);
                    }
                }
            }
        }
    }

    public void OnScreenTapUp(Vector2 tapPosition) 
    {
        if (_currentStep == "Icing")
        {
            _icingPlaced = false;
            DrawIcingLineCheck();
        }
        else if (_currentStep == "Decorate")
        {
            for(int i = 0; i < Decorations.Count; i++)
            {
                if (rectOverlaps(Decorations[i], DecorationPlacements[i]))
                {
                    DecorationChecks[i] = true;
                }
                else
                {
                    ProgressBar.value -= 0.05f;
                    Decorations[i].anchoredPosition = _originalPos;
                }
            }
        }
    }

    private void Update()
    {
        if (_currentStep == "Icing" && _icingFilled == 80)
        {
            _currentStep = "Decorate";
            DecorationTask.SetActive(true);
        }
        if (_currentStep == "Decorate")
        {
            float checks = 0;
            for (int i = 0; i < DecorationChecks.Count; i++)
            {
                if(DecorationChecks[i] == true)
                {
                    checks += 1;
                }
            }
            if(checks == Decorations.Count)
            {
                TaskManager.GetComponent<TaskManager>().TaskComplete = true;
            }
        }
    }

    void DrawIcingLineCheck()
    {
        _icingFilled = 0;
        for (int i = 0; i < PointList.Count; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(PointList[i], -Vector3.forward);
            Debug.DrawRay(PointList[i], -Vector3.forward, Color.red, 500);

            if (hit.collider != null)
            {
                PointCheck[i] = true;
                _icingFilled++;
            }
            else
            {
                PointCheck[i] = false;
            }
        }
        FillBar.value = _icingFilled / 80;
        float FillPrecent = Mathf.RoundToInt((_icingFilled / 80) * 100);
        FillPercent.text = FillPrecent.ToString() + "%";
    }


    IEnumerator IcingStep()
    {
        while (_icingPlaced)
        {
            Vector3 _scale = IcingPlot.GetComponent<RectTransform>().localScale;
            if (_scale.x < _icingFillArea)
            {
                IcingPlot.GetComponent<RectTransform>().localScale += new Vector3(_scale.x += _icingFillPortions, _scale.y += _icingFillPortions, _scale.z);
            }
            yield return new WaitForSeconds(_icingFillSpeed);
        }

    }

    void CheckDecorations()
    {

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
