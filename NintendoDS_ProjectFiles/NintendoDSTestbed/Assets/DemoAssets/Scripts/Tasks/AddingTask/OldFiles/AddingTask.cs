using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Fetches each color from color list in inspector re-assigning the colors randomly to set the order of plates that need to be added.
/// Sets the current plate that needs to be added to the bowl by setting the color indicatator to the assigned plate color.
/// Gets if a plate is being held from AddingPlateInfo which assigns PlateInfo variable to check if plate is being dragged.
/// If being dragged moves plate object with mouse cursor and if overlapping with the mixing bowl and released it removes plate object.
/// While if plate is released and isn't over the bowl it decreases the quality value and resets the plate.
/// Upon all plates being added marks task as complete. 
/// </summary>

//Plate Info containing variables used to describe the plate object
class PlateInfo
{
    public GameObject PlateObject;
    public RectTransform PlatePosition;
    public Vector3 PlateOrigin;
    public Color PlateColor;
    public Vector3 PlateOffset;
}

//Used in public list for setting the plates needed to be added and their corolated color
[Serializable]
public struct ColorValue
{
    public Color PlateColor;
    public Image Ingredient;
    public string Name;
}


// MonoBehaviour to handle main program and functions of the code
public class AddingTask : MonoBehaviour, IDSTapListener
{
    Dictionary<Vector3, string> ColorValues = new Dictionary<Vector3, string>();

    Dictionary<int, GameObject> PlateObjects = new Dictionary<int, GameObject>(); 

    //Object Variables
    public Image Bowl;
    public Text Instructions;
    public Slider ProgressBar;

    //Key Variable containing all plates in scene
    public List<ColorValue> Plates = new List<ColorValue>();

    //Randomized list for order plates are added
    List<Color> PlateColorOrder = new List<Color>();

    //Info of currently selected plate
    PlateInfo PlateEquipped = new PlateInfo();

    //Objective & Info Variables
    bool PlateHeld;
    int PlateNum;
    int PlatesEmptied;
    GameObject TaskManager;

    //Base Function
    private void Start() 
    {
        TaskManager = GameObject.Find("TaskManager");
        //Sets Color Dictionary to be pulled from
        SetIngredientDictionary();
        //Randomizes the plate colors order for task objective
        SortColors();
        //Sets the instructions to the first plates values
        Bowl.color = PlateColorOrder[PlatesEmptied];
        SetColorIndicator(Bowl.color);
        StartCoroutine(SetPlateDictionary());
    }

    //IDSTapListener Functions
    public void OnScreenTapDown(Vector2 tapPosition) {

    } 

    public void OnScreenDrag(Vector2 tapPosition) //Gets Mouse Drag
    {
        if (PlateHeld) //Checks if Plate is being held & if so moves plate with cursor
        {
            MovePlateWithCursor(PlateEquipped.PlatePosition, tapPosition, PlateEquipped.PlateOffset.x, PlateEquipped.PlateOffset.y);
        }
    }

    public void OnScreenTapUp(Vector2 tapPosition) //Gets Mouse Up
    {
        if (PlateHeld) //Checks if plate is being held & Calls Reset Plate function
        {
            ResetPlate(CheckPlatePos());
        }
        PlateObjects[PlateNum].gameObject.GetComponent<AddingPlateInfo>().SetPlateHeldInParent();
    }

    public void Update()
    {

        /* uses the d-pad
        float axisValue = Input.GetAxisRaw("Horizontal");
        bool rightInput = false;
        bool leftInput = false;
        rightInput = axisValue > 0.1f;
        leftInput = axisValue < -0.1f;

        if (Input.GetButtonDown("Horizontal"))
        {
            if (leftInput && PlateNum < PlateObjects.Count - 1)
            {
                PlateEquipped.PlateObject.transform.GetChild(0).gameObject.SetActive(true);
                PlateNum++;
                PlateObjects[PlateNum].gameObject.GetComponent<AddingPlateInfo>().SetPlateHeldInParent();
            }
            else if (rightInput && PlateNum > 0)
            {
                PlateEquipped.PlateObject.transform.GetChild(0).gameObject.SetActive(true);
                PlateNum--;
                PlateObjects[PlateNum].gameObject.GetComponent<AddingPlateInfo>().SetPlateHeldInParent();
            }
        }
        */

        if (Input.GetButtonDown("Fire1") && PlateObjects[0].activeSelf)
        {
            if (PlateHeld) //Checks if plate is being held & Calls Reset Plate function
            {
                if (PlateEquipped.PlateObject == PlateObjects[0])
                {
                    ResetPlate(CheckPlateColor());
                }
                else
                {
                    PlateEquipped.PlateObject.transform.GetChild(0).gameObject.SetActive(true);
                    PlateObjects[0].gameObject.GetComponent<AddingPlateInfo>().SetPlateHeldInParent();
                }
            }
        }
        if (Input.GetButtonDown("Fire3") && PlateObjects[1].activeSelf)
        {
            if (PlateHeld) //Checks if plate is being held & Calls Reset Plate function
            {
                if (PlateEquipped.PlateObject == PlateObjects[1])
                {
                    ResetPlate(CheckPlateColor());
                }
                else
                {
                    PlateEquipped.PlateObject.transform.GetChild(0).gameObject.SetActive(true);
                    PlateObjects[1].gameObject.GetComponent<AddingPlateInfo>().SetPlateHeldInParent();
                }
            }
        }
        if (Input.GetButtonDown("Jump") && PlateObjects[2].activeSelf)
        {
            if (PlateHeld) //Checks if plate is being held & Calls Reset Plate function
            {
                if (PlateEquipped.PlateObject == PlateObjects[2])
                {
                    ResetPlate(CheckPlateColor());
                }
                else
                {
                    PlateEquipped.PlateObject.transform.GetChild(0).gameObject.SetActive(true);
                    PlateObjects[2].gameObject.GetComponent<AddingPlateInfo>().SetPlateHeldInParent();
                }
            }
        }
        if (Input.GetButtonDown("Fire2") && PlateObjects[3].activeSelf)
        {
            if (PlateHeld) //Checks if plate is being held & Calls Reset Plate function
            {
                if (PlateEquipped.PlateObject == PlateObjects[3])
                {
                    ResetPlate(CheckPlateColor());
                }
                else
                {
                    PlateEquipped.PlateObject.transform.GetChild(0).gameObject.SetActive(true);
                    PlateObjects[3].gameObject.GetComponent<AddingPlateInfo>().SetPlateHeldInParent();
                }
            }
        }
    }

