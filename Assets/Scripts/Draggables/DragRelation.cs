using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class DragRelation : DragObject
{

    public override void Dropped(SelectExitEventArgs args)
    {
        base.Dropped(args);

        List<Collider> collisionList = GetComponentInChildren<Collision>(true).collisionList;
        transform.GetChild(0).gameObject.SetActive(false); // turn off collider
        foreach (Collider c in collisionList)
        {
            if (c.transform == trash)
            {
                Trash();
                return;
            }
        }

        PlaceRelation(collisionList);
    }

    private bool PlaceRelation(List<Collider> collisionList)
    {
        GameObject classObject = null;

        foreach (Collider c in collisionList)
        {
            if (classObject == null && c.gameObject.layer == 6) //classes
            {
                classObject = c.gameObject;
                break;
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            PlacingRelation(classObject);
        }
        else
        {
            string id = classObject == null ? "NULL" : classObject.GetComponentInParent<ClassView>().id;

            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("PlacingRelation", RpcTarget.MasterClient, id);
            return true;
        }

        return true;
    }

    public void PlacingRelation(GameObject classObject)
    {
        Relation relation = gameObject.GetComponentInParent<Identity>().relationReference;

        if (classObject == null)
        {
            if (storage != null)
            {
                if (transform.parent.name.Equals("Arrow"))
                {
                    relation.AttachToClass(relation.sourceClass, storage);
                }
                else
                {
                    relation.AttachToClass(storage, relation.destinationClass);
                }
                storage = null;
            }
            else
            {
                Destroy(relation.gameObject);
            }
            return;
        }

        UserClass newClass = classObject.GetComponentInParent<Identity>().classReference;

        bool isArrow = transform.parent.name.Equals("Arrow");
        string message;
        if ((isArrow && !relation.CanAttach(relation.sourceClass, newClass, out message)) || (!isArrow && !relation.CanAttach(newClass, relation.destinationClass, out message)))
        {
            if (lockView.IsLocked && lockView.HasLock)
            {
                errorPanel.gameObject.SetActive(true);
                errorPanel.GetChild(1).GetComponent<Text>().text = message;
            }
            else
            {
                PhotonView photonView = PhotonView.Get(this);
                photonView.RPC("PrintError", RpcTarget.Others, message);
            }
            return;
        }

        if (storage != null)
        {
            storage.relations.Remove(relation);
        }
        else
        {
            Iml.GetSingleton().structuralModel.relations.Add(relation);
        }
        if (isArrow)
        {
            relation.AttachToClass(relation.sourceClass, newClass);
        }
        else
        {
            relation.AttachToClass(newClass, relation.destinationClass);
        }
        gameObject.GetComponent<EditRelation>().SetUpPositions();
        storage = null;
        return;
    }

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
