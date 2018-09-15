using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputType
{
    MOUSE_KEYBOARD, JOYSTICK
}
public class InputHandler : MonoBehaviour {

    public InputType GetInputType { get; private set; }

    private Vector2 lastMousePos;

    private static InputHandler _instance;

    public static InputHandler Instance()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start () {
        GetInputType = InputType.MOUSE_KEYBOARD;
        lastMousePos = Input.mousePosition;
    }
	 
	// Update is called once per frame
	void Update () {

        HandleInputType();
    }


    private void HandleInputType()
    {

        bool joystickUsed = GetJoystickUsed();
        if (joystickUsed)
        {
            Debug.Log("joystick used");
        }


        bool mouseAndKeyboardUsed = false;
        if (!joystickUsed && Input.anyKeyDown || lastMousePos != (Vector2)Input.mousePosition)
            mouseAndKeyboardUsed = true;
        lastMousePos = Input.mousePosition;

        if (mouseAndKeyboardUsed)
        {
            Debug.Log("MouseAndKeyboard used");
        }

        if (GetInputType == InputType.MOUSE_KEYBOARD && joystickUsed)
            GetInputType = InputType.JOYSTICK;

        if (GetInputType == InputType.JOYSTICK && mouseAndKeyboardUsed)
            GetInputType = InputType.MOUSE_KEYBOARD;

        Cursor.visible = GetInputType == InputType.MOUSE_KEYBOARD;
        //Debug.Log(GetInputType);
    }


    bool GetJoystickUsed()
    {
        bool joystickUsed = false;
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown("joystick 1 button " + i))
            {
                joystickUsed = true;
                break;
            }
        }


        if (Input.GetAxis("Left Stick X") != 0.0f ||
            Input.GetAxis("Left Stick Y") != 0.0f ||
            Input.GetAxis("Stick Triggers") != 0.0f ||
            Input.GetAxis("Right Stick X") != 0.0f ||
            Input.GetAxis("Right Stick Y") != 0.0f)
        {
            joystickUsed = true;
        }

        return joystickUsed;
    }
}
