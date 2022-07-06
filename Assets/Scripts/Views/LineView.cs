using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineView : MonoBehaviour, IPunObservable
{
    public Vector3 start;
    public Vector3 end;
    public bool useColor = true;
    public Color color;



    protected LineRenderer lineRenderer;

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(lineRenderer.GetPosition(0));
            stream.SendNext(lineRenderer.GetPosition(1));
            Vector3 colorVec = new Vector3(lineRenderer.endColor.r, lineRenderer.endColor.g, lineRenderer.endColor.b);
            stream.SendNext(colorVec);
            //Debug.Log("Writing!");
        }
        else
        {
            //Debug.LogWarning(text + " " + stream.Count);
            start = (Vector3)stream.ReceiveNext();
            end = (Vector3)stream.ReceiveNext();
            Vector3 colorVec = (Vector3)stream.ReceiveNext();
            color = new Color(colorVec.x, colorVec.y, colorVec.z);

            if (lineRenderer == null)
                if (transform.GetComponent<TextMesh>() != null)
                {
                    SetRenderer();
                    if (lineRenderer.positionCount != 2)
                        lineRenderer.positionCount = 2;
                }
                else
                {
                    return;
                }
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
            if (useColor)
            {
                lineRenderer.startColor = color;
                lineRenderer.endColor = color;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetRenderer();
    }

    protected virtual void SetRenderer()
    {
        lineRenderer = transform.GetComponent<LineRenderer>();
    }
}
