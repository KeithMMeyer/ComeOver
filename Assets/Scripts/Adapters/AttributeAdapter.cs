using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeAdapter : MonoBehaviour
{
    [PunRPC]
    public void TrashObject()
    {
        transform.GetComponentInChildren<Drag>().TrashObject();
    }

    [PunRPC]
    public void PlacingAttribute(string classId, string attributeText)
    {
        transform.GetComponentInChildren<Drag>().PlacingAttribute(classId, attributeText);
    }
}
