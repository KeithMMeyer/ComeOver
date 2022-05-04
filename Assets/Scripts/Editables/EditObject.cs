using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class EditObject : MonoBehaviour
{

    protected XRSimpleInteractable interactable;
    protected ToolBox toolbox;
    protected Transform editPanel;
    private Transform errorPanel;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        toolbox = GameObject.Find("ToolBox").GetComponent<ToolBox>();
        interactable = GetComponent<XRSimpleInteractable>();
    }

    protected virtual void OpenDrawer(SelectEnterEventArgs args)
    {
        toolbox.closeAll();
        editPanel.gameObject.SetActive(true);
        errorPanel = GameObject.Find("Main Canvas").transform.GetChild(1);
    }

    protected virtual bool ValidateName(string candidateName)
    {
        if (candidateName == null || candidateName.Equals("") || !ValidIdentifier(candidateName))
        {
            //should say type of thingy
            string text = "\"" + candidateName + "\" is not a valid " + "" + " name; valid identifiers contain only: letters, digits, underscores, or dollar signs, and must not begin with a digit. Keywords must also not be used.";
            PrintError(text);
            return false;
        }
        return true;
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


    private bool ValidIdentifier(string candidateName)
    {
        Regex regex = new Regex("^([a-zA-Z_$][a-zA-Z\\d_$]*)$");
        bool syntax = regex.IsMatch(candidateName);

        for (int i = 0; i < JAVA_KEYWORDS.Length; i++)
        {
            if (candidateName.ToLower().Equals(JAVA_KEYWORDS[i]))
                return false;
        }

        return syntax;
    }

    protected bool ValidateBounds(string lower, string upper, out string message)
    {
        int lowerNum;
        int upperNum;
        message = "The newly entered upper bound (" + upper + ") creates a violation of valid bounds(typically caused by the lower bound being greater than the upper bound).";
        if (int.TryParse(lower, out lowerNum) && lowerNum >= 0)
        {
            if (upper.Equals("*"))
                return true;
            if (int.TryParse(upper, out upperNum) && upperNum > 0)
            {
                return upperNum >= lowerNum;
            }
            message = "The value " + upper + " is not a valid upper bound. Bounds must be positive integers, or * to indicated unbounded.";
            return false;
        }
        message = "The value " + lower + " is not a valid upper bound. Bounds must be positive integers, or * to indicated unbounded.";
        return false;
    }

    protected void BumpField(InputField field, bool upDirection)
    {
        string current = field.text;
        if (current.Equals("*"))
        {
            if (upDirection)
                field.text = "0";
        }
        else
        {
            int number = int.Parse(field.text);
            number = upDirection ? number + 1 : number - 1;
            field.text = number.ToString();
        }
        field.onEndEdit.Invoke(field.text);
    }

    protected void PrintError(string message)
    {
        errorPanel.gameObject.SetActive(true);
        errorPanel.GetChild(1).GetComponent<Text>().text = message;
    }

}
