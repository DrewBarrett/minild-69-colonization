using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {
    float shootCooldown = 5f;
    int damage = 30;
    float shootTimer;
    float range = 10f;
    public GameObject shootPoint;
    public AudioClip gunshot;
    float m_GunFlareVisibleSeconds = .07f;
    // Use this for initialization
    void Start () {
        shootTimer = 0f;
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<TurretHealth>().upgradeLevel > 0)
        {
            //if turret is built...
            shootTimer -= Time.deltaTime;
            float mod = GetComponent<TurretHealth>().upgradeLevel * .2f;
            RaycastHit2D[] hitsRight = Physics2D.RaycastAll(new Vector2(transform.position.x, 1), transform.right, range);
            RaycastHit2D[] hitsLeft = Physics2D.RaycastAll(new Vector2(transform.position.x, 1), transform.right * -1, range);
            GameObject target = null;
            target = FindClosestEnemy(hitsRight, target);
            target = FindClosestEnemy(hitsLeft, target);
            if (target)
            {
                var dir = target.transform.position - GetComponentInChildren<SpriteRenderer>().gameObject.transform.position;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                angle -= 90;
                GetComponentInChildren<SpriteRenderer>().gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                if (shootTimer <= 0)
                {
                    Shoot(target);
                    shootTimer = shootCooldown;
                }
            }
            
        }
	}
    void Shoot(GameObject target)
    {
        GetComponent<AudioSource>().PlayOneShot(gunshot);
        Vector3 startvector = shootPoint.transform.position;
        //startvector.x *= bulletexit.transform.forward.x;
        GetComponent<LineRenderer>().SetPosition(0, startvector);
        target.GetComponent<Health>().TakeDamage(damage);
        GetComponent<LineRenderer>().SetPosition(1, target.transform.position);
        StartCoroutine(UpdateLineRenderer());
    }
    private IEnumerator UpdateLineRenderer()
    {
        GetComponent<LineRenderer>().enabled = true;
        // Create a timer.
        float timer = 0f;

        // While that timer has not yet reached the length of time that the gun effects should be visible for...
        while (timer < m_GunFlareVisibleSeconds)
        {

            // Wait for the next frame.
            yield return null;

            // Increment the timer by the amount of time waited.
            timer += Time.deltaTime;
        }
        GetComponent<LineRenderer>().enabled = false;
    }
    GameObject FindClosestEnemy(RaycastHit2D[] hits, GameObject tar)
    {
        GameObject target = tar;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.gameObject.tag == "Enemy" && hit.transform.gameObject.GetComponent<Health>().Dead == false)
            {
                if (target == null)
                {
                    target = hit.transform.gameObject;
                }
                else
                {
                    if (Mathf.Abs(target.transform.position.x - gameObject.transform.position.x) > Mathf.Abs(hit.transform.position.x - gameObject.transform.position.x))
                    {
                        target = hit.transform.gameObject;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        return target;
    }
}
