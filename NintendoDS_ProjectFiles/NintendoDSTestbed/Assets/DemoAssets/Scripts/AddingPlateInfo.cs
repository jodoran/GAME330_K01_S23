using UnityEngine;
using UnityEngine.UI;

//Class calls AddingTask function to SetPlateHeld on interaction down with its set object
public class AddingPlateInfo : MonoBehaviour, IDSTapListener
{
    public Color Color;
    public Vector3 Offset;

    private GameObject BaseObject;
    private RectTransform Plate;

    private void Start() //Sets Plate to the attached Objects
    {
        BaseObject = this.transform.parent.gameObject;
        Plate = this.GetComponent<RectTransform>();
    }

    public void OnScreenTapDown(Vector2 tapPosition) // On Pressed Sets the Plate to being Held
    {
        if (DSTapRouter.RectangleContainsDSPoint(GetComponent<RectTransform>(), tapPosition))
        {
            BaseObject.GetComponent<AddingTask>().SetPlateHeld(gameObject, Plate, Offset, Color);
        }
    }

    public void OnScreenDrag(Vector2 tapPosition){ }
    public void OnScreenTapUp(Vector2 tapPosition){ }
}
