using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAttribute : DragObject
{

    public override void Trash()
    {
        base.Trash();

        UserAttribute attribute = gameObject.GetComponentInParent<Identity>().attributeReference;
        TrashAttribute(attribute);
    }

    private void TrashAttribute(UserAttribute attribute)
    {
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
