using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;
    [SerializeField] private GameObject staminaIndicator = null;
    [SerializeField] private Text weaponText = null;
    void Start()
    {
        GameObject tmp = PhotonNetwork.Instantiate(playerPrefab.name, this.transform.position, Quaternion.identity);
        Camera.main.GetComponent<CameraController>().Target = tmp;
        tmp.GetComponent<LocalPlayerController>().staminaIndicator = staminaIndicator;
        tmp.GetComponent<LocalPlayerController>().curWeaponText = weaponText;

    }

}
