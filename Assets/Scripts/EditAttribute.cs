using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class EditAttribute : MonoBehaviour
{

    private XRSimpleInteractable interactable;
    private ToolBox toolbox;
    private Transform editPanel;
    UserAttribute attributeReference;

    // Start is called before the first frame update
    void Start()
    {
        toolbox = GameObject.Find("ToolBox").GetComponent<ToolBox>();
        editPanel = toolbox.transform.GetChild(0).GetChild(2);

        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OpenDrawer);
        //interactable.onActivate.AddListener(OpenDrawer);

        attributeReference = transform.GetComponentInParent<Identity>().attributeReference;

    }

    public void OpenDrawer(SelectEnterEventArgs args)
    {
        toolbox.closeAll();
        editPanel.gameObject.SetActive(true);

        Dropdown visibilityField = editPanel.GetChild(0).GetChild(1).GetComponent<Dropdown>();
        visibilityField.onValueChanged.RemoveAllListeners();
        string[] visibilityArray = { "PUBLIC", "PRIVATE", "PROTECTED" };
        List<string> visibilities = visibilityArray.ToList();
        visibilityField.value = visibilities.IndexOf(attributeReference.visibility);
        visibilityField.onValueChanged.AddListener(SaveVisibility);

        InputField nameField = editPanel.GetChild(1).GetChild(1).GetComponent<InputField>();
        nameField.onEndEdit.RemoveAllListeners();
        nameField.placeholder.GetComponent<Text>().text = attributeReference.name;
        nameField.text = attributeReference.name;
        nameField.onEndEdit.AddListener(SaveName);

        Dropdown typeField = editPanel.GetChild(2).GetChild(1).GetComponent<Dropdown>();
        typeField.onValueChanged.RemoveAllListeners();
        string[] typeArray = { "STRING", "BOOLEAN", "DOUBLE", "INTEGER" };
        List<string> types = typeArray.ToList();
        typeField.value = types.IndexOf(attributeReference.type);
        typeField.onValueChanged.AddListener(SaveType);

        InputField lowerBound = editPanel.GetChild(3).GetChild(1).GetComponent<InputField>();
        lowerBound.onEndEdit.RemoveAllListeners();
        lowerBound.placeholder.GetComponent<Text>().text = attributeReference.lowerBound;
        lowerBound.text = attributeReference.lowerBound;
        lowerBound.onEndEdit.AddListener(SaveLower);
        editPanel.GetChild(3).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
        editPanel.GetChild(3).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { BumpField(lowerBound, true); });
        editPanel.GetChild(3).GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
        editPanel.GetChild(3).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { BumpField(lowerBound, false); });

        InputField upperBound = editPanel.GetChild(4).GetChild(1).GetComponent<InputField>();
        upperBound.onEndEdit.RemoveAllListeners();
        upperBound.placeholder.GetComponent<Text>().text = attributeReference.upperBound;
        upperBound.text = attributeReference.upperBound;
        upperBound.onEndEdit.AddListener(SaveUpper);
        editPanel.GetChild(4).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
        editPanel.GetChild(4).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { BumpField(upperBound, true); });
        editPanel.GetChild(4).GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
        editPanel.GetChild(4).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { BumpField(upperBound, false); });

        InputField valueField = editPanel.GetChild(5).GetChild(1).GetComponent<InputField>();
        valueField.onEndEdit.RemoveAllListeners();
        valueField.placeholder.GetComponent<Text>().text = attributeReference.value;
        valueField.text = attributeReference.value;
        valueField.onEndEdit.AddListener(SaveValue);
    }

    public void BumpField(InputField field, bool upDirection)
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

    public void SaveName(string s)
    {
        attributeReference.setName(s);
        attributeReference.generateDisplayString();
    }

    public void SaveUpper(string s)
    {
        attributeReference.upperBound = s;
        attributeReference.generateDisplayString();
    }

    public void SaveLower(string s)
    {
        attributeReference.lowerBound = s;
        attributeReference.generateDisplayString();
    }

    public void SaveValue(string s)
    {
        attributeReference.setValue(s);
        attributeReference.generateDisplayString();
    }

    public void SaveVisibility(int i)
    {
        string[] visibilityArray = { "PUBLIC", "PRIVATE", "PROTECTED" };
        attributeReference.visibility = visibilityArray[i];
        attributeReference.generateDisplayString();
    }

    public void SaveType(int i)
    {
        string[] strings = { "STRING", "BOOLEAN", "DOUBLE", "INTEGER" };
        attributeReference.type = strings[i];
        attributeReference.generateDisplayString();
    }
}
