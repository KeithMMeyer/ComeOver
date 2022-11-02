using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class LobbyGameManager : MonoBehaviour
{
    [SerializeField]
    private Text lobbyPageNumber;
    [SerializeField]
    public Button[] buttons;

    private int curretLobbyPage = 0;
    public string[] roomNames;

    public string[] getLobbyNames()
    {
        int j = curretLobbyPage * 6;
        string[] currentPageNames = new string[6];
        for(int i=0; i<6 && (j+i) < roomNames.Length; i++)
        {
            currentPageNames[i] = roomNames[j + i];
        }
        return currentPageNames;
        //Gets the name of 5 or so lobbies and puts one with each button
    }

    private void initializeButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];

            int buttonIndex = i;
            button.onClick.AddListener(() => testButton(button.GetComponentInChildren<Text>().text));
        }
    }

    private void setupCurrentLobbypage()
    {
        //gets the correct info for each button that will lead to a lobby to present and puts it into the ui
        lobbyPageNumber.text = (curretLobbyPage + 1) + " of " + "Stuff";
        string[] roomsOnPage = getLobbyNames();
        for (int i =0; i < buttons.Length; i++)
        {
            buttons[i].GetComponentInChildren<Text>().text = roomsOnPage[i]; //will change text to lobby name;
        }
    }

    public void UpdateLobbyPage()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        initializeButtons();
        setupCurrentLobbypage();
        LobbyConnection con = new LobbyConnection();
        con.getRoomNames(roomNames);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void testButton(string name)
    {
        Debug.Log("" + name);
    }
}

public class LobbyConnection : MonoBehaviourPunCallbacks
{
    private List<string> roomNames = new List<string>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            //add names to a list to use later
            if(!room.RemovedFromList)
            {
                roomNames.Add(room.Name);
            }
        }
    }

    public void getRoomNames(string[] rooms)
    {
        rooms = new string[roomNames.Count];
        int i = 0;
        foreach(string s in roomNames) {
            rooms[i] = s;
            i++;
        }
        //return rooms;
    }
}
