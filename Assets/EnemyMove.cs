using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMove : MonoBehaviour
{
    public GameObject rayCastStart;
    public int damageAmount = 3;
    GameObject player;
    //GameObject attackTarget;
    bool shouldMove;
    //public float attackcooldown;
    public float jumpCooldown = 2f;
    float jumpTimer;
    public float speedModifier = 3f;
    bool sprinting = false;
    float sprintSpeedModifier = 6f;
    float cooldownTimer;
    List<Collider2D> currentTriggers;
    GameManager gm;
    bool isJumping;
    bool shouldReverse = false;
    float reverseDistance = 4f;
    float reverseRemaining;
    // Use this for initialization
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        currentTriggers = new List<Collider2D>();
        setShouldMove(true);
        cooldownTimer = 0;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        float val = Mathf.Sign(transform.position.x - player.transform.position.x);
        if (GetComponent<Health>().Dead)
        {
            return;
        }
        if (!shouldMove)
        {
            if (gameObject.GetComponent<Rigidbody2D>().isKinematic)
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
            else
            {
                gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
            }
            return;
        }
        if (shouldReverse)
        {
            if (reverseRemaining <= 0 && jumpTimer <= 0)
            {
                
                shouldReverse = false;
            }
            else
            {
                jumpTimer -= Time.fixedDeltaTime;
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(val * speedModifier * .2f, 0);
                reverseRemaining -= Time.fixedDeltaTime;
            }
            return;
        }
        if (!isJumping)
        {
            
            //transform.Translate(new Vector3(.1f * val * -1, 0));
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(val * -1 * speedModifier, 0);
        }
        if (isJumping || jumpTimer > 0)
        {
            jumpTimer -= Time.fixedDeltaTime;

            return;
        }
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayCastStart.transform.position, new Vector2(val * -1, 0), 5f);
        foreach (RaycastHit2D hit in hits)
        {
            //Debug.Log(hit.ToString());
            if (hit.transform.gameObject.tag == "Player" || (hit.transform.gameObject.tag == "Wall" && (!hit.transform.gameObject.GetComponent<Health>().Dead && !hit.transform.gameObject.GetComponentInParent<Base>().playerOwned == false)))
            {
                
                Jump(val * -1);
                break;
            }
        }
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
    void SetShouldReverse()
    {
        shouldReverse = true;
        reverseRemaining = reverseDistance;
    }
    /*void Attack()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }
        cooldownTimer = attackcooldown;
        if (attackTarget == null || attackTarget.GetComponent<Health>().Dead)
        {
            //setShouldMove(true);
            return;
        }
        attackTarget.GetComponent<Health>().TakeDamage(5);

    }*/
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

    void Jump(float dir)
    {
        jumpTimer = jumpCooldown;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        isJumping = true;
        GetComponent<Rigidbody2D>().isKinematic = false;
        GetComponent<Rigidbody2D>().AddForce(new Vector2(7 * dir, 3), ForceMode2D.Impulse);
    }
    public void OnTriggerEnter2D(Collider2D collision)
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

        /*if (collision.gameObject.tag == "Enemy" && collision.GetComponent<EnemyMove>().shouldMove == false)
        {
            currentTriggers.Add(collision);
            setShouldMove(false);
            return;
        }*/
        if (collision.gameObject.tag == "Wall")
        {
            if (collision.GetComponent<Health>().Dead || collision.GetComponentInParent<Base>().playerOwned == false)
            {

                return;
            }
            currentTriggers.Add(collision);
            setShouldMove(false);
            //attackTarget = collision.gameObject;
            collision.gameObject.GetComponent<Health>().TakeDamage(damageAmount);
            SetShouldReverse();
        }
        if (collision.gameObject.tag == "Floor")
        {

            isJumping = false;
            GetComponent<Rigidbody2D>().isKinematic = true;
            shouldMove = true;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (GetComponent<Health>().Dead)
            return;
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("collision exit");
        if (GetComponent<Health>().Dead)
        {
            return;
        }
        setShouldMove(true);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (GetComponent<Health>().Dead)
        {
            return;
        }
        currentTriggers.Remove(collision);
        int surroundNoMoveCount = 0;
        foreach (Collider2D col in currentTriggers)
        {
            if (col.gameObject.tag == "Wall" && col.GetComponent<Health>().Dead == false)
            {
                return;
            }
            if (col.gameObject.tag == "Enemy" && col.GetComponent<Health>().Dead == false && col.GetComponent<EnemyMove>().shouldMove == false)
            {
                surroundNoMoveCount++;
            }
        }
        if (surroundNoMoveCount >= 2)
        {
            Debug.Log("We are colliding with 2 non moving and alive enemies so we will not start moving under any circumstance");
            return;
        }
        /*if (collision.gameObject.tag == "Enemy" && (collision.GetComponent<EnemyMove>().shouldMove || collision.GetComponent<EnemyHealth>().Dead))
        {
            Debug.Log("We just exited collision with a dead or now moving enemy so we should move too");
            setShouldMove(true);
            return;
        }*/

    }

    /*public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall" && collision.GetComponent<Health>().Dead == true && shouldMove == false && attackTarget != null)
        {
            attackTarget = null;
            setShouldMove(true);
            Debug.Log("The wall is dead so we will move now");
        }
    }*/
}
