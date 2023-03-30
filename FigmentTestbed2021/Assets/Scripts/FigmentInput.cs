﻿using UnityEngine;
using UnityEngine.UI;

public class FigmentInput : MonoBehaviour {

    public enum FigmentButton
    {
        LeftButton,
        RightButton,
        ActionButton
    }

    public delegate void ButtonEvent(FigmentButton buttonType);

    public static event ButtonEvent OnButtonDown;
    public static event ButtonEvent OnButtonHold;
    public static event ButtonEvent OnButtonUp;

    static bool[] FigmentButtonPressed;
    static bool[] FigmentButtonPressedLastFrame;


    public Button[] buttons;


    // Use this for initialization
    void Start () 
    {
        FigmentButtonPressed = new bool[System.Enum.GetValues(typeof(FigmentButton)).Length];
        FigmentButtonPressedLastFrame = new bool[System.Enum.GetValues(typeof(FigmentButton)).Length];
    }

    private KeyCode[] leftKeys = new KeyCode[] { KeyCode.LeftArrow, KeyCode.A };
    private KeyCode[] rightKeys = new KeyCode[] { KeyCode.RightArrow, KeyCode.D };
    private KeyCode[] actionKeys = new KeyCode[] { KeyCode.Space, KeyCode.Return, KeyCode.E };

    // Update is called once per frame
    void Update () 
    {
        // Only update if the mouse is not being used
        if (!Input.GetMouseButton(0))
        {
            LaunchButtonEventsFromKeyboard(FigmentButton.LeftButton, leftKeys );
            LaunchButtonEventsFromKeyboard(FigmentButton.RightButton, rightKeys);
            LaunchButtonEventsFromKeyboard(FigmentButton.ActionButton, actionKeys);

            UpdateButtonStateFromKeyboard(FigmentButton.LeftButton, leftKeys);
            UpdateButtonStateFromKeyboard(FigmentButton.RightButton, rightKeys);
            UpdateButtonStateFromKeyboard(FigmentButton.ActionButton, actionKeys);
        }
    }

    public void PressButton(int buttonType)
    {
        // Unfortunately have to use an int due to Unity's UI rules
        FigmentButtonPressed[buttonType] = true;

        if (FigmentButtonPressedLastFrame[(int)buttonType] != true)
        {
            if (OnButtonDown != null)
            {
                OnButtonDown((FigmentButton)buttonType);
            }
        }

        if (OnButtonHold != null)
        {
            OnButtonHold((FigmentButton)buttonType);
        }
    }

    void UpdateButtonStateFromKeyboard(FigmentButton buttonType, KeyCode[] keyboardKeys)
    {
        FigmentButtonPressedLastFrame[(int)buttonType] = FigmentButtonPressed[(int)buttonType];

        foreach(KeyCode code in keyboardKeys)
        {
            if(Input.GetKey(code))
            {
                FigmentButtonPressed[(int)buttonType] = true;
                return;
            }
        }
        FigmentButtonPressed[(int)buttonType] = false;
    }

    void LaunchButtonEventsFromKeyboard(FigmentButton buttonType, KeyCode[] keyboardKeys)
    {
        foreach (KeyCode keyboardKey in keyboardKeys)
        {
            if (Input.GetKeyDown(keyboardKey))
            {
                if (OnButtonDown != null)
                {
                    OnButtonDown(buttonType);
                }

                Button button = buttons[(int)buttonType];
                button.image.color = button.colors.pressedColor;
            }

            if (Input.GetKey(keyboardKey))
            {
                if (OnButtonHold != null)
                {
                    OnButtonHold(buttonType);
                }

                Button button = buttons[(int)buttonType];
                button.image.color = button.colors.pressedColor;
            }

            if (Input.GetKeyUp(keyboardKey))
            {
                if (OnButtonUp != null)
                {
                    OnButtonUp(buttonType);
                }

                Button button = buttons[(int)buttonType];
                button.image.color = button.colors.normalColor;
            }
        }
    }

    public static bool GetButton(FigmentButton buttonType)
    {
        return FigmentButtonPressed[(int)buttonType];
    }

    public static bool GetButtonDown(FigmentButton buttonType)
    {
        return FigmentButtonPressed[(int)buttonType] && !FigmentButtonPressedLastFrame[(int)buttonType];
    }

    public static bool GetButtonUp(FigmentButton buttonType)
    {
        return !FigmentButtonPressed[(int)buttonType] && FigmentButtonPressedLastFrame[(int)buttonType];
    }
}