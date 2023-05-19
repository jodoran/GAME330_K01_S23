using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class PlateInfo
{
    public GameObject PlateObject;
    public RectTransform PlatePosition;
    public Vector3 PlateOrigin;
    public Color PlateColor;
    public Vector3 PlateOffset;
}

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
        
    //Public Variables
    public Image Bowl;
    public Text Instructions;
    public Slider ProgressBar;

    public List<ColorValue> Plates = new List<ColorValue>();

    //Variables
    List<Color> PlateColorOrder = new List<Color>();
    PlateInfo PlateEquipped = new PlateInfo();
    bool PlateHeld;
    int PlatesEmptied;

    //Base Function
    private void Start()
    {
        SetIngredientDictionary();
        SortColors();
        Bowl.color = PlateColorOrder[PlatesEmptied];
        SetColorIndicator(Bowl.color);
    }

    //IDSTapListener Functions
    public void OnScreenTapDown(Vector2 tapPosition) { }

    public void OnScreenDrag(Vector2 tapPosition)
    {
        if (PlateHeld)
        {
            MovePlateWithCursor(PlateEquipped.PlatePosition, tapPosition, PlateEquipped.PlateOffset.x, PlateEquipped.PlateOffset.y);
        }
    }

    public void OnScreenTapUp(Vector2 tapPosition)
    {
        if (PlateHeld)
        {
            ResetPlate(CheckPlatePos());
        }
    }


    //Public Functions
    public void SetPlateHeld(GameObject plateObject, RectTransform platePosition, Vector3 offset, Color color) //Sets the current plate in hand
    {
        PlateEquipped.PlateObject = plateObject;
        PlateEquipped.PlateOffset = offset;
        PlateEquipped.PlateOrigin = platePosition.anchoredPosition;
        PlateEquipped.PlatePosition = platePosition;
        PlateEquipped.PlateColor = color;
        PlateHeld = true;
    }


    //Private Functions
    bool CheckPlatePos() //Checks if Plate in hand is overlapping the bowl and correct color
    {
        if (rectOverlaps(PlateEquipped.PlatePosition, Bowl.rectTransform)) //Overlap Check
        {
            if (GetBaseColor(PlateEquipped.PlateColor) == GetBaseColor(PlateColorOrder[PlatesEmptied])) //Color Check
            {
                ProgressBar.value += .02f;
                return true; 
            }
            else
            {
                ProgressBar.value -= .1f;
                return false; 
            }
        }
        else
        {
            return false;
        }
    }

    void ResetPlate(bool added) //Resets the position of the plate and checks if ingredient was added
    {
        Vector3 currPosition = PlateEquipped.PlatePosition.anchoredPosition;
        currPosition.x += (PlateEquipped.PlateOrigin.x - currPosition.x);
        currPosition.y += (PlateEquipped.PlateOrigin.y - currPosition.y);
        PlateEquipped.PlatePosition.anchoredPosition = currPosition;

        PlateHeld = false;

        if (added)
        {
            PlateEquipped.PlateObject.SetActive(false);
            PlatesEmptied++;

            if (PlatesEmptied < PlateColorOrder.Count)
            {
                Bowl.color = PlateColorOrder[PlatesEmptied];
                SetColorIndicator(Bowl.color);
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
            System.Random rand = new System.Random();
            Vector3 selectedColor = ColorValues.ElementAt(rand.Next(0, ColorValues.Count)).Key;
            Color fullColor = new Vector4(selectedColor.x, selectedColor.y, selectedColor.z, 1);
            if (!PlateColorOrder.Contains(fullColor))
            {
                PlateColorOrder.Add(fullColor);
            }
            else
            {
                i--;
            }
        }
    }

    void SetIngredientDictionary()
    {
        foreach(ColorValue plates in Plates)
        {
            ColorValues.Add(GetBaseColor(plates.PlateColor), plates.Name);
        }
    }
}

