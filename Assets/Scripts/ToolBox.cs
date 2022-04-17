using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolBox : MonoBehaviour
{
    public Camera viewCamera;

    private PrimaryButtonWatcher primary;
    private SecondaryButtonWatcher secondary;
    public bool PrimaryPressed = false; // used to display button state in the Unity Inspector window
    public bool SecondaryPressed = false; // used to display button state in the Unity Inspector window
    public string relationMode;

    private int relationCount = 0;


    void Start()
    {
        relationMode = null;
        primary = GetComponent<PrimaryButtonWatcher>();
        secondary = GetComponent<SecondaryButtonWatcher>();
        primary.primaryButtonPress.AddListener(OnPrimaryButtonEvent);
        secondary.secondaryButtonPress.AddListener(OnSecondaryButtonEvent);
        closeAll();
    }

    public void OnPrimaryButtonEvent(bool pressed)
    {
        closeAll();

        PrimaryPressed = pressed;

        if (pressed)
        {
            transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            relationMode = null;
        }

    }

    public void OnSecondaryButtonEvent(bool pressed)
    {
        closeAll();

        SecondaryPressed = pressed;

        if (pressed)
        {
            Transform editPanel = transform.GetChild(0).GetChild(4);
            editPanel.gameObject.SetActive(true);
            relationMode = null;
            InputField modelField = editPanel.GetChild(0).GetChild(1).GetComponent<InputField>();
            //modelField.onEndEdit.RemoveAllListeners();
            modelField.placeholder.GetComponent<Text>().text = Iml.GetSingleton().structuralModel.name;
            modelField.text = Iml.GetSingleton().structuralModel.name;
            //modelField.onEndEdit.AddListener(delegate (string name) { classReference.SetName(name); });

            InputField fileField = editPanel.GetChild(1).GetChild(1).GetComponent<InputField>();
            //modelField.onEndEdit.RemoveAllListeners();
            fileField.placeholder.GetComponent<Text>().text = Iml.GetSingleton().structuralModel.name + ".iml";
            fileField.text = Iml.GetSingleton().structuralModel.name + ".iml";
            //modelField.onEndEdit.AddListener(delegate (string name) { classReference.SetName(name); });
        }

    }

    public void closeAll()
    {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
    }

    private void Update()
    {
        Vector3 lookAtPos = new Vector3(viewCamera.transform.position.x, viewCamera.transform.position.y, viewCamera.transform.position.z);
        transform.LookAt(lookAtPos);

        if (relationMode == null)
        {
            GameObject controller = GameObject.Find("RightHand Controller");
            Gradient blueGradient = new Gradient();
            blueGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.blue, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 1.0f) }
            );
            Gradient redGradient = new Gradient();
            redGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 1.0f) }
            );
            controller.GetComponent<XRInteractorLineVisual>().validColorGradient = blueGradient;
            controller.GetComponent<XRInteractorLineVisual>().invalidColorGradient = redGradient;
        }
    }

    public void InsertRelation(UserClass classReference, SelectEnterEventArgs args)
    {
        Relation relation = new Relation
        {
            name = relationMode.Equals("INHERITENCE") ? null : "newRelation" + (relationCount > 0 ? "" + relationCount : ""),
            type = relationMode
        };
        relation.CreateGameObject();
        relation.sourceClass = classReference;
        Vector3 other = classReference.gameObject.transform.position;
        other.x += 1;
        relation.SetPoints(classReference.gameObject.transform.position, other);
        //relation.AttachToClass(classReference, Iml.GetSingleton().structuralModel.classes[0]);
        GameObject.Find("XR Interaction Manager").GetComponent<XRInteractionManager>().SelectExit(args.interactorObject, args.interactableObject);
        GameObject.Find("XR Interaction Manager").GetComponent<XRInteractionManager>().SelectEnter(
            args.interactorObject, relation.gameObject.transform.GetChild(0).GetChild(0).GetComponent<XRSimpleInteractable>());
        relation.gameObject.transform.GetChild(0).GetChild(0).GetComponent<EditRelation>().Init(); // so edit listeners will be called first
        relation.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Drag>().Grabbed(args);
        relationMode = null;
        return;
    }

}