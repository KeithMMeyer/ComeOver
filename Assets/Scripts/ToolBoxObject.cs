using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Random = UnityEngine.Random;

public class ToolBoxObject : MonoBehaviour
{

    public String type;

    private int classCount = 0;
    private int attrCount = 0;

    private LayerMask normalMask;

    public void CreateObject()
    {
        if (type == "CLASS")
        {
            UserClass newClass = new UserClass();
            newClass.SetName("NewClass" + (classCount > 0 ? "" + classCount : ""));
            newClass.id = ((int) Random.Range(0,500)).ToString(); //need to update
            newClass.SetPosition(new Vector3(0, 0, 3));
            classCount++;
            Iml.GetSingleton().structuralModel.classes.Add(newClass);
            newClass.CreateGameObject();

            //GameObject.Find("XR Interaction Manager").GetComponent<XRInteractionManager>().SelectEnter(GameObject.Find("RightHand Controller").GetComponent<XRRayInteractor>(), (IXRSelectInteractable) newClass.gameObject.transform.GetChild(0).GetComponent<XRSimpleInteractable>());
            //GameObject.Find("RightHand Controller").GetComponent<XRRayInteractor>().StartManualInteraction((IXRSelectInteractable)newClass.gameObject.transform.GetChild(0).GetComponent<XRSimpleInteractable>());
            //newClass.gameObject.transform.GetChild(0).GetComponent<Drag>().Grabbed(null);
        }
        else
        {
            if (type == "ATTRIBUTE")
            {
                UserAttribute newAttribute = new UserAttribute();
                newAttribute.name = "newAttr";
                newAttribute.name += attrCount > 0 ? "" + attrCount : "";
                attrCount++;
                newAttribute.value = "";
                newAttribute.CreateGameObject();

                Vector3 position = Iml.To3dPosition(0, 0, 2.99f);
                newAttribute.gameObject.transform.position = position;
                Vector3 meshScale = newAttribute.gameObject.transform.GetChild(0).transform.localScale;
                meshScale.x = newAttribute.GetWidth();
                newAttribute.gameObject.transform.GetChild(0).transform.localScale = meshScale;
                
            }
            else
            {

                GameObject controller = GameObject.Find("RightHand Controller");
                Gradient gradient = new Gradient();
                gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.green, 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 1.0f) }
                );
                controller.GetComponent<XRInteractorLineVisual>().validColorGradient = gradient;
                controller.GetComponent<XRInteractorLineVisual>().invalidColorGradient = gradient;
                //normalMask = controller.GetComponent<XRRayInteractor>().raycastMask;
                //controller.GetComponent<XRRayInteractor>().raycastMask = LayerMask.NameToLayer("Classes");
                transform.parent.parent.parent.GetComponent<ToolBox>().relationMode = type;

            }
        }
    }
}
