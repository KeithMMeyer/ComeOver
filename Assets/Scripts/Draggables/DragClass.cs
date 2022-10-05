using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DragClass : DragObject
{

    public override void Dropped(SelectExitEventArgs args)
    {
        base.Dropped(args);

        List<Collider> collisionList = GetComponentInChildren<Collision>().collisionList;
        transform.GetChild(0).gameObject.SetActive(false); // turn off collider
        foreach (Collider c in collisionList)
        {
            if (c.transform == trash)
            {
                Trash();
                return;
            }
        }
        gameObject.GetComponentInParent<Identity>().classReference.SetPosition(transform.position);
    }

    public override void Trash()
    {
        base.Trash();

        UserClass classReference = gameObject.GetComponentInParent<Identity>().classReference;
        TrashClass(classReference);
    }

    private void TrashClass(UserClass classReference)
    {
        //Destroy(classReference.gameObject);
        PhotonNetwork.Destroy(classReference.gameObject);
        foreach (UserAttribute ua in classReference.attributes)
        {
            //Destroy(ua.gameObject);
            PhotonNetwork.Destroy(ua.gameObject);
        }
        Iml.GetSingleton().structuralModel.classes.Remove(classReference);
        List<Relation> imlRelations = Iml.GetSingleton().structuralModel.relations;
        for (int i = 0; i < imlRelations.Count; i++)
        {
            if (imlRelations[i].source.Equals(classReference.id) || imlRelations[i].destination.Equals(classReference.id))
            {
                DragRelation.TrashRelation(imlRelations[i]);
                i--;
            }
        }
    }
}
