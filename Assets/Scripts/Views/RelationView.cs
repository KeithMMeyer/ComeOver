using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelationView : LineView
{
    public string type;

    private bool firstPass = true;

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);
        if (stream.IsWriting)
        {
            stream.SendNext(transform.GetComponent<Identity>().relationReference.type);
        } else
        {
            type = (string)stream.ReceiveNext();
            if (firstPass)
            {
                if (!type.Equals("COMPOSITION"))
                {
                    //PhotonNetwork.Destroy(transform.GetChild(1).GetChild(1).gameObject);
                }
                if (type.Equals("INHERITENCE"))
                {
                    //PhotonNetwork.Destroy(transform.GetChild(2).gameObject);
                    //PhotonNetwork.Destroy(transform.GetChild(2).gameObject);
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
        // Start is called before the first frame update
    void Start()
    {
        lineRenderer = transform.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
