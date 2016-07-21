using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMove : MonoBehaviour
{
    GameObject player;
    GameObject attackTarget;
    bool shouldMove;
    public float attackcooldown;
    public float speedModifier = 3f;
    float cooldownTimer;
    List<Collider2D> currentTriggers;
    // Use this for initialization
    void Start()
    {
        currentTriggers = new List<Collider2D>();
        setShouldMove(true);
        cooldownTimer = 0;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        if (GetComponent<Health>().dead)
        {
            return;
        }
        if (!shouldMove)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            return;
        }
        if (GetComponent<Health>().dead)
        {
            return;
        }
        float val = Mathf.Sign(transform.position.x - player.transform.position.x);
        //transform.Translate(new Vector3(.1f * val * -1, 0));
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(val * -1 * speedModifier, 0);
    }

    void Update()
    {
        if (GetComponent<Health>().dead)
        {
            return;
        }
        if (attackTarget)
        {
            Attack();
        }
    }

    void Attack()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }
        cooldownTimer = attackcooldown;
        if (attackTarget == null || attackTarget.GetComponent<Health>().dead)
        {
            
            //setShouldMove(true);
            return;
        }
        attackTarget.GetComponent<Health>().TakeDamage(5);

    }
    void setShouldMove(bool should)
    {
        shouldMove = should;
        //GetComponent<Rigidbody2D>().isKinematic = should;
        /*if (should)
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }*/

    }
    public void OnCollisionEnter2D(Collision2D collision)
    {


    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !GetComponent<Health>().dead)
        {
            player.GetComponent<player>().Die();
            return;
        }
        
        if (collision.gameObject.tag == "Enemy" && collision.GetComponent<EnemyMove>().shouldMove == false)
        {
            currentTriggers.Add(collision);
            setShouldMove(false);
            return;
        }
        if (collision.gameObject.tag == "Wall")
        {
            if (collision.GetComponent<Health>().dead || collision.GetComponentInParent<Base>().playerOwned == false)
            {
                
                return;
            }
            currentTriggers.Add(collision);
            setShouldMove(false);
            attackTarget = collision.gameObject;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("collision exit");
        if (GetComponent<Health>().dead)
        {
            return;
        }
        setShouldMove(true);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (GetComponent<Health>().dead)
        {
            return;
        }
        currentTriggers.Remove(collision);
        int surroundNoMoveCount = 0;
        foreach (Collider2D col in currentTriggers)
        {
            if (col.gameObject.tag == "Wall" && col.GetComponent<Health>().dead == false)
            {
                return;
            }
            if (col.gameObject.tag == "Enemy" && col.GetComponent<Health>().dead == false && col.GetComponent<EnemyMove>().shouldMove == false)
            {
                surroundNoMoveCount++;
            }
        }
        if (surroundNoMoveCount >=2)
        {
            Debug.Log("We are colliding with 2 non moving and alive enemies so we will not start moving under any circumstance");
            return;
        }
        if (collision.gameObject.tag == "Enemy" && (collision.GetComponent<EnemyMove>().shouldMove || collision.GetComponent<EnemyHealth>().dead))
        {
            Debug.Log("We just exited collision with a dead or now moving enemy so we should move too");
            setShouldMove(true);
            return;
        }

    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall" && collision.GetComponent<Health>().dead == true && shouldMove == false && attackTarget != null)
        {
            attackTarget = null;
            setShouldMove(true);
            Debug.Log("The wall is dead so we will move now");
        }
    }
}
