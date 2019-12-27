using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviourPun, IPunObservable
{

    SpriteRenderer rend;
    Color[] colors;
    public int enemyIndex;
    void Awake()
    {
        Init();
    }
    void Init()
    {
        rend = this.gameObject.GetComponent<SpriteRenderer>();
        colors = StaticValues.GetColors();
    }
    [PunRPC]
    public void DamageEnemy(int strength)
    {
        //we should damage enemy, but for now kill it
        Destroy(this.gameObject);
    }
    public void InitializeEnemy(bool isOnline)
    {
        Init();
        enemyIndex = Random.Range(0, 4);
        if (isOnline)
            photonView.RPC("ChangeColor", RpcTarget.All, enemyIndex);
        else
            ChangeColor(enemyIndex);
    }
    [PunRPC]
    void ChangeColor(int _colorIndex)
    {
        if (rend == null)
            Init();
        rend.color = colors[_colorIndex];
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //we're host
            stream.SendNext(enemyIndex);
        }
        else
        {
            //we're client
            this.enemyIndex = (int)stream.ReceiveNext();
        }
        //throw new System.NotImplementedException();
    }
}
