using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragRelation : DragObject
{

    public override void Trash()
    {
        base.Trash();

        Relation relation = gameObject.GetComponentInParent<Identity>().relationReference;
        TrashRelation(relation);
    }

    public static void TrashRelation(Relation relation)
    {
        //Destroy(relation.gameObject);
        PhotonNetwork.Destroy(relation.gameObject);
        Iml.GetSingleton().structuralModel.relations.Remove(relation);
        if (relation.sourceClass != null)
            relation.sourceClass.relations.Remove(relation);
        if (relation.destinationClass != null)
            relation.destinationClass.relations.Remove(relation);
    }
}
