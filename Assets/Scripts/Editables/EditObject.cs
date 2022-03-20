using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class EditObject : MonoBehaviour
{

    protected XRSimpleInteractable interactable;
    protected ToolBox toolbox;
    protected Transform editPanel;

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
            if (current.Equals("0") && !upDirection)
            {
                field.text = "*";
            }
            else
            {
                int number = int.Parse(field.text);
                number = upDirection ? number + 1 : number - 1;
                field.text = number.ToString();
            }
        }
        field.onEndEdit.Invoke(field.text);
    }

}
