using UnityEngine;
using System.Collections;

public class EnemyMoveWalk : EnemyMove {
    bool shouldReverse = false;
    float reverseDistance = 4f;
    float reverseRemaining;
    float jumpCooldown = 2f;
    float jumpTimer;
    bool isJumping;
    bool shouldMove = true;
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (GetComponent<Health>().Dead)
        {
            return;
        }
        float val = Mathf.Sign(transform.position.x - player.transform.position.x);
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
        if (CheckRayForTargets(hits).collider)
        {
            Jump(val * -1);
        }
    }
    protected override void CheckCollision(Collider2D collision)
    {
        base.CheckCollision(collision);
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        //CheckCollision(collision);
    }
    void SetShouldReverse()
    {
        shouldReverse = true;
        reverseRemaining = reverseDistance;
    }
    void Jump(float dir)
    {
        jumpTimer = jumpCooldown;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        isJumping = true;
        GetComponent<Rigidbody2D>().isKinematic = false;
        GetComponent<Rigidbody2D>().AddForce(new Vector2(7 * dir, 3), ForceMode2D.Impulse);
    }

    protected override void CollideWithWall(Collider2D collision)
    {
        base.CollideWithWall(collision);
        shouldMove = false;
        //attackTarget = collision.gameObject;
        
        SetShouldReverse();
    }
    protected override void CollideWithFloor(Collider2D collision)
    {
        base.CollideWithFloor(collision);
        
        isJumping = false;
        GetComponent<Rigidbody2D>().isKinematic = true;
        shouldMove = true;
    }
}
