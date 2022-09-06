using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAdapter : MonoBehaviour
{

    [PunRPC]
    public void CreateObject(string type)
    {
        ToolBox toolBox = GameObject.Find("ToolBox").GetComponent<ToolBox>();
        toolBox.CreateObject(type);

    }

    [PunRPC]
    public void InsertRelation(string id, string relationMode)
    {
        ToolBox toolBox = GameObject.Find("ToolBox").GetComponent<ToolBox>();
        UserClass target = null;
        foreach(UserClass c in Iml.GetSingleton().structuralModel.classes)
            if (c.id.Equals(id))
            {
                target = c;
                break;
            }
        toolBox.relationMode = relationMode;
        toolBox.InsertRelation(target, null);

    }

    [PunRPC]
    public void EditModel(string type, string value)
    {
        ToolBox toolBox = GameObject.Find("ToolBox").GetComponent<ToolBox>();
        if (type.Equals("NAME"))
            toolBox.SaveName(value);
        if (type.Equals("ROUTE"))
            toolBox.SaveRoute(int.Parse(value));
    }

}
