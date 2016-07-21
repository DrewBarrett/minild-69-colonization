using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class player : MonoBehaviour
{
    public GameObject bulletexit;
    public AudioClip gunshot;
    public Text deadtext;
    public int distance;
    bool dead = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!dead && Input.GetMouseButtonDown(0))
        {
            Fire();
        }
        else
        {
            bulletexit.GetComponent<LineRenderer>().enabled = false;
        }
    }
    public void Die()
    {
        dead = true;
        Debug.Log("Player died");
        deadtext.gameObject.SetActive(true);

        gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Pickup")
        {
            GameObject.Destroy(collision.gameObject.transform.parent.gameObject);//now thats a mouthfull
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().AddCoins(1);
        }
    }

    void Fire()
    {
        GetComponent<AudioSource>().PlayOneShot(gunshot);
        RaycastHit2D[] rayhits = Physics2D.RaycastAll(bulletexit.transform.position, bulletexit.transform.right * Mathf.Sign(transform.localScale.x), distance);
        RaycastHit2D rayhit = new RaycastHit2D();
        foreach (RaycastHit2D hit in rayhits)
        {
            if ((hit.collider.gameObject.tag == "Enemy" && !hit.collider.GetComponent<Health>().dead) || hit.collider.isTrigger == false)
            {
                rayhit = hit;
                break;
            }
        }
        bulletexit.GetComponent<LineRenderer>().enabled = true;
        Vector3 startvector = bulletexit.transform.position;
        //startvector.x *= bulletexit.transform.forward.x;
        bulletexit.GetComponent<LineRenderer>().SetPosition(0, startvector);
        float dist;
        if (rayhit.collider && rayhit.collider.gameObject.GetComponent<Health>() != null)
        {
            rayhit.collider.gameObject.GetComponent<Health>().TakeDamage(10);
        }
        if (rayhit.distance > 0)
        {
            dist = rayhit.distance;
        }
        else
        {
            dist = distance;
        }
        bulletexit.GetComponent<LineRenderer>().SetPosition(1, bulletexit.transform.position + new Vector3(dist * Mathf.Sign(transform.localScale.x), 0));
    }
}
