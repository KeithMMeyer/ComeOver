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
    public void InsertRelation(string id)
    {
        ToolBox toolBox = GameObject.Find("ToolBox").GetComponent<ToolBox>();
        UserClass target = null;
        foreach(UserClass c in Iml.GetSingleton().structuralModel.classes)
            if (c.id.Equals(id))
            {
                target = c;
                break;
            }

        toolBox.InsertRelation(target, null);

    }

}
