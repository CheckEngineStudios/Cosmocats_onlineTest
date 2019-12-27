using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileController : MonoBehaviourPun, IPunObservable
{
    SpriteRenderer rend; //local ref to sprite renderer
    public int attackIndex; //Type of the attack. we have 3 elements, so 0,1,2 are accepted
    public bool isProjectile; //checks whether we have projectile or static attack
    public int attackStrength; //how many damage we're dealing
    Color[] projectileColors; //projectile [and enemy] colors
    bool shouldMove; //check if we should move the projectile
    public float moveSpeed; //speed of the projectile
    Vector3 moveDirection; //dir of the projectile
    CircleCollider2D col2d; //ref to circle col2d of this projectile
    bool isOnline;

    void Update()
    {
        if (shouldMove)
        {
            transform.position = transform.position + moveDirection * moveSpeed * Time.deltaTime;
        }
    }
    void Init()
    {
        rend = this.gameObject.GetComponent<SpriteRenderer>();
        shouldMove = false;
        isProjectile = false; //wtf 
        projectileColors = StaticValues.GetColors();
        col2d = this.gameObject.GetComponent<CircleCollider2D>();
    }
    IEnumerator DieAfterAWhile(float time)
    {
        yield return new WaitForSeconds(time);
        DestroyProjectile();
    }
    
    public void CreateProjectile(int _attackIndex,bool _isProjectile, int _attackStrength, 
        Vector3 _moveDirection, float radius, bool _isOnline)
    {
        isOnline = _isOnline;
        if (_isOnline)
            photonView.RPC("Initialize", RpcTarget.All, _attackIndex, _isProjectile);
        else
            Initialize(_attackIndex, _isProjectile);
        attackStrength = _attackStrength;
        if (!isProjectile)
        {
            col2d.radius = radius;
        }
        else
        {
            moveDirection = _moveDirection;
            shouldMove = true;
        }
    }
    [PunRPC]
    void Initialize(int _attackIndex, bool _isProjectile)
    {
        if (rend == null)
            Init();
        attackIndex = _attackIndex;
        rend.color = projectileColors[attackIndex];
        isProjectile = _isProjectile;
        if(!isProjectile)
            StartCoroutine(DieAfterAWhile(1));
        else
            StartCoroutine(DieAfterAWhile(30));

    }
    void DestroyProjectile()
    {
        Destroy(this.gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            //we hit enemy
            EnemyController tmpcont = collision.gameObject.GetComponent<EnemyController>();
            if(tmpcont.enemyIndex == attackIndex)
            {
                //we hit correct one
                if (isOnline)
                    tmpcont.photonView.RPC("DamageEnemy", Photon.Pun.RpcTarget.All, attackStrength);
                else
                    tmpcont.DamageEnemy(attackStrength);
                DestroyProjectile();
            }
            else
            {
                //we didnt do fuck-all
                Physics2D.IgnoreCollision(collision.collider, col2d);
            }
        }
        if (collision.collider.tag == "Ground")
        {
            //we hit ground
            DestroyProjectile();
        }
        //probably need more checks before we ignore collisions entirely
        else
        {
            Physics2D.IgnoreCollision(collision.collider, col2d);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //throw new System.NotImplementedException();
    }
}
