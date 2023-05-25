using UnityEngine;
using System;
using System.Collections.Generic;

//Class calls AddingTask function to SetPlateHeld on interaction down with its set object
public class AddingPlateInfo : MonoBehaviour, IDSTapListener
{
    public Color Color;
    public Vector3 Offset;

    public int PlateNum;

    private GameObject BaseObject;
    private RectTransform Plate;

    private void Start() //Sets Plate to the attached Objects
    {
        BaseObject = this.transform.parent.gameObject;
        Plate = this.GetComponent<RectTransform>();

        BaseObject.GetComponent<AddingTask>().SetPlateObjects(PlateNum, gameObject);
    }

    public void OnScreenTapDown(Vector2 tapPosition) // On Pressed Sets the Plate to being Held
    {
        if (DSTapRouter.RectangleContainsDSPoint(GetComponent<RectTransform>(), tapPosition))
        {
            SetPlateHeldInParent();
        }
    }

    public void OnScreenDrag(Vector2 tapPosition){ } //Not Used
    public void OnScreenTapUp(Vector2 tapPosition){ } //Not Used
 
    public void SetPlateHeldInParent()
    {
        BaseObject.GetComponent<AddingTask>().ChangeActivePlateColor();
        BaseObject.GetComponent<AddingTask>().SetPlateHeld(gameObject, Plate, Offset, Color, PlateNum);
        this.transform.GetChild(0).gameObject.SetActive(false);
    }
}
