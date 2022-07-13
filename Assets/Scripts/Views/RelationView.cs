using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelationView : LineView
{
    public string type;
    public string source;
    public string destination;

    private bool firstPass = true;

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);
        if (stream.IsWriting)
        {
            string sourceRef = transform.GetComponent<Identity>().relationReference.source;
            string destRef = transform.GetComponent<Identity>().relationReference.destination;
            stream.SendNext(transform.GetComponent<Identity>().relationReference.type);
            stream.SendNext(sourceRef != null ? sourceRef : "NULL");
            stream.SendNext(destRef != null ? destRef : "NULL");
        } else
        {
            type = (string)stream.ReceiveNext();
            string output1 = (string)stream.ReceiveNext();
            if (!output1.Equals("NULL"))
                source = output1;
            string output2 = (string)stream.ReceiveNext();
            if (!output2.Equals("NULL"))
                destination = output2;

            if (firstPass)
            {
                if (!type.Equals("COMPOSITION"))
                {
                    GameObject.Destroy(transform.GetChild(1).GetChild(1).gameObject);
                }
                if (type.Equals("INHERITENCE"))
                {
                    //PhotonNetwork.Destroy(transform.GetChild(2).gameObject);
                    //Destroy(transform.GetChild(2).gameObject);
                    gameObject.transform.GetChild(2).GetComponent<MeshRenderer>().forceRenderingOff = true;
                    //PhotonNetwork.Destroy(transform.GetChild(3).gameObject);
                    //Destroy(transform.GetChild(3).gameObject);
                    gameObject.transform.GetChild(3).GetComponent<MeshRenderer>().forceRenderingOff = true;
                }
                firstPass = false;
            }
            transform.GetChild(0).GetChild(1).GetComponent<TextMesh>().color = color;
            if (type.Equals("COMPOSITION"))
            {
                transform.GetChild(1).GetChild(1).GetComponent<TextMesh>().color = color;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
