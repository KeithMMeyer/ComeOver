using UnityEngine;
using System;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class LobbyGameManager : MonoBehaviour
{
   
    public void TestingButtonFucntion()
    {
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Seeing if this works");
    }
}
