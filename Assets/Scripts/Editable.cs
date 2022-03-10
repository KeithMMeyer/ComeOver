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


    // Start is called before the first frame update
    void Start()
    {
        toolbox = GameObject.Find("ToolBox").GetComponent<ToolBox>();
        editPanel = toolbox.transform.GetChild(0).GetChild(1);

        interactable = GetComponent<XRSimpleInteractable>();
        interactable.onSelectEntered.AddListener(OpenDrawer);
        //interactable.onActivate.AddListener(OpenDrawer);


    }

    public void OpenDrawer(XRBaseInteractor i)
    {
        toolbox.closeAll();
        editPanel.gameObject.SetActive(true);

        UserClass classReference = transform.GetComponentInParent<Identity>().classReference;

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
        UserClass classReference = transform.GetComponentInParent<Identity>().classReference;
        classReference.setName(s);
    }

    public void SaveDropdown(int i)
    {
        UserClass classReference = transform.GetComponentInParent<Identity>().classReference;
        classReference.setAbstract(i == 1);
    }
}
