using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour 
{
    [SerializeField] private Text pingText, netText;
    [SerializeField] private GameObject netTextBackground;
    bool displayDetailedStats = false;
    public static GameMaster instance;
    void Start()
    {
        instance = this;
        PhotonNetwork.NetworkStatisticsEnabled = true;
        //AudioManager.instance.PlayOnce("ojezus");
        StaticValues.SetupNetwork();
        netTextBackground.SetActive(false);
    }
    void LateUpdate()
    {
        DebugInformation();
    }
    void DebugInformation()
    {
        pingText.text = "Ping: " + PhotonNetwork.GetPing();
        if (displayDetailedStats)
        {
            netText.text = "Network status: \n" + PhotonNetwork.NetworkStatisticsToString();
            netTextBackground.SetActive(true);

        }
        else
        {
            netTextBackground.SetActive(false);
            netText.text = "";
        }
        if (Input.GetKeyUp(KeyCode.P))
            displayDetailedStats = !displayDetailedStats;

    }
}
