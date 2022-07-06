using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class EditRelation : EditObject
{

    Relation relationReference;

    private List<string> options;

    // Start is called before the first frame update
    protected override void Start()
    {
        if (interactable == null)
            Init();
    }

    public void Init()
    {
        base.Start();
        editPanel = toolbox.transform.GetChild(0).GetChild(3);
        interactable.selectEntered.AddListener(OpenDrawer);
        relationReference = transform.parent.GetComponentInParent<Identity>().relationReference;
    }

    protected override void OpenDrawer(SelectEnterEventArgs args)
    {
        base.OpenDrawer(args);

        string name = null;
        string type;

        if (PhotonNetwork.IsMasterClient)
        {
            name = relationReference.name;
            type = relationReference.type;
        }
        else
        {
            type = transform.GetComponentInParent<RelationView>().type;
            type ??= "";
            if (type != "INHERITENCE")
            {
                name = transform.parent.parent.GetChild(2).GetComponent<TextMesh>().text;
            }
        }

        if (type.Equals("INHERITENCE"))
        {
            editPanel.GetChild(0).gameObject.SetActive(false);
            editPanel.GetChild(3).gameObject.SetActive(false);
            editPanel.GetChild(4).gameObject.SetActive(false);
        }
        else
        {
            InputField nameField = editPanel.GetChild(0).GetChild(1).GetComponent<InputField>();
            nameField.transform.parent.gameObject.SetActive(true);
            nameField.onEndEdit.RemoveAllListeners();
            nameField.placeholder.GetComponent<Text>().text = name;
            nameField.text = name;
            nameField.onEndEdit.AddListener(SaveName);
            SetUpBounds();
        }

        SetUpPositions();
    }

    public void SetUpPositions()
    {
        Dropdown sourceField = editPanel.GetChild(1).GetChild(1).GetComponent<Dropdown>();
        sourceField.onValueChanged.RemoveAllListeners();
        Dropdown destField = editPanel.GetChild(2).GetChild(1).GetComponent<Dropdown>();
        destField.onValueChanged.RemoveAllListeners();

        options = new List<string>();
        int sourceNum = 0;
        int destNum = 0;

        if (PhotonNetwork.IsMasterClient)
        {

            //if (sourceField.options.Count != Iml.GetSingleton().structuralModel.classes.Count)
            //{
            foreach (UserClass classRef in Iml.GetSingleton().structuralModel.classes)
            {
                options.Add(classRef.name);
            }

            //}

            sourceNum = Iml.GetSingleton().structuralModel.classes.IndexOf(relationReference.sourceClass);
            destNum = Iml.GetSingleton().structuralModel.classes.IndexOf(relationReference.destinationClass);

        } else
        {

            string sourceId = transform.GetComponentInParent<RelationView>().source;
            string destId = transform.GetComponentInParent<RelationView>().destination;
            ClassView[] classes = GameObject.FindObjectsOfType<ClassView>();

            for (int i = 0; i < classes.Length; i++)
            {
                if (sourceId.Equals(classes[i].id))
                    sourceNum = i;
                if (destId.Equals(classes[i].id))
                    destNum = i;
                options.Add(classes[i].transform.GetChild(1).GetComponent<TextMesh>().text);
            }
        }

        sourceField.ClearOptions();
        sourceField.AddOptions(options);
        destField.ClearOptions();
        destField.AddOptions(options);

        sourceField.value = sourceNum;
        sourceField.onValueChanged.AddListener(SaveSource);
        destField.value = destNum;
        destField.onValueChanged.AddListener(SaveDestination);
    }

    private void SetUpBounds()
    {
        string lower;
        string upper;
        if (PhotonNetwork.IsMasterClient)
        {
            lower = relationReference.lowerBound;
            upper = relationReference.upperBound;
        }
        else
        {
            string[] bounds = transform.parent.parent.GetChild(3).GetComponent<TextMesh>().text.Split(new string[] { ".." }, StringSplitOptions.None);
            lower = bounds[0].Substring(1);
            upper = bounds[1].Substring(0, (bounds[1].Length - 1));
        }

        InputField lowerBound = editPanel.GetChild(3).GetChild(1).GetComponent<InputField>();
        lowerBound.transform.parent.gameObject.SetActive(true);
        lowerBound.onEndEdit.RemoveAllListeners();
        lowerBound.placeholder.GetComponent<Text>().text = lower;
        lowerBound.text = lower;
        lowerBound.onEndEdit.AddListener(SaveLower);
        editPanel.GetChild(3).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
        editPanel.GetChild(3).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { BumpField(lowerBound, true); });
        editPanel.GetChild(3).GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
        editPanel.GetChild(3).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { BumpField(lowerBound, false); });

        InputField upperBound = editPanel.GetChild(4).GetChild(1).GetComponent<InputField>();
        upperBound.transform.parent.gameObject.SetActive(true);
        upperBound.onEndEdit.RemoveAllListeners();
        upperBound.placeholder.GetComponent<Text>().text = upper;
        upperBound.text = upper;
        upperBound.onEndEdit.AddListener(SaveUpper);
        editPanel.GetChild(4).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
        editPanel.GetChild(4).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { BumpField(upperBound, true); });
        editPanel.GetChild(4).GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
        editPanel.GetChild(4).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { BumpField(upperBound, false); });
        editPanel.GetChild(4).GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
        editPanel.GetChild(4).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { StarField(upperBound); });
    }

    public void SaveLower(string s)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("EditRelation", RpcTarget.MasterClient, "LOWER", s);
            return;
        }
        string message;
        if (ValidateBounds(s, relationReference.upperBound, out message))
        {
            relationReference.UpdateBounds(s, null);
        }
        else
        {
            if (message.Contains("greater"))
            {
                relationReference.UpdateBounds(s, s);
                editPanel.GetChild(4).GetChild(1).GetComponent<InputField>().text = s;
            }
            else
            {
                editPanel.GetChild(3).GetChild(1).GetComponent<InputField>().text = relationReference.lowerBound;
                PrintError(message);
            }
        }
    }

    public void SaveUpper(string s)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("EditRelation", RpcTarget.MasterClient, "UPPER", s);
            return;
        }
        string message;
        if (ValidateBounds(relationReference.lowerBound, s, out message))
        {
            relationReference.UpdateBounds(null, s);
        }
        else
        {
            editPanel.GetChild(4).GetChild(1).GetComponent<InputField>().text = relationReference.upperBound;
            PrintError(message);
        }
    }

    public void SaveSource(int i)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("EditRelation", RpcTarget.MasterClient, "SOURCE", options[i]);
            return;
        }
        relationReference.sourceClass.relations.Remove(relationReference);
        UserClass classRef = Iml.GetSingleton().structuralModel.classes[i];
        if (relationReference.CanAttach(classRef, relationReference.destinationClass, out string message))
        {
            relationReference.AttachToClass(classRef, relationReference.destinationClass);
        }
        else
        {
            editPanel.GetChild(1).GetChild(1).GetComponent<Dropdown>().value = Iml.GetSingleton().structuralModel.classes.IndexOf(relationReference.sourceClass);
            relationReference.sourceClass.relations.Add(relationReference);
            PrintError(message);
        }
    }

    public void SaveDestination(int i)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("EditRelation", RpcTarget.MasterClient, "DEST", options[i]);
            return;
        }
        relationReference.destinationClass.relations.Remove(relationReference);
        UserClass classRef = Iml.GetSingleton().structuralModel.classes[i];
        if (relationReference.CanAttach(relationReference.sourceClass, classRef, out string message))
        {
            relationReference.AttachToClass(relationReference.sourceClass, classRef);
        }
        else
        {
            editPanel.GetChild(1).GetChild(1).GetComponent<Dropdown>().value = Iml.GetSingleton().structuralModel.classes.IndexOf(relationReference.sourceClass);
            relationReference.destinationClass.relations.Add(relationReference);
            PrintError(message);
        }
    }

    public void SaveName(string name)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("EditRelation", RpcTarget.MasterClient, "NAME", name);
            return;
        }
        InputField nameField = editPanel.GetChild(0).GetChild(1).GetComponent<InputField>();
        if (ValidateName(name)) { relationReference.SetName(name); } else { nameField.text = relationReference.name; }
    }

    private bool ValidateName(string candidateName)
    {
        if (candidateName == relationReference.name)
            return true;
        if (relationReference.sourceClass.FindAttribute(candidateName) != null || relationReference.sourceClass.FindRelation(candidateName) != null)
        {
            PrintError("IML Class " + relationReference.sourceClass.name + " already contains, inherits, or is inherited by a Class with an attribute/relation with the name \"" + candidateName + "\". Please select a unique relation name.");
            return false;
        }

        return ValidateName(candidateName, "Relation");
    }
}
