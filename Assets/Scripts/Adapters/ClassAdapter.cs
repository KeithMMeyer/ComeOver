using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassAdapter : ObjectAdapter
{

    [PunRPC]
    public void UpdateRelations(Vector3 position)
    {
        UserClass classReference = transform.GetComponent<Identity>().classReference;
        //TODO update stored position
        classReference.UpdateRelations();

    }

    [PunRPC]
    public void EditClass(string type, string value)
    {
        EditClass ec = transform.GetComponentInChildren<EditClass>();
        if (type.Equals("NAME"))
            ec.SaveName(value);
        if (type.Equals("ISABSTRACT"))
            ec.SaveIsAbstract(int.Parse(value));
    }
}
