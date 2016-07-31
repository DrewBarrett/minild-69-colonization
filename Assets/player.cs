using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class player : MonoBehaviour
{
    public GameObject bulletexit;
    public AudioClip gunshot;
    public GameObject staminaText;
    public Text deadtext;
    public int distance;
    bool dead = false;
    public float fireCooldown = .1f;
    float fireTimer;
    float m_GunFlareVisibleSeconds = .07f;
    float maxStamina = 100f;
    float staminaRechargeDelay = 3f;
    float staminaRechargeTimer;
    float staminaRechargeRate = 2f;
    bool staminaRecharge = false;
    internal bool staminaInUse = false;
    float stamina;
    GameObject target = null;
    // Use this for initialization
    void Start()
    {
        stamina = 0;
        staminaRechargeTimer = staminaRechargeDelay;
        UpdateStaminaText();
    }

    public void BuildTarget()
    {
        if (GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().SpendMoney(target.GetComponent<Health>().BuildCost()))
        {
            target.GetComponent<Health>().Build();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead && fireTimer <= 0 && Input.GetButtonDown("Fire1"))
        {
            Fire();
            
        }
        else
        {
            //bulletexit.GetComponent<LineRenderer>().enabled = false;
            fireTimer -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump"))
        {
            if (target)
            {
                BuildTarget();
            }
        }
        if (!staminaRecharge && !staminaInUse)
        {
            staminaRechargeTimer -= Time.deltaTime;
        }
        if (staminaRechargeTimer <= 0)
        {
            staminaRecharge = true;
        }
        if (stamina < maxStamina && staminaRecharge)
        {
            stamina += staminaRechargeRate * Time.deltaTime;
            if (stamina > maxStamina)
            {
                stamina = maxStamina;
            }
            UpdateStaminaText();
        }
        
    }

    public bool SpendStamina(float v)
    {
        if (v <= stamina)
        {
            staminaInUse = true;
            stamina -= v;
            staminaRecharge = false;
            staminaRechargeTimer = staminaRechargeDelay;
            UpdateStaminaText();
            return true;
        }
        return false;
    }

    void UpdateStaminaText()
    {
        //TODO: color code
        staminaText.GetComponent<Text>().text = "Stamina: " + stamina.ToString("F0");
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
        if (collision.gameObject.tag == "Buildable")
        {
            target = collision.gameObject;
            target.GetComponentInChildren<Canvas>(true).gameObject.SetActive(true);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Buildable")
        {
            //Debug.Log("Buildable Object Exited");
            if (target != null)
            {
                target.GetComponentInChildren<Canvas>(true).gameObject.SetActive(false);
            }
            target = null;

        }
    }

    void Fire()
    {
        fireTimer = fireCooldown;
        GetComponent<AudioSource>().PlayOneShot(gunshot);
        RaycastHit2D[] rayhits = Physics2D.RaycastAll(bulletexit.transform.position, bulletexit.transform.right * Mathf.Sign(transform.localScale.x), distance);
        RaycastHit2D rayhit = new RaycastHit2D();
        foreach (RaycastHit2D hit in rayhits)
        {
            if ((hit.collider.gameObject.tag == "Enemy" && !hit.collider.GetComponent<Health>().Dead) || hit.collider.isTrigger == false)
            {
                rayhit = hit;
                break;
            }
        }
        
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
        StartCoroutine(UpdateLineRenderer());
    }
    private IEnumerator UpdateLineRenderer()
    {
        bulletexit.GetComponent<LineRenderer>().enabled = true;
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
        bulletexit.GetComponent<LineRenderer>().enabled = false;
    }
}
