using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModelButtonScript : MonoBehaviour
{
	public delegate void ModelButtonClicked();
	public static event ModelButtonClicked OnModelButtonClicked;

	public string modelId;

	private void Start()
	{
		modelId = gameObject.GetComponentInChildren<TextMeshProUGUI>().text;
	}

	public void OnClick()
	{
		// Structure of URL is http://iml.cec.miamioh.edu:5000/CoMoVRAPI/GetModel?userID=userID&diagramID=modelID
		// Gets the userID from preferences
		string userID = PlayerPrefs.GetString("userID");
		Debug.Log("Model button clicked: " + modelId);
		string url = "http://iml.cec.miamioh.edu:5000/CoMoVRAPI/GetModel?userID=" + userID + "&diagramID=" + modelId;
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
		PhotonNetwork.RaiseEvent(EventCodes.LoadSceneEventCode, url, raiseEventOptions, SendOptions.SendReliable);
	}
}
