using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Quit : MonoBehaviourPunCallbacks
{
    public void quitToLobby()
	{
		PhotonNetwork.LeaveRoom();
	}

	public override void OnLeftRoom()
	{
		Debug.Log("Left Room");
		SceneManager.LoadScene("LobbyScene");
	}
}
