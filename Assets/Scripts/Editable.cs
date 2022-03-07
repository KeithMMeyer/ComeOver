using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class Editable : MonoBehaviour
{

    private XRSimpleInteractable interactable;
    private Transform editPanel;


    // Start is called before the first frame update
    void Start()
    {
        editPanel = GameObject.Find("ToolBox").transform.GetChild(0).GetChild(1);

        interactable = GetComponent<XRSimpleInteractable>();
        interactable.onSelectEntered.AddListener(OpenDrawer);
        //interactable.onActivate.AddListener(OpenDrawer);


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenDrawer(XRBaseInteractor i)
    {
        editPanel.gameObject.SetActive(true);

        UserClass classReference = transform.GetComponentInParent<Identity>().classReference;

        InputField inputField = editPanel.GetChild(0).GetChild(1).GetComponent<InputField>();
        inputField.placeholder.GetComponent<Text>().text = classReference.name;
        inputField.text = classReference.name;

        Dropdown dropDown = editPanel.GetChild(1).GetChild(1).GetComponent<Dropdown>();
        dropDown.value = classReference.isAbstract.Equals("TRUE") ? 1 : 0;
        dropDown.onValueChanged.AddListener(SaveChanges);
    }

    public void SaveChanges(int i)
    {
        Dropdown dropDown = editPanel.GetChild(1).GetChild(1).GetComponent<Dropdown>();
        UserClass classReference = transform.GetComponentInParent<Identity>().classReference;
        classReference.setAbstract(dropDown.value == 1);
    }
}
