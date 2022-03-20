using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class EditRelation : MonoBehaviour
{

    private XRSimpleInteractable interactable;
    private ToolBox toolbox;
    private Transform editPanel;
    Relation relationReference;

    // Start is called before the first frame update
    void Start()
    {
        toolbox = GameObject.Find("ToolBox").GetComponent<ToolBox>();
        editPanel = toolbox.transform.GetChild(0).GetChild(3);

        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OpenDrawer);
        //interactable.onActivate.AddListener(OpenDrawer);

        relationReference = transform.parent.GetComponentInParent<Identity>().relationReference;

    }

    public void OpenDrawer(SelectEnterEventArgs args)
    {
        toolbox.closeAll();
        editPanel.gameObject.SetActive(true);

        InputField nameField = editPanel.GetChild(0).GetChild(1).GetComponent<InputField>();
        nameField.onEndEdit.RemoveAllListeners();
        nameField.placeholder.GetComponent<Text>().text = relationReference.name;
        nameField.text = relationReference.name;
        nameField.onEndEdit.AddListener(SaveName);


        Dropdown sourceField = editPanel.GetChild(1).GetChild(1).GetComponent<Dropdown>();
        sourceField.onValueChanged.RemoveAllListeners();
        Dropdown destField = editPanel.GetChild(2).GetChild(1).GetComponent<Dropdown>();
        destField.onValueChanged.RemoveAllListeners();

        if (sourceField.options.Count != Iml.getSingleton().structuralModel.classes.Count)
        {
            List<string> options = new List<string>();
            foreach (UserClass classRef in Iml.getSingleton().structuralModel.classes)
            {
                options.Add(classRef.name); // Or whatever you want for a label
            }
            sourceField.ClearOptions();
            sourceField.AddOptions(options);
            destField.ClearOptions();
            destField.AddOptions(options);
        }

        sourceField.value = Iml.getSingleton().structuralModel.classes.IndexOf(relationReference.sourceClass);
        sourceField.onValueChanged.AddListener(SaveSource);
        destField.value = Iml.getSingleton().structuralModel.classes.IndexOf(relationReference.destinationClass);
        destField.onValueChanged.AddListener(SaveDestination);


        InputField lowerBound = editPanel.GetChild(3).GetChild(1).GetComponent<InputField>();
        lowerBound.onEndEdit.RemoveAllListeners();
        lowerBound.placeholder.GetComponent<Text>().text = relationReference.lowerBound;
        lowerBound.text = relationReference.lowerBound;
        lowerBound.onEndEdit.AddListener(SaveLower);
        editPanel.GetChild(3).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
        editPanel.GetChild(3).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { BumpField(lowerBound, true); });
        editPanel.GetChild(3).GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
        editPanel.GetChild(3).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { BumpField(lowerBound, false); });

        InputField upperBound = editPanel.GetChild(4).GetChild(1).GetComponent<InputField>();
        upperBound.onEndEdit.RemoveAllListeners();
        upperBound.placeholder.GetComponent<Text>().text = relationReference.upperBound;
        upperBound.text = relationReference.upperBound;
        upperBound.onEndEdit.AddListener(SaveUpper);
        editPanel.GetChild(4).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
        editPanel.GetChild(4).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { BumpField(upperBound, true); });
        editPanel.GetChild(4).GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
        editPanel.GetChild(4).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { BumpField(upperBound, false); });

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
        relationReference.setName(s);
    }

    public void SaveLower(string s)
    {
        relationReference.updateBounds(s, null);
    }

    public void SaveUpper(string s)
    {
        relationReference.updateBounds(null, s);
    }

    public void SaveSource(int i)
    {
        relationReference.sourceClass.relations.Remove(relationReference);
        UserClass classRef = Iml.getSingleton().structuralModel.classes[i];
        relationReference.sourceClass = classRef;
        relationReference.source = classRef.id;
        relationReference.attachToClass(classRef, relationReference.destinationClass);
        //GameObject.Destroy(transform.parent.gameObject);
    }

    public void SaveDestination(int i)
    {
        relationReference.destinationClass.relations.Remove(relationReference);
        UserClass classRef = Iml.getSingleton().structuralModel.classes[i];
        relationReference.destinationClass = classRef;
        relationReference.destination = classRef.id;
        relationReference.attachToClass(relationReference.sourceClass, classRef);
        //GameObject.Destroy(transform.parent.gameObject);
    }
}
