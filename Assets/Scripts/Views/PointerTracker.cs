using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTracker : LineView
{
    public override bool useColor { get; set; } = false;

    // Update is called once per frame
    void Update()
    {

    }

    protected override void SetRenderer()
    {
        if (PhotonView.Get(this).IsMine)
        {
            GameObject controller = GameObject.Find("RightHand Controller");
            if (controller != null)
                lineRenderer = controller.GetComponent<LineRenderer>();
        } else
        {
            base.SetRenderer();
        }
    }
}
