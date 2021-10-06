using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBoxObject : MonoBehaviour
{

    public String type;
    void OnMouseDown()
    {
        if (type == "class")
        {
            ClassXml newClass = new ClassXml();
            newClass.name = "New Class";
            newClass.createGameObject();

            Vector3 position = transform.position;
            //position.y += 1;
            newClass.gameObject.transform.position = position;
        } else
        {
            AttributeXml newAttribute = new AttributeXml();
            newAttribute.name = "New Attribute";
            newAttribute.value = "STRING";
            newAttribute.createGameObject();

            Vector3 position = transform.position;
            //position.y += 1;
            newAttribute.gameObject.transform.position = position;
        }


    }
}
