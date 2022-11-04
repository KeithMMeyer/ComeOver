using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identity : MonoBehaviour
{
    public UserClass classReference { set; get; }
    public UserAttribute attributeReference { set; get; }
    public Relation relationReference { set; get; }

	[PunRPC]
	void DestroySelf()
	{
		// Destroys the GameObject this script is attached to with Photon Network
		PhotonNetwork.Destroy(this.gameObject);
	}
}