    IEnumerator SetPlateDictionary()
    {
        yield return new WaitForSeconds(0.1f);
        PlateObjects[PlateNum].gameObject.GetComponent<AddingPlateInfo>().SetPlateHeldInParent();
    }

    public void SetPlateObjects(int ObjectNum, GameObject PlateObject)
    {
        PlateObjects.Add(ObjectNum, PlateObject);
    }

    public void ChangeActivePlateColor()
    {
        if (PlateEquipped.PlateObject != null)
        {
            PlateEquipped.PlateObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    //Public Functions
    public void SetPlateHeld(GameObject plateObject, RectTransform platePosition, Vector3 offset, Color color, int plateNum) //Sets the current plate in hand
    {
        PlateEquipped.PlateObject = plateObject;
        PlateEquipped.PlateOffset = offset;
        PlateEquipped.PlateOrigin = platePosition.anchoredPosition;
        PlateEquipped.PlatePosition = platePosition;
        PlateEquipped.PlateColor = color;
        PlateHeld = true;

        PlateNum = plateNum;
    }


    //Private Functions
    bool CheckPlatePos() //Checks if Plate in hand is overlapping the bowl and correct color
    {
        PlateEquipped.PlateObject.transform.GetChild(0).gameObject.SetActive(true);
        if (rectOverlaps(PlateEquipped.PlatePosition, Bowl.rectTransform)) //Overlap Check
        {
            return CheckPlateColor(); //Color Check
        }
        else
        {
            return false;
        }
    }

    bool CheckPlateColor()
    {
        if (GetBaseColor(PlateEquipped.PlateColor) != GetBaseColor(PlateColorOrder[PlatesEmptied])) //Color Check
        {
            ProgressBar.value -= .1f;
            return false;
        }
        else
        {
            return true;
        }
    }

    void ResetPlate(bool added) //Resets the position of the plate and checks if ingredient was added
    {
        //Sets plate position back to origin
        Vector3 currPosition = PlateEquipped.PlatePosition.anchoredPosition;
        currPosition.x += (PlateEquipped.PlateOrigin.x - currPosition.x);
        currPosition.y += (PlateEquipped.PlateOrigin.y - currPosition.y);
        PlateEquipped.PlatePosition.anchoredPosition = currPosition;

        //Sets Plate to not being held
        //PlateHeld = false;

        if (added) //Checks if plate was over bowl
        {
            //Removes plate and adds to progress
            PlateEquipped.PlateObject.SetActive(false);
            PlatesEmptied++;

            //If task not complete sets instructions to next plates values
            if (PlatesEmptied < PlateColorOrder.Count)
            {
                Bowl.color = PlateColorOrder[PlatesEmptied];
                PlateObjects[PlateNum].gameObject.GetComponent<AddingPlateInfo>().SetPlateHeldInParent();
                SetColorIndicator(Bowl.color);
            }
            else
            {
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

    Vector3 GetBaseColor(Color rgba)
    {
        //Get Color
        Vector3 baseColor = new Vector3(rgba.r, rgba.g, rgba.b);

        //Return Color
        return baseColor;
    }

    void SetColorIndicator(Color nextColor)
    {
        //Get Color from color dictionary
        string colorName = ColorValues[GetBaseColor(nextColor)];

        //Set Next Color to Add
        Instructions.text = "Add " + colorName + " to the Bowl";
    }

    void SortColors()
    {
        for (int i = 0; i < ColorValues.Count; i++) 
        {
            //gets random plate color from all plates values
            System.Random rand = new System.Random();
            Vector3 selectedColor = ColorValues.ElementAt(rand.Next(0, ColorValues.Count)).Key;
            //Sets color back to readable color using Color struct
            Color fullColor = new Vector4(selectedColor.x, selectedColor.y, selectedColor.z, 1);
            //Checks if plate color was already added to objective list
            if (!PlateColorOrder.Contains(fullColor))
            {
                //Adds plate to objective list
                PlateColorOrder.Add(fullColor);
            }
            else
            {
                //Sets for loop back to get a different color
                i--;
            }
        }
    }

    void SetIngredientDictionary()
    {
        foreach(ColorValue plates in Plates)
        {
            //Adds plate colors to Color Dictionary
            ColorValues.Add(GetBaseColor(plates.PlateColor), plates.Name);
        }
    }
}

