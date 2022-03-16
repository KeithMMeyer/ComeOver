using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class Editable : MonoBehaviour
{

    private XRSimpleInteractable interactable;
    private ToolBox toolbox;
    private Transform editPanel;
    private UserClass classReference;

    // Start is called before the first frame update
    void Start()
    {
        toolbox = GameObject.Find("ToolBox").GetComponent<ToolBox>();
        editPanel = toolbox.transform.GetChild(0).GetChild(1);

        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OpenDrawer);
        //interactable.onActivate.AddListener(OpenDrawer);

        classReference = transform.GetComponentInParent<Identity>().classReference;

    }

    public void OpenDrawer(SelectEnterEventArgs args)
    {
        toolbox.closeAll();
        editPanel.gameObject.SetActive(true);

        InputField nameField = editPanel.GetChild(0).GetChild(1).GetComponent<InputField>();
        nameField.onEndEdit.RemoveAllListeners();
        nameField.placeholder.GetComponent<Text>().text = classReference.name;
        nameField.text = classReference.name;
        nameField.onEndEdit.AddListener(SaveInputField);

        Dropdown abstractField = editPanel.GetChild(1).GetChild(1).GetComponent<Dropdown>();
        abstractField.onValueChanged.RemoveAllListeners();
        abstractField.value = classReference.isAbstract.Equals("TRUE") ? 1 : 0;
        abstractField.onValueChanged.AddListener(SaveDropdown);
    }


    public void SaveInputField(string s)
    {
        classReference.setName(s);
    }

    public void SaveDropdown(int i)
    {
        classReference.setAbstract(i == 1);
    }
}
