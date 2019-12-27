using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
/// <summary>
/// Local Player Controller, despite the name, controlls the behaviour of Player GO in local environment.
/// It can be used to control the player, who's online, but won't do too much in terms of controlling of online behaviour
/// </summary>
public class LocalPlayerController : MonoBehaviourPun
{
    //public values
    public float moveSpeed; //def move speed
    public float jumpForce; // force of jump
    public Transform attackPivot; //pivot of our attack
    public GameObject projectile; //attack of our guy
    public Text curWeaponText; //ui text ref
    public GameObject staminaIndicator; //gameobject of stamina indicator

    //private values
    Animator playerAnimator; //ref to the animator
    Rigidbody2D rb2d; //ref to rb of the character
    Color[] colors; //stored colors of enemies/attacks
    RectTransform staminaRect; //rectTrans of stamina indicator
    Image staminaImg; //stamina image of stamina indicator
    float curStaminaIndWidth, maxStaminaIndWidth; //current and max width of stamina indicator
    float moveXAxis; //current move axis
    int curAttack; // index of current attack
    bool isJumping, isAttacking; //checks for whether we jumping or attacking
    bool alreadyFlipped; //check if we flipped character
    bool canAttackAnim; //check if we can do attack animation
    float[] staminaRegen; //regen stamina for each attack
    float[] maxStamina; //max stamina for each attack
    float[] stamina; //stamina for each attack
    float[,] staminaCost; //stamina cost for each attack type and category

