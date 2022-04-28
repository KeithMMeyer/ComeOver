using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class EditAttribute : EditObject
{

    UserAttribute attributeReference;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        editPanel = toolbox.transform.GetChild(0).GetChild(2);
        interactable.selectEntered.AddListener(OpenDrawer);
        attributeReference = transform.GetComponentInParent<Identity>().attributeReference;
    }

    protected override void OpenDrawer(SelectEnterEventArgs args)
    {
        base.OpenDrawer(args);

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
        nameField.onEndEdit.AddListener(delegate (string name) { attributeReference.SetName(name); });

        Dropdown typeField = editPanel.GetChild(2).GetChild(1).GetComponent<Dropdown>();
        typeField.onValueChanged.RemoveAllListeners();
        typeField.value = attributeReference.GetTypeNumber();
        typeField.onValueChanged.AddListener(SaveType);

        SetUpBounds();

        InputField valueField = editPanel.GetChild(5).GetChild(1).GetComponent<InputField>();
        valueField.onEndEdit.RemoveAllListeners();
        valueField.placeholder.GetComponent<Text>().text = attributeReference.value;
        valueField.text = attributeReference.value;
        valueField.onEndEdit.AddListener(SaveValue);
    }

    private void SetUpBounds()
    {
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
    }

    private void SaveLower(string s)
    {
        string message;
        if (ValidateBounds(s, attributeReference.upperBound, out message))
        {
            attributeReference.UpdateBounds(s, null);
        }
        else
        {
            if (message.Contains("greater"))
            {
                attributeReference.UpdateBounds(s, s);
                editPanel.GetChild(4).GetChild(1).GetComponent<InputField>().text = s;
            }
            else
            {
                editPanel.GetChild(3).GetChild(1).GetComponent<InputField>().text = attributeReference.lowerBound;
                PrintError(message);
            }
        }
        editPanel.GetChild(5).GetChild(1).GetComponent<InputField>().text = attributeReference.value;
    }

    private void SaveUpper(string s)
    {
        string message;
        if (ValidateBounds(attributeReference.lowerBound, s, out message))
        {
            attributeReference.UpdateBounds(null, s);
            editPanel.GetChild(5).GetChild(1).GetComponent<InputField>().text = attributeReference.value;
        }
        else
        {
            editPanel.GetChild(4).GetChild(1).GetComponent<InputField>().text = attributeReference.upperBound;
            PrintError(message);
        }
    }

    private void SaveVisibility(int i)
    {
        string[] visibilityArray = { "PUBLIC", "PRIVATE", "PROTECTED" };
        attributeReference.visibility = visibilityArray[i];
        attributeReference.GenerateDisplayString();
    }

    private void SaveType(int i)
    {
        attributeReference.SetType(i);
        editPanel.GetChild(5).GetChild(1).GetComponent<InputField>().text = attributeReference.value;
    }

    private void SaveValue(string value)
    {
        string message = attributeReference.SetValue(value);
        if (message != null)
        {
            PrintError(message);
            editPanel.GetChild(5).GetChild(1).GetComponent<InputField>().text = attributeReference.value;
        }
    }
}
