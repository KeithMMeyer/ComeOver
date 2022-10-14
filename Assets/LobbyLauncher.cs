using UnityEngine;
using System;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class LobbyLauncher : MonoBehaviourPunCallbacks
{
	/*// A list of available rooms the user can join
	public List<string> roomNames = new List<string>();
	// A list of the buttons corresponding to rooms
	public List<GameObject> roomButtons = new List<GameObject>();
	// Start is called before the first frame update
	public GameObject LobbyButtonPrefab;

    void Start()
    {
		buttonMaker();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		foreach (RoomInfo room in roomList)
		{
			// Adds the names of each room to a list to use later
			roomNames.Add(room.Name);
			// Rooms that no longer exist can still show up in roomList.
			// The RemovedFromList parameter will determine if the room
			// is still available.
			if (!room.RemovedFromList)
			{
				// Removes the room from the list if the room is not available
				roomNames.Remove(room.Name);
			}
		}

		Debug.Log(roomNames);
		//Debug.Log($"There are {i} rooms.");
	}

	public void buttonMaker()
    {
		foreach (string name in roomNames)
		{
			GameObject button = (GameObject)Instantiate(LobbyButtonPrefab);
			button.GetComponent<Button>().onClick.AddListener(PhotonNetwork.JoinRoom(name));
			roomButtons.Add(button);
		}
	}*/

}