    void Start()
    {
        Init();
    }
    void Update()
    {
        if (PhotonNetwork.OfflineMode)
        {
            //we're offline, do stuff locally
            moveXAxis = Input.GetAxis("Horizontal");
            PlayerInput();
            StaminaRegen();
            AnimationUpdate();
            GUIUpdate();
        }
        else
        {
            if (photonView.IsMine)
            {
                moveXAxis = Input.GetAxis("Horizontal");
                PlayerInput();
                StaminaRegen();
                AnimationUpdate();
                GUIUpdate();
            }
        }
    }
    void Init()
    {
        alreadyFlipped = true;
        playerAnimator = this.transform.Find("sword_man").GetComponent<Animator>();
        rb2d = this.gameObject.GetComponent<Rigidbody2D>();
        isJumping = isAttacking = false;
        colors = StaticValues.GetColors();
        staminaRect = staminaIndicator.GetComponent<RectTransform>();
        staminaImg = staminaIndicator.GetComponent<Image>();
        maxStaminaIndWidth = staminaRect.sizeDelta.x;
        //hardcode for now
        maxStamina = new float[4];
        stamina = new float[4];
        staminaRegen = new float[4];
        staminaCost = new float[4, 2];
        for (int i = 0; i < 4; i++)
        {
            maxStamina[i] = 100f;
            stamina[i] = maxStamina[i];
            staminaRegen[i] = 10f;
            staminaCost[i, 0] = 15;
            staminaCost[i, 1] = 30f;
        }
    }
    void StaminaRegen()
    {
        for (int i = 0; i < 4; i++)
        {
            stamina[i] += staminaRegen[i] * Time.deltaTime;
            if (stamina[i] > maxStamina[i])
                stamina[i] = maxStamina[i];
        }
    }
    void GUIUpdate()
    {
        curWeaponText.text = "Current Weapon: " + (curAttack + 1).ToString(); //maybe we should updating GUI in game master?
        curWeaponText.color = colors[curAttack];
        staminaImg.color = colors[curAttack];
        curStaminaIndWidth = (stamina[curAttack] / maxStamina[curAttack]) * maxStaminaIndWidth;
        staminaRect.sizeDelta = new Vector2(curStaminaIndWidth, staminaRect.sizeDelta.y);
    }
    void AnimationUpdate()
    {
        if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if (Input.GetButtonUp("Fire1") || Input.GetButtonUp("Fire2"))
            {
                if (canAttackAnim)
                    playerAnimator.Play("Attack");
            }
            else
            {
                if (moveXAxis != 0)
                {
                    playerAnimator.Play("Run");
                }
                else
                {
                    playerAnimator.Play("Idle");
                }
            }
        }
    }
    void PlayerInput()
    {
        if (moveXAxis != 0)
        {
            //we're either moving left or right
            transform.Translate(Vector2.right * moveXAxis * moveSpeed * Time.deltaTime);
            //flipping sprite mechanics
            if (moveXAxis > 0)
            {
                if (alreadyFlipped)
                    Flip(false);
                alreadyFlipped = false;
            }
            else if (moveXAxis < 0)
            {
                if (!alreadyFlipped)
                    Flip(true);
                alreadyFlipped = true;
            }
        }
        //TODO: Check if we can jump because we're jumping every time we press the button
        if (Input.GetButtonUp("Jump"))
        {
            rb2d.velocity = new Vector2(0, 0);
            rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        //weapon change logic because it's too long and shitty
        WeaponChange();
        //normal attack
        if (Input.GetButtonUp("Fire1"))
        {
            Attack(false);
        }
        //ranged attack
        if (Input.GetButtonUp("Fire2"))
        {
            Attack(true);
        }
    }
    void WeaponChange()
    {
        //change weapon via alphanumeric keypad
        if (Input.GetKeyUp(KeyCode.Alpha1))
            curAttack = 0;
        if (Input.GetKeyUp(KeyCode.Alpha2))
            curAttack = 1;
        if (Input.GetKeyUp(KeyCode.Alpha3))
            curAttack = 2;
        if (Input.GetKeyUp(KeyCode.Alpha4))
            curAttack = 3;
        //change weapon via mouse scroll
        if (Input.GetAxis("ChangeWeapon") > 0)
        {
            curAttack++;
        }
        if (Input.GetAxis("ChangeWeapon") < 0)
        {
            curAttack--;
        }
        //checker if we didn't get too far with weapon change
        if (curAttack > 3)
            curAttack = 0;
        if (curAttack < 0)
            curAttack = 3;
    }
    void Flip(bool bLeft)
    {
        transform.localScale = new Vector3(bLeft ? 1 : -1, 1, 1);
    }
    void Attack(bool isProjectile)
    {
        if (!isProjectile)
        {
            if (stamina[curAttack] >= staminaCost[curAttack, 0])
            {
                if (PhotonNetwork.OfflineMode)
                    FireLocalProjectile(isProjectile);
                else
                    FireOnlineProjectile(isProjectile);
                stamina[curAttack] -= staminaCost[curAttack, 0];
                canAttackAnim = true;
            }
            else
            {
                canAttackAnim = false;
            }
        }
        else
        {
            if (stamina[curAttack] >= staminaCost[curAttack, 1])
            {
                if (PhotonNetwork.OfflineMode)
                    FireLocalProjectile(isProjectile);
                else
                    FireOnlineProjectile(isProjectile);
                stamina[curAttack] -= staminaCost[curAttack, 1];
                canAttackAnim = true;
            }
            else
            {
                canAttackAnim = false;
            }
        }

    }
    void FireOnlineProjectile(bool isProjectile)
    {
        if (!isProjectile)
        {
            GameObject tmp = PhotonNetwork.Instantiate(projectile.name, attackPivot.position, Quaternion.identity);
            tmp.GetComponent<ProjectileController>().CreateProjectile(curAttack, isProjectile, 1, Vector3.zero, 0.2f,true);
        }
        else
        {
            GameObject tmp = PhotonNetwork.Instantiate(projectile.name, attackPivot.position, Quaternion.identity);
            Vector3 moveDir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - attackPivot.position);
            moveDir.z = 0;
            moveDir.Normalize();
            tmp.GetComponent<ProjectileController>().CreateProjectile(curAttack, isProjectile, 1, moveDir, 0, true); //radius doesnt matter if we're not using static projectile
        }
    }
    void FireLocalProjectile(bool isProjectile)
    {
        if (!isProjectile)
        {
            GameObject tmp = GameObject.Instantiate(projectile, attackPivot.position, Quaternion.identity);
            tmp.GetComponent<ProjectileController>().CreateProjectile(curAttack, isProjectile, 1, Vector3.zero, 0.2f, false);
        }
        else
        {
            GameObject tmp = GameObject.Instantiate(projectile, attackPivot.position, Quaternion.identity);
            Vector3 moveDir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - attackPivot.position);
            moveDir.z = 0;
            moveDir.Normalize();
            tmp.GetComponent<ProjectileController>().CreateProjectile(curAttack, isProjectile, 1, moveDir, 0, false); //radius doesnt matter if we're not using static projectile
        }

    }
}
