using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshCollider))]

public class EditText : MonoBehaviour
{
    public int field;
    [SerializeField] InputField inputField;


    private void Start()
    {
        inputField = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<InputField>();
    }

    
    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            editState();
            //inputField.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        }
    }

    public void editState()
    {
        string text = transform.parent.GetChild(1).GetComponent<TextMesh>().text;
        inputField.gameObject.SetActive(true);
        inputField.placeholder.GetComponent<Text>().text = text;
        inputField.text = transform.parent.GetChild(1).GetComponent<TextMesh>().text;
        inputField.GetComponent<SaveText>().targetText = transform.parent.GetChild(1).GetComponent<TextMesh>();
        inputField.GetComponent<SaveText>().targetScript = this;
        inputField.GetComponent<SaveText>().startKeyboard(text);
    }

    public void saveState(string text)
    {
        UserAttribute attr = transform.parent.parent.GetComponent<Identity>().attributeReference;
        switch (field)
        {
            case (0):
                attr.name = text;
                break;
            case (1):
                attr.type = text;
                break;
            case (2):
                attr.value = text;
                break;
            case (3):
                attr.visibility = text;
                break;
            case (4):
                attr.upperBound = text;
                break;
            case (5):
                attr.lowerBound = text;
                break;
            default:
                Debug.LogError("Saving to invalid field " + field + "!");
                break;
        }


    }

    public string validateText(string candidateName)
    {
        if (field > 0)
            return "valid";
        if (candidateName == null || candidateName.Equals("") || !validIdentifier(candidateName))
        {
            return "\"" + candidateName + "\" is not a valid Attribute name; valid identifiers contain only: letters, digits, underscores, or dollar signs, and must not begin with a digit. Keywords must also not be used.";
        }
        return "valid";
    }

    //validation

    string[] JAVA_KEYWORDS = {"abstract", "assert", "boolean", "break", "byte", "case",
                            "catch", "char", "class", "const", "continue", "default",
                            "double", "do", "else", "enum", "extends", "false",
                            "final", "finally", "float", "for", "goto", "if",
                            "implements", "import", "instanceof", "int", "interface", "long",
                            "native", "new", "null", "package", "private", "protected",
                            "public", "return", "short", "static", "strictfp", "super",
                            "switch", "synchronized", "this", "throw", "throws", "transient",
                            "true", "try", "void", "volatile", "while" };


    private bool validIdentifier(string candidateName)
    {
        Regex regex = new Regex("^([a - zA - Z_$][a - zA - Z\\d_$] *)$");
        bool syntax = regex.IsMatch(candidateName);

        for (int i = 0; i < JAVA_KEYWORDS.Length; i++)
        {
            if (candidateName.ToLower().Equals(JAVA_KEYWORDS[i]))
                return false;
        }

        return syntax;
    }


}
