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

            InputField nameField = editPanel.GetChild(0).GetChild(1).GetComponent<InputField>();
            nameField.onEndEdit.RemoveAllListeners();
            nameField.placeholder.GetComponent<Text>().text = classReference.name;
            nameField.text = classReference.name;
            nameField.onEndEdit.AddListener(delegate (string name) { classReference.SetName(name); });

            Dropdown abstractField = editPanel.GetChild(1).GetChild(1).GetComponent<Dropdown>();
            abstractField.onValueChanged.RemoveAllListeners();
            abstractField.value = classReference.isAbstract.Equals("TRUE") ? 1 : 0;
            abstractField.onValueChanged.AddListener(delegate (int isAbstract) { classReference.SetAbstract(isAbstract == 1); });
        }
    }
}
