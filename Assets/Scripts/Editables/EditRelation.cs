using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class EditRelation : EditObject
{

    Relation relationReference;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        editPanel = toolbox.transform.GetChild(0).GetChild(3);
        interactable.selectEntered.AddListener(OpenDrawer);
        relationReference = transform.parent.GetComponentInParent<Identity>().relationReference;
    }

    protected override void OpenDrawer(SelectEnterEventArgs args)
    {
        base.OpenDrawer(args);

        InputField nameField = editPanel.GetChild(0).GetChild(1).GetComponent<InputField>();
        nameField.onEndEdit.RemoveAllListeners();
        nameField.placeholder.GetComponent<Text>().text = relationReference.name;
        nameField.text = relationReference.name;
        nameField.onEndEdit.AddListener(delegate (string name) { relationReference.setName(name); });

        SetUpPositions();
        SetUpBounds();

    }

    private void SetUpPositions()
    {
        Dropdown sourceField = editPanel.GetChild(1).GetChild(1).GetComponent<Dropdown>();
        sourceField.onValueChanged.RemoveAllListeners();
        Dropdown destField = editPanel.GetChild(2).GetChild(1).GetComponent<Dropdown>();
        destField.onValueChanged.RemoveAllListeners();

        if (sourceField.options.Count != Iml.getSingleton().structuralModel.classes.Count)
        {
            List<string> options = new List<string>();
            foreach (UserClass classRef in Iml.getSingleton().structuralModel.classes)
            {
                options.Add(classRef.name);
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
    }

    private void SetUpBounds()
    {
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

    private void SaveLower(string s)
    {
        string message;
        if(ValidateBounds(s, relationReference.upperBound, out message))
        {
            relationReference.updateBounds(s, null);
        } else
        {
            if (message.Contains("greater"))
            {
                relationReference.updateBounds(s, s);
            }
            else
            {
                editPanel.GetChild(3).GetChild(1).GetComponent<InputField>().text = relationReference.lowerBound;
                PrintError(message);
            }
        }
    }

    private void SaveUpper(string s)
    {
        string message;
        if (ValidateBounds(relationReference.lowerBound, s, out message))
        {
            relationReference.updateBounds(null, s);
        }
        else
        {
                editPanel.GetChild(4).GetChild(1).GetComponent<InputField>().text = relationReference.upperBound;
                PrintError(message);
        }
    }

    private void SaveSource(int i)
    {
        relationReference.sourceClass.relations.Remove(relationReference);
        UserClass classRef = Iml.getSingleton().structuralModel.classes[i];
        relationReference.sourceClass = classRef;
        relationReference.source = classRef.id;
        relationReference.attachToClass(classRef, relationReference.destinationClass);
    }

    private void SaveDestination(int i)
    {
        relationReference.destinationClass.relations.Remove(relationReference);
        UserClass classRef = Iml.getSingleton().structuralModel.classes[i];
        relationReference.destinationClass = classRef;
        relationReference.destination = classRef.id;
        relationReference.attachToClass(relationReference.sourceClass, classRef);
    }
}
