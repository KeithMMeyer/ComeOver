using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolBox : MonoBehaviour
{
    public Camera viewCamera;
    private Vector3 velocity = Vector3.zero;

    private PrimaryButtonWatcher watcher;
    public bool IsPressed = false; // used to display button state in the Unity Inspector window
    public string relationMode = null;

    private int relationCount = 0;


    void Start()
    {
        watcher = GetComponent<PrimaryButtonWatcher>();
        watcher.primaryButtonPress.AddListener(onPrimaryButtonEvent);
        closeAll();
    }

    public void onPrimaryButtonEvent(bool pressed)
    {
        closeAll();

        IsPressed = pressed;

        if (pressed)
        {
            transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            relationMode = null;
        }

    }

    public void closeAll()
    {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
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
        relation.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Drag>().Grabbed(args);
        relationMode = null;
        return;
    }

}