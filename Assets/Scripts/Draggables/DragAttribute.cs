using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class DragAttribute : DragObject
{

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
        bool placed = PlaceAttribute(collisionList);

        if (!placed)
        {
            if (lockView.IsLocked && lockView.HasLock)
            {
                errorPanel.gameObject.SetActive(true);
                errorPanel.GetChild(1).GetComponent<Text>().text = "Attributes can only be added to IML Classes.";
            }
            else
            {
                PhotonView photonView = PhotonView.Get(this);
                photonView.RPC("PrintError", RpcTarget.Others, "Attributes can only be added to IML Classes.");
            }
            if (gameObject.GetComponentInParent<Identity>().attributeReference.parent != null)
            {
                gameObject.GetComponentInParent<Identity>().attributeReference.parent.Resize();
            }
            else
            {
                Destroy(gameObject.transform.parent.gameObject);
            }
        }
    }

    private bool PlaceAttribute(List<Collider> collisionList)
    {
        GameObject classObject = null;
        GameObject attributeObject = null;

        foreach (Collider c in collisionList)
        {
            if (classObject == null && c.gameObject.layer == 6) //classes
            {
                classObject = c.gameObject;
            }
            if (attributeObject == null && c.gameObject.layer == 7) //attributes
            {
                attributeObject = c.gameObject;
            }
        }

        if (classObject == null)
            return false;

        if (PhotonNetwork.IsMasterClient)
        {
            PlacingAttribute(classObject, attributeObject);
        }
        else
        {
            string id = classObject.GetComponentInParent<ClassView>().id;
            string text = null;
            if (attributeObject != null)
                text = attributeObject.transform.parent.GetChild(1).GetComponent<Text>().text;

            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("PlacingAttribute", RpcTarget.MasterClient, id, text);
            return true;
        }

        return true;
    }

    public void PlacingAttribute(string classId, string attributeText)
    {
        UserClass classRefence = null;
        GameObject attributeObject = null;

        foreach (UserClass c in Iml.GetSingleton().structuralModel.classes)
        {
            if (c.id.Equals(classId))
            {
                classRefence = c;
                break;
            }
        }
        if (classRefence == null)
            Debug.LogError("Tried to place attribute but no class match found! ID:'" + classId + "'!");
        foreach (UserAttribute a in classRefence.attributes)
        {
            if (a.displayString.Equals(attributeText))
            {
                attributeObject = a.gameObject.transform.GetChild(0).gameObject;
                break;
            }
        }
        PlacingAttribute(classRefence.gameObject.transform.GetChild(0).gameObject, attributeObject);
    }

    private void PlacingAttribute(GameObject classObject, GameObject attributeObject)
    {
        UserAttribute attribute = gameObject.GetComponentInParent<Identity>().attributeReference;
        //UserAttribute other = attribute.parent.FindAttribute(name);
        //if (attribute.parent.FindRelation(name) != null || (other != null && other != attribute))
        //{
        //    errorPanel.gameObject.SetActive(true);
        //    errorPanel.GetChild(1).GetComponent<Text>().text = "Changing this attribute would result a duplicate name for attributes and/or relations; update aborted.";
        //    return false;
        //}

        if (attribute.parent != null)
        {
            UserClass oldClass = attribute.parent;
            oldClass.attributes.Remove(attribute);
            oldClass.Resize();
        }

        UserClass newClass = classObject.GetComponentInParent<Identity>().classReference;

        int position = 0;

        if (classObject.transform.position.y > transform.position.y)
            position = newClass.attributes.Count;

        if (attributeObject != null)
            if (newClass.attributes.Contains(attributeObject.transform.parent.GetComponent<Identity>().attributeReference) && attributeObject.transform.position.y > transform.position.y)
                position = newClass.attributes.IndexOf(attributeObject.transform.parent.GetComponent<Identity>().attributeReference) + 1;

        newClass.attributes.Insert(position, attribute);
        newClass.Resize();
    }

    protected override void TrashThis()
    {
        UserAttribute attribute = gameObject.GetComponentInParent<Identity>().attributeReference;

        //Destroy(attribute.gameObject);
        PhotonNetwork.Destroy(attribute.gameObject);
        List<UserClass> imlClasses = Iml.GetSingleton().structuralModel.classes;
        foreach (UserClass uc in imlClasses)
        {
            if (uc.attributes.Contains(attribute))
            {
                uc.attributes.Remove(attribute);
                uc.Resize();
                break;
            }
        }
    }
}
