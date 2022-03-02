using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolBoxObject : MonoBehaviour
{

    public String type;

    private int classCount = 0;
    private int attrCount = 0;
    public void createObject()
    {
        if (type == "class")
        {
            UserClass newClass = new UserClass();
            newClass.name = "NewClass";
            newClass.name += classCount > 0 ? ""+classCount : "";
            classCount++;
            newClass.createGameObject();

            Vector3 position = new Vector3((float)(0 / 200.0) - 2, (float)(0 / 200.0) + 1, 3);
            //position.y += 1;
            newClass.gameObject.transform.position = position;
            GameObject.Find("XR Interaction Manager").GetComponent<XRInteractionManager>().ForceSelect(GameObject.Find("RightHand Controller").GetComponent<XRRayInteractor>(), newClass.gameObject.GetComponent<XRSimpleInteractable>());
        }
        else
        {
            UserAttribute newAttribute = new UserAttribute();
            newAttribute.name = "NewAttribute";
            newAttribute.name += attrCount > 0 ? "" + attrCount : "";
            attrCount++;
            newAttribute.value = "";
            newAttribute.createGameObject();

            Vector3 position = new Vector3((float)(0 / 200.0) - 2, (float)(0 / 200.0) + 1, 3);
            //position.y += 1;
            newAttribute.gameObject.transform.position = position;
        }
    }

    /*
    void OnMouseDown()
    {
        if (type == "class")
        {
            UserClass newClass = new UserClass();
            newClass.name = "New Class";
            newClass.createGameObject();

            Vector3 position = transform.position;
            //position.y += 1;
            newClass.gameObject.transform.position = position;
        } else
        {
            UserAttribute newAttribute = new UserAttribute();
            newAttribute.name = "New Attribute";
            newAttribute.value = "STRING";
            newAttribute.createGameObject();

            Vector3 position = transform.position;
            //position.y += 1;
            newAttribute.gameObject.transform.position = position;
        }
    }
    */
}
