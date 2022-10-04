using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectAdapter : MonoBehaviour
{
    [PunRPC]
    public void TrashObject()
    {
        transform.GetComponentInChildren<DragObject>().TrashObject();
    }

    [PunRPC]
    public void PrintError(string message)
    {
        LockView lockView = transform.GetComponent<LockView>();
        if (lockView.IsLocked && lockView.HasLock)
        {
            Transform errorPanel = GameObject.Find("Main Canvas").transform.GetChild(1);
            errorPanel.gameObject.SetActive(true);
            errorPanel.GetChild(1).GetComponent<Text>().text = message;
        }
    }
}
