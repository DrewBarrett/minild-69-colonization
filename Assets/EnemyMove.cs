using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMove : MonoBehaviour
{
    public GameObject rayCastStart;
    public int damageAmount = 3;
    protected GameObject player;
    //GameObject attackTarget;
    //public float attackcooldown;

    public float speedModifier = 3f;
    bool sprinting = false;
    float sprintSpeedModifier = 6f;
    float cooldownTimer;
    GameManager gm;
    
    
    
    // Use this for initialization
    virtual protected void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        cooldownTimer = 0;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    virtual protected void FixedUpdate()
    {
        if (GetComponent<Health>().Dead)
        {
            return;
        }
    }
    protected bool CheckRayForTargets(RaycastHit2D[] hits)
    {
        foreach (RaycastHit2D hit in hits)
        {
            //Debug.Log(hit.ToString());
            if (hit.transform.gameObject.tag == "Player" || (hit.transform.gameObject.tag == "Wall" && (!hit.transform.gameObject.GetComponent<Health>().Dead && !hit.transform.gameObject.GetComponentInParent<Base>().playerOwned == false)))
            {
                return true;
                
            }
        }
        return false;
    }
    void Update()
    {
        if (GetComponent<Health>().Dead)
        {
            return;
        }
        /*if (attackTarget)
        {
            Attack();
        }*/
    }
    
  
    public void SetSprinting(bool sprint)
    {
        if (sprint == sprinting)
        {
            return;
        }
        else
        {
            if (!sprint)
            {
                if (gm.Night || gm.Countering)
                {
                    return;
                }
            }
        }
        sprinting = sprint;
        if (sprinting)
        {
            speedModifier *= 2;
        }
        else
        {
            speedModifier *= .5f;
        }
    }

    virtual protected void CheckCollision(Collider2D collision)
    {
        if (GetComponent<Health>().Dead)
        {
            return;
        }
        if (collision.gameObject.tag == "Player" && !GetComponent<Health>().Dead)
        {

            //GetComponent<Rigidbody2D>().velocity = ( transform.position - collision.transform.position);
            player.GetComponent<player>().BeAttacked(gameObject);


            return;
        }
        if (collision.gameObject.tag == "Wall")
        {
            if (collision.GetComponent<Health>().Dead || collision.GetComponentInParent<Base>().playerOwned == false)
            {
                return;
            }
            CollideWithWall(collision);
        }
        if (collision.gameObject.tag == "Floor")
        {
            CollideWithFloor(collision);

        }


    }
    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {
        CheckCollision(collision);
        
    }
    virtual protected void CollideWithFloor(Collider2D collision)
    {
        
    }
    virtual protected void CollideWithWall(Collider2D collision)
    {
        collision.gameObject.GetComponent<Health>().TakeDamage(damageAmount);
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (GetComponent<Health>().Dead)
            return;
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        Debug.LogError("collision exit");
        /*if (GetComponent<Health>().Dead)
        {
            return;
        }
        setShouldMove(true);*/
    }
}
