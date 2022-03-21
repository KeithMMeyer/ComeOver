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
            message = "The value " + upper + " is not a valid upper bound. Bounds must be positive integers, or * to indicated unbounded";
            return false;
        }
        message = "The value " + lower + " is not a valid upper bound. Bounds must be positive integers, or * to indicated unbounded";
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
