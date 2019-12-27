using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OnlinePlayerController : MonoBehaviourPun, IPunObservable
{
    bool hasDisabledComponentsForMultiplayer;
    Rigidbody2D rb2d;
    CapsuleCollider2D cap2d;
    Vector2 prevFramePos;
    Animator playerAnimator;

    void Start()
    {
        //check if we're online
        if (PhotonNetwork.OfflineMode)
            this.enabled = false;
        rb2d = GetComponent<Rigidbody2D>();
        cap2d = GetComponent<CapsuleCollider2D>();
        playerAnimator = this.transform.Find("sword_man").GetComponent<Animator>();

    }
    void Update()
    {
        prevFramePos = transform.position;
    }
    private void LateUpdate()
    {
        if (!photonView.IsMine)
        {
            //animation update for other player
            OnlineAnimationUpdate();
            //disable components for maintaining better online experience
            if (!hasDisabledComponentsForMultiplayer)
            {
                DisableOnlineStuff();
            }
        }

    }
    private void DisableOnlineStuff()
    {
        rb2d.isKinematic = true;
        rb2d.simulated = false;
        cap2d.enabled = false;
        hasDisabledComponentsForMultiplayer = true;
    }
    private void OnlineAnimationUpdate()
    {
        if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            Vector2 curPos = transform.position;

            if (!StaticValues.AreVectorsEqual(curPos, prevFramePos))
            {
                playerAnimator.Play("Run");
            }
            else
            {
                playerAnimator.Play("Idle");
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
