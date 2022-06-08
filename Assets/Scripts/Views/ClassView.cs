using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassView : MonoBehaviour, IPunObservable
{

    public bool isAbstract;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if(transform.GetComponent<Identity>().classReference != null)
                stream.SendNext(transform.GetComponent<Identity>().classReference.isAbstract.Equals("TRUE"));
        }
        else
        {
            isAbstract = (bool)stream.ReceiveNext();
            if (isAbstract)
            {
                gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/AbstractColor");

            }
            else
            {
                gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/UIColor");
            }
        }
    }
}
