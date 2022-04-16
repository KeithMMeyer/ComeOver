using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour
{
    private TouchScreenKeyboard overlayKeyboard;
    private InputField field;
    private bool waiting = true;


    // Start is called before the first frame update
    void Start()
    {
        field = GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        if (field.isFocused && waiting) {
            overlayKeyboard = TouchScreenKeyboard.Open(field.text, TouchScreenKeyboardType.Default);
            waiting = false;
        }
        if (overlayKeyboard != null && overlayKeyboard.active)
        {
            field.text = overlayKeyboard.text;
        } else
        {
            if (waiting == false)
            {
                field.DeactivateInputField();
                waiting = true;
            }
        }
    }
}
