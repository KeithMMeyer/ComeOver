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
    [SerializeField]
    private Button incrementer;
    [SerializeField]
    private Button decrementer;

    private int currentLobbyPage = 0;
    public string[] roomNames;

    public string[] getLobbyNames()
    {
        int j = currentLobbyPage * 6;
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
        incrementer.onClick.AddListener(() => changePage(1));
        decrementer.onClick.AddListener(() => changePage(-1));
    }

    private void setupCurrentLobbypage()
    {
        //gets the correct info for each button that will lead to a lobby to present and puts it into the ui
        double numPages = roomNames.Length / 6;
        lobbyPageNumber.text = (currentLobbyPage + 1) + " of " + Math.Ceiling(numPages);
        string[] roomsOnPage = getLobbyNames();
        for (int i =0; i < buttons.Length; i++)
        {
            buttons[i].GetComponentInChildren<Text>().text = roomsOnPage[i]; //will change text to lobby name;
        }
    }

    private void changePage(int changer)
    {
        double numPages = roomNames.Length / 6;
        if ((changer == -1 && currentLobbyPage == 0) || (changer == 1 && currentLobbyPage == Math.Ceiling(numPages)))
        {
            Debug.Log("Nothing Changed");
        }
        else
        {
            currentLobbyPage += changer;
            Debug.Log("Something Changed");
            setupCurrentLobbypage();
        }

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
                Debug.Log(room.Name);
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
