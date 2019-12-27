using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviourPunCallbacks
{

    [SerializeField] private Text waitingStatusText = null;
    [SerializeField] private InputField nameInputField = null;

    //const vals
    private const string GameVersion = "0.1";
    private const int MaxPlayersPerRoom = 2;
    private const string PlayerPrefsNameKey = "PlayerName";

    //stored nickname in case we need them in the game itself
    public string nickName;
    //bool to check whether we are "host" player
    public bool isHost;

    bool isConnecting = false;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        DontDestroyOnLoad(this);
        isHost = false;
        SetUpNickname();
    }
    public void StartOffline()
    {
        PhotonNetwork.OfflineMode = true;
        isHost = true;
        SceneManager.LoadScene("Main_Game");
    }
    void SetUpNickname()
    {
        //do we have def name stored in playerprefs?
        if (PlayerPrefs.GetString(PlayerPrefsNameKey) == "")
        {
        }
        //all good, continue
        else
        {
            nickName = PlayerPrefs.GetString(PlayerPrefsNameKey);
            nameInputField.text = nickName;
            PhotonNetwork.NickName = nickName;
        }

    }
    public void SaveNickname()
    {
        nickName = nameInputField.text;
        PlayerPrefs.SetString(PlayerPrefsNameKey, nickName);
        PhotonNetwork.NickName = nickName;
    }
    public void FindOpponet()
    {
        SaveNickname();
        if (PlayerPrefs.GetString(PlayerPrefsNameKey) == "")
        {
            waitingStatusText.text += "\n Please input your nickname before continuing!";
        }
        else
        {
            isConnecting = true;
            waitingStatusText.text += "\n Hello " + PhotonNetwork.NickName + "!";
            waitingStatusText.text += "\n Searching...";
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.GameVersion = GameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        waitingStatusText.text += "\n Connected to Master.";
        waitingStatusText.text += "\n Current Server Region: " + PhotonNetwork.CloudRegion;
        if (isConnecting)
            PhotonNetwork.JoinRandomRoom();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {

        waitingStatusText.text += $"\n Disconnected due to: {cause}";
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        waitingStatusText.text += "\n No clients waiting for a Player, creating new room";
        isHost = true;
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MaxPlayersPerRoom });

    }
    public override void OnJoinedRoom()
    {
        waitingStatusText.text += "\n Client Joined a room";
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount != MaxPlayersPerRoom)
        {
            waitingStatusText.text += "\n Waiting for Player";
        }
        else
        {
            waitingStatusText.text += "\n Player found";
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayersPerRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            waitingStatusText.text += "\n Player found";

            PhotonNetwork.LoadLevel("Main_Game");
        }
    }

}
