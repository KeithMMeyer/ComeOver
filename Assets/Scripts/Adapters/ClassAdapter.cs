using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassAdapter : MonoBehaviour
{

    [PunRPC]
    public void UpdateRelations(Vector3 position)
    {
        UserClass classReference = transform.GetComponent<Identity>().classReference;
        //TODO update stored position
        classReference.UpdateRelations();

    }
}
