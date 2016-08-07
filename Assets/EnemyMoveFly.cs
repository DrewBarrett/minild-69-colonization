using UnityEngine;
using System.Collections;

public class EnemyMoveFly : EnemyMove {
    bool lunging = false;
    bool recovering = false;
    bool flipped = false;
    bool bounceback = false;
    float bounceTimer = 0;
    float bounceTime = .3f;
    float jumpTimer = 0;
    float jumpCooldown = .7f;
    float targetHeight = 3.5f;

    protected override void Start()
    {
        base.Start();
        Vector3 newpos = transform.position;
        newpos.y = targetHeight;
        transform.position = newpos;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (GetComponent<Health>().Dead)
        {
            return;
        }
        if (bounceback)
        {
            
            if (bounceTimer <= 0)
            {
                bounceback = false;
                recovering = true;
                Jump();
            }
            else
            {
                bounceTimer -= Time.fixedDeltaTime;
            }
        }
        float val = Mathf.Sign(transform.position.x - player.transform.position.x);
        if (recovering)
        {
            
            if (jumpTimer <= 0)
            {
                Jump();
            }
            else
            {
                jumpTimer -= Time.fixedDeltaTime;
            }
            if (transform.position.y >= targetHeight)
            {
                recovering = false;
                GetComponent<Rigidbody2D>().isKinematic = true;
            }
        }
        if (!lunging && !recovering && !bounceback)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(val * -1 * speedModifier, gameObject.GetComponent<Rigidbody2D>().velocity.y);
            if ((!flipped && GetComponent<Rigidbody2D>().velocity.x < 0) || (flipped && GetComponent<Rigidbody2D>().velocity.x > 0))
            {
                Flip();
            }
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 5f, Vector2.zero);
            RaycastHit2D target = CheckRayForTargets(hits);
            if (target.collider)
            {
                //if the target is real
                //attack target
                Lunge(target.point);
            }
        }

    }
    void Flip()
    {
        flipped = !flipped;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }
    void Lunge(Vector2 t)
    {
        lunging = true;
        GetComponent<Rigidbody2D>().velocity = new Vector2(t.x - transform.position.x, t.y - transform.position.y) * speedModifier;
    }
    void Jump()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x / 2, 0);
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 5.5f), ForceMode2D.Impulse);
        jumpTimer = jumpCooldown;
    }
    protected override void CollideWithFloor(Collider2D collision)
    {
        base.CollideWithFloor(collision);
        if (lunging)
        {
            lunging = false;
            recovering = true;
            GetComponent<Rigidbody2D>().isKinematic = false;
            GetComponent<Rigidbody2D>().velocity /= 3;//slow the fuck down
            Jump();
        }
        else
        {
            Debug.LogError("wtf just happened m8");
        }
    }
    protected override void CollideWithWall(Collider2D collision)
    {
        base.CollideWithWall(collision);
        if (lunging)
        {
            bounceTimer = bounceTime;
            lunging = false;
            bounceback = true;
            Vector2 newvel = GetComponent<Rigidbody2D>().velocity;
            newvel.x *= -1;
            //Debug.Log("Old velocity: " + GetComponent<Rigidbody2D>().velocity.ToString() + "New Velocity: " + newvel.ToString());
            GetComponent<Rigidbody2D>().velocity = newvel;
            GetComponent<Rigidbody2D>().isKinematic = false;
        }
        else
        {
            Debug.LogError("Thats not supposed to happen");
        }
    }
}
