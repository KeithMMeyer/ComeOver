using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class EditAttribute : EditObject
{

    UserAttribute attributeReference;
    private Material defaultMaterial;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        editPanel = toolbox.transform.GetChild(0).GetChild(2);
        interactable.selectEntered.AddListener(OpenDrawer);
        defaultMaterial = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        attributeReference = transform.GetComponentInParent<Identity>().attributeReference;
    }

    protected override void OpenDrawer(SelectEnterEventArgs args)
    {
        base.OpenDrawer(args);

        string visibility;
        string name;
        int type;
        string value;
        if (PhotonNetwork.IsMasterClient)
        {
            visibility = attributeReference.visibility;
            name = attributeReference.name;
            type = GetTypeNumber(attributeReference.type);
            value = attributeReference.value;
        }
        else
        {
            string[] text = transform.parent.GetChild(1).GetComponent<TextMesh>().text.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Debug.LogWarning(transform.parent.GetChild(1).GetComponent<TextMesh>().text);
            visibility = text[1].Equals("+") ? "PUBLIC" : text[1].Equals("-") ? "PRIVATE" : "PROTECTED";
            name = text[2];
            type = GetTypeNumber(text[4]);
            value = text.Length > 5 ? text[6] : ""; //fix to not break on spaces

        }

        Dropdown visibilityField = editPanel.GetChild(0).GetChild(1).GetComponent<Dropdown>();
        visibilityField.onValueChanged.RemoveAllListeners();
        string[] visibilityArray = { "PUBLIC", "PRIVATE", "PROTECTED" };
        List<string> visibilities = visibilityArray.ToList();
        visibilityField.value = visibilities.IndexOf(visibility);
        visibilityField.onValueChanged.AddListener(SaveVisibility);

        InputField nameField = editPanel.GetChild(1).GetChild(1).GetComponent<InputField>();
        nameField.onEndEdit.RemoveAllListeners();
        nameField.placeholder.GetComponent<Text>().text = name;
        nameField.text = name;
        nameField.onEndEdit.AddListener(SaveName);

        Dropdown typeField = editPanel.GetChild(2).GetChild(1).GetComponent<Dropdown>();
        typeField.onValueChanged.RemoveAllListeners();
        typeField.value = type;
        typeField.onValueChanged.AddListener(SaveType);

        SetUpBounds();

        InputField valueField = editPanel.GetChild(5).GetChild(1).GetComponent<InputField>();
        valueField.onEndEdit.RemoveAllListeners();
        valueField.placeholder.GetComponent<Text>().text = value;
        valueField.text = value;
        valueField.onEndEdit.AddListener(SaveValue);

        UpdateColor(true);
    }

    private void SetUpBounds()
    {
        string lower;
        string upper;
        if (PhotonNetwork.IsMasterClient)
        {
            lower = attributeReference.lowerBound;
            upper = attributeReference.upperBound;
        }
        else
        {
            lower = transform.GetComponentInParent<AttributeView>().lowerBound;
            upper = transform.GetComponentInParent<AttributeView>().upperBound;
        }
        InputField lowerBound = editPanel.GetChild(3).GetChild(1).GetComponent<InputField>();
        lowerBound.onEndEdit.RemoveAllListeners();
        lowerBound.placeholder.GetComponent<Text>().text = lower;
        lowerBound.text = lower;
        lowerBound.onEndEdit.AddListener(SaveLower);
        editPanel.GetChild(3).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
        editPanel.GetChild(3).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { BumpField(lowerBound, true); });
        editPanel.GetChild(3).GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
        editPanel.GetChild(3).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { BumpField(lowerBound, false); });

        InputField upperBound = editPanel.GetChild(4).GetChild(1).GetComponent<InputField>();
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
            photonView.RPC("EditAttribute", RpcTarget.MasterClient, "LOWER", s);
            return;
        }
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

    public void SaveUpper(string s)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("EditAttribute", RpcTarget.MasterClient, "UPPER", s);
            return;
        }
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

    public void SaveVisibility(int i)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("EditAttribute", RpcTarget.MasterClient, "VISIBILITY", i.ToString());
            return;
        }
        string[] visibilityArray = { "PUBLIC", "PRIVATE", "PROTECTED" };
        attributeReference.visibility = visibilityArray[i];
        attributeReference.GenerateDisplayString();
    }

    public void SaveName(string name)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("EditAttribute", RpcTarget.MasterClient, "NAME", name);
            return;
        }
        InputField nameField = editPanel.GetChild(1).GetChild(1).GetComponent<InputField>();
        if (ValidateName(name)) { attributeReference.SetName(name); } else { nameField.text = attributeReference.name; }
    }

    public void SaveType(int i)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("EditAttribute", RpcTarget.MasterClient, "TYPE", i.ToString());
            return;
        }
        attributeReference.SetType(i);
        editPanel.GetChild(5).GetChild(1).GetComponent<InputField>().text = attributeReference.value;
    }

    public void SaveValue(string value)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("EditAttribute", RpcTarget.MasterClient, "VALUE", value);
            return;
        }
        string message = attributeReference.SetValue(value);
        if (message != null)
        {
            PrintError(message);
            editPanel.GetChild(5).GetChild(1).GetComponent<InputField>().text = attributeReference.value;
        }
    }

    private bool ValidateName(string candidateName)
    {
        if (candidateName == attributeReference.name)
            return true;
        if (attributeReference.parent.FindAttribute(candidateName) != null || attributeReference.parent.FindRelation(candidateName) != null)
        {
            PrintError("IML Class " + attributeReference.parent.name + " already contains, inherits, or is inherited by a Class with an attribute/relation with the name \"" + candidateName + "\". Please select a unique attribute name.");
            return false;
        }

        return ValidateName(candidateName, "Attribute");
    }

    public int GetTypeNumber(string type)
    {
        string[] typeArray = { "STRING", "BOOLEAN", "DOUBLE", "INTEGER" };
        List<string> types = typeArray.ToList();
        return types.IndexOf(type);
    }

    protected override void UpdateColor(bool isLocked)
    {
        if (isLocked)
        {
            transform.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/LockedColor");
        }
        else
        {
            transform.GetComponent<MeshRenderer>().material = defaultMaterial;
        }
    }
}
