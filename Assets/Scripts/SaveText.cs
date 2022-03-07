using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveText : MonoBehaviour
{
    public Transform errorPanel;

    public TextMesh targetText;
    public EditText targetScript;

    private TouchScreenKeyboard overlayKeyboard;
    public static string inputText = "";

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<InputField>().onEndEdit.AddListener(saveText);
        gameObject.SetActive(false);
        errorPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (overlayKeyboard != null)
            GetComponent<InputField>().placeholder.GetComponent<Text>().text = overlayKeyboard.text;
    }

    public void startKeyboard(string text)
    {
        overlayKeyboard = TouchScreenKeyboard.Open(text, TouchScreenKeyboardType.Default);
    }


    private void saveText(string textInField)
    {
        if (targetText != null && targetScript != null)
        {
            string valid = targetScript.validateText(textInField);

            if (valid.Equals("valid"))
            {
                targetText.text = textInField;
                targetScript.saveState(textInField);
            }
            else
            {
                errorPanel.gameObject.SetActive(true);
                errorPanel.GetChild(1).GetComponent<Text>().text = valid;
            }
        }
        gameObject.SetActive(false);

    }
}
