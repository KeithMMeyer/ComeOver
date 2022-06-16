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
}
