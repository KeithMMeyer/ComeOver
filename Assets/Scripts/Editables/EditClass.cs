using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class EditClass : EditObject
{

    private UserClass classReference;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        editPanel = toolbox.transform.GetChild(0).GetChild(1);
        interactable.selectEntered.AddListener(OpenDrawer);
        classReference = transform.GetComponentInParent<Identity>().classReference;
    }

    protected override void OpenDrawer(SelectEnterEventArgs args)
    {
        if (toolbox.relationMode != null)
        {
            toolbox.InsertRelation(classReference, args);
            return;
        }
        else
        {
            base.OpenDrawer(args);

            string name;
            int isAbstract;
            if (PhotonNetwork.IsMasterClient)
            {
                name = classReference.name;
                isAbstract = classReference.isAbstract.Equals("TRUE") ? 1 : 0;
            }
            else
            {
                name = transform.parent.GetChild(1).GetComponent<TextMesh>().text;
                bool? isAbs = transform.GetComponentInParent<ClassView>().isAbstract;
                isAbstract = isAbs != null ? isAbs.Value ? 1 : 0 : 0;
            }

            InputField nameField = editPanel.GetChild(0).GetChild(1).GetComponent<InputField>();
            nameField.onEndEdit.RemoveAllListeners();
            nameField.placeholder.GetComponent<Text>().text = name;
            nameField.text = name;
            nameField.onEndEdit.AddListener(delegate (string name) { if(ValidateName(name)){ classReference.SetName(name); } else { nameField.text = classReference.name; }
        });

            Dropdown abstractField = editPanel.GetChild(1).GetChild(1).GetComponent<Dropdown>();
            abstractField.onValueChanged.RemoveAllListeners();
            abstractField.value = isAbstract;
            abstractField.onValueChanged.AddListener(delegate (int isAbstract) { classReference.SetAbstract(isAbstract == 1); });
        }
    }

    protected override bool ValidateName(string candidateName)
    {
        foreach (UserClass c in Iml.GetSingleton().structuralModel.classes)
            if (c.name.Equals(candidateName) && !c.Equals(classReference))
            {
                PrintError("IML Structural Model already contains a Class with the name " + candidateName + ". Please use unqiue Class names.");
                return false;
            }
        return base.ValidateName(candidateName);
    }

    }
