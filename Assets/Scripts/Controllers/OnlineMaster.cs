using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OnlineMaster : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private SpawnController spawnCont = null;
    GameMaster gm;
    MainMenu menu;
    public bool isHost;

    private void Start()
    {
        gm = gameObject.GetComponent<GameMaster>();
        menu = FindObjectOfType<MainMenu>();
        if (menu != null)
        {
            isHost = menu.isHost;
            Destroy(menu.gameObject);
        }
        if (isHost)
        {
            foreach (Transform trans in spawnCont.spawnPoints)
            {
                CreateEnemy(trans);
            }
        }
    }
    void CreateEnemy(Transform trans)
    {
        GameObject tmp = PhotonNetwork.Instantiate("Enemy", trans.position, Quaternion.identity);
        tmp.GetComponent<EnemyController>().InitializeEnemy(!PhotonNetwork.OfflineMode);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //throw new System.NotImplementedException();
    }
}
