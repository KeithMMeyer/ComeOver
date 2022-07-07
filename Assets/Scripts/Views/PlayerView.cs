using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{

    GameObject sphere;

    // Start is called before the first frame update
    void Start()
    {
        sphere = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        PhotonView photonView = PhotonView.Get(this);
        if (sphere == null)
            sphere = transform.GetChild(0).gameObject;
        if (!photonView.AmOwner)
                sphere.gameObject.SetActive(true);
    }
}
