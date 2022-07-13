using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class TextView : MonoBehaviour, IPunObservable
{
    public string text;
    //public Color color;

    TextMesh mesh;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(mesh.text);
            //Vector3 colorVec = new Vector3(mesh.color.r, mesh.color.g, mesh.color.b);
            //stream.SendNext(mesh.text != null && !mesh.text.Equals("") ? mesh.text : "Loading");
            //stream.SendNext(colorVec);
            //Debug.Log("Writing!");
        }
        else
        {
            text = (string)stream.ReceiveNext();
            //Vector3 colorVec = (Vector3)stream.ReceiveNext();
            //color = new Color(colorVec.x, colorVec.y, colorVec.z);

            if (mesh == null)
                if (transform.GetComponent<TextMesh>() != null)
                {
                    mesh = transform.GetComponent<TextMesh>();
                }
                else
                {
                    return;
                }
            mesh.text = text;
            //mesh.color = color;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mesh = transform.GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
