using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DragClass : DragObject
{

    // Update is called once per frame
    void Update()
    {
        if (grabbed)
        {
            if (TryUpdatePosition(out Vector3 position))
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    UpdateClassRelations(position);
                }
                else
                {
                    PhotonView photonView = PhotonView.Get(this);
                    photonView.RPC("UpdateClassRelations", RpcTarget.MasterClient, position);
                }
            }
        }
    }

    private void UpdateClassRelations(Vector3 position)
    {
        UserClass classReference = transform.parent.GetComponent<Identity>().classReference;
        classReference.UpdateRelations();

    }

    protected override void DropThis(List<Collider> collisionList)
    {
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

    protected override void TrashThis()
    {
        UserClass classReference = gameObject.GetComponentInParent<Identity>().classReference;

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
