using Photon.Pun;
using UnityEngine;

public class AttributeView : MonoBehaviour, IPunObservable
{

    public string lowerBound;
    public string upperBound;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (transform.GetComponent<Identity>().attributeReference != null)
            {
                stream.SendNext(transform.GetComponent<Identity>().attributeReference.lowerBound);
                stream.SendNext(transform.GetComponent<Identity>().attributeReference.upperBound);
            }
            else
            {
                stream.SendNext("NULL");
                stream.SendNext("NULL");
            }
        }
        else
        {
            string output1 = (string)stream.ReceiveNext();
            if (!output1.Equals("NULL"))
                lowerBound = output1;
            string output2 = (string)stream.ReceiveNext();
            if (!output2.Equals("NULL"))
                upperBound = output2;
           
        }
    }
}
