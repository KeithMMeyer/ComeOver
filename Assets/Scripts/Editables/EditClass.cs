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
        classReference.SetName(toolbox.relationMode.ToString());
        if (toolbox.relationMode != null)
        {
            Relation relation = new Relation
            {
                name = "newRelation",
                type = toolbox.relationMode
            };
            relation.CreateGameObject();
            relation.sourceClass = classReference;
            Vector3 other = classReference.gameObject.transform.position;
            other.x += 1;
            relation.SetPoints(classReference.gameObject.transform.position, other);
            classReference.SetName("helpless");
            //relation.AttachToClass(classReference, Iml.GetSingleton().structuralModel.classes[0]);
            GameObject.Find("XR Interaction Manager").GetComponent<XRInteractionManager>().SelectExit(args.interactorObject, args.interactableObject);
            GameObject.Find("XR Interaction Manager").GetComponent<XRInteractionManager>().SelectEnter(
                args.interactorObject, relation.gameObject.transform.GetChild(0).GetChild(0).GetComponent<XRSimpleInteractable>());
            relation.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Drag>().Grabbed(args);
            toolbox.relationMode = null;
            return;
        }
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
