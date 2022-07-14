using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelationAdapter : ObjectAdapter
{
    [PunRPC]
    public void EditRelation(string type, string value)
    {
        EditRelation er = transform.GetComponentInChildren<EditRelation>();
        if (type.Equals("NAME"))
            er.SaveName(value);
        if (type.Equals("LOWER"))
            er.SaveLower(value);
        if (type.Equals("UPPER"))
            er.SaveUpper(value);
        if (type.Equals("SOURCE"))
            foreach (UserClass classRef in Iml.GetSingleton().structuralModel.classes)
                if (classRef.name.Equals(value))
                    er.SaveSource(Iml.GetSingleton().structuralModel.classes.IndexOf(classRef));
        if (type.Equals("DEST"))
            foreach (UserClass classRef in Iml.GetSingleton().structuralModel.classes)
                if (classRef.name.Equals(value))
                    er.SaveDestination(Iml.GetSingleton().structuralModel.classes.IndexOf(classRef));
    }

    [PunRPC]
    public void GrabRelation()
    {
        transform.GetComponentInChildren<Drag>().GrabRelation();
    }

    [PunRPC]
    public void UpdateRelation(Vector3 position)
    {
        transform.GetComponentInChildren<Drag>().UpdateRelation(position);
    }

    [PunRPC]
    public void PlacingRelation(string id)
    {
        if (!"NULL".Equals(id))
        {
            foreach (UserClass c in Iml.GetSingleton().structuralModel.classes)
            {
                if (id.Equals(c.id))
                {
                    transform.GetComponentInChildren<Drag>().PlacingRelation(c.gameObject);
                    return;
                }
            }
        } else
        {
            transform.GetComponentInChildren<Drag>().PlacingRelation(null);
        }
    }

}
