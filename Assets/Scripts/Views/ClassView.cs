using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassView : MonoBehaviour, IPunObservable
{

    public string id;
    public bool isAbstract;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (transform.GetComponent<Identity>().classReference != null)
            {
                stream.SendNext(transform.GetComponent<Identity>().classReference.id);
                stream.SendNext(transform.GetComponent<Identity>().classReference.isAbstract.Equals("TRUE") ? 1 : 0);
            }
            else
            {
                stream.SendNext("NULL");
                stream.SendNext(3);
            }
        }
        else
        {
            string output1 = (string)stream.ReceiveNext();
            if (!output1.Equals("NULL"))
                id = output1;
            int output2 = (int)stream.ReceiveNext();
            if (output2 == 3)
                return;

            isAbstract = output2 == 1;

            if (!transform.GetComponent<LockView>().IsLocked)
            {
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
}
