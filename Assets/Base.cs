using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Base : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    private GameObject alertHolder;
    public GameObject alertPrefab;
    GameObject myAlert;
    public GameObject flag;
    public GameObject[] walls;
    public GameObject LeftBase = null;
    public GameObject RightBase = null;
    GameManager gm;
    GameObject player;
    public bool playerOwned = false;
    float RespawnSpeed = 10f;
    float OldRespawnSpeed;
    float timer;
    public float repairDelay = 10f;
    float attackedTimer = 0f;
    // Use this for initialization
    void Start()
    {
        alertHolder = GameObject.FindGameObjectWithTag("AlertHolder");
        myAlert = Instantiate(alertPrefab);
        myAlert.transform.SetParent(alertHolder.transform);
        myAlert.transform.localScale = new Vector3(1, 1, 1);
        player = GameObject.FindGameObjectWithTag("Player");
        timer = RespawnSpeed;
        OldRespawnSpeed = 10f;
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //gm.AddBase(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerOwned && timer <= 0)
        {
            SpawnEnemy();
            timer = RespawnSpeed + Random.Range(0f, RespawnSpeed);
        }
        else
        {
            timer -= Time.deltaTime;
        }
        if (attackedTimer <= 0)
        {
            //TODO: Optimize this shit jeezus
            foreach (GameObject wall in walls)
            {
                wall.GetComponent<WallHealth>().shouldRepair = true;
            }

            myAlert.SetActive(false);
        }
        else
        {
            attackedTimer -= Time.deltaTime;
            if (playerOwned && Mathf.Abs(player.transform.position.x - transform.position.x) > 30)
            {
                myAlert.SetActive(true);
                UpdateAlertText();
            }
            else
            {
                myAlert.SetActive(false);
            }
        }
    }
    void UpdateAlertText()
    {
        myAlert.GetComponent<Text>().text = "Base under attack! " + Mathf.Abs(player.transform.position.x - transform.position.x).ToString("F1") + "\'";
    }
    public void TakeDamage()
    {
        attackedTimer = repairDelay;
        foreach (GameObject wall in walls)
        {
            wall.GetComponent<WallHealth>().shouldRepair = false;
        }
    }
    public void RespawnSpeedOverride(bool shouldOverride, float newSpeed)
    {
        if (shouldOverride)
        {
            timer -= (RespawnSpeed - newSpeed);
            RespawnSpeed = newSpeed;
            
        }
        else
        {
            if (gm.Countering || gm.Night)
            {
                return;
            }
            RespawnSpeed = OldRespawnSpeed;
        }
    }
    public void RespawnSpeedOverride(bool shouldOverride)
    {
        RespawnSpeedOverride(shouldOverride, 0f);
        if (shouldOverride == true)
        {
            Debug.LogError("No override time set!");
        }
    }
    void SpawnEnemy()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            return;
        }
        if (!gm)
        {
            Start();
        }
        int enemyNum = Random.Range(0, enemyPrefabs.Length);
        GameObject enemy = (GameObject)Instantiate(enemyPrefabs[enemyNum], new Vector3(gameObject.transform.position.x + 4, 1.3f), Quaternion.identity);
        enemy.GetComponent<EnemyMove>().damageAmount = Mathf.RoundToInt(gm.eDamage);
        enemy.GetComponent<EnemyMove>().speedModifier += gm.eSpeedModBonus;
        enemy.GetComponent<EnemyHealth>().newMax(Mathf.RoundToInt(gm.eHealth));
        if (OldRespawnSpeed != RespawnSpeed)
        {
            enemy.GetComponent<EnemyMove>().SetSprinting(true);
        }
    }

    public void Upgrade()
    {
        Debug.Log("Upgraded");
        foreach (GameObject wall in walls)
        {
            wall.SetActive(true);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.GetComponent<Health>().Dead)
            {
                return;//dead people cant capture bases
            }
            CaptureBase(false);//decapture the base
            return;
        }
        if (EnemiesNearby())
        {
            return;//we need to clear the area
        }
        if (playerOwned)
        {
            return;
            //no need to capture base
        }
        //capture base
        CaptureBase(true);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !playerOwned)
        {
            if (!EnemiesNearby())
            {
                CaptureBase(true);
            }
            else
            {
                return;
            }
        }
        
        //TODO: Check if player is inside
    }

    bool EnemiesNearby()
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in go)//TODO: Holy fuck optimize this shit whose idea was this anyways fuck me shit
        {
            if (!enemy.GetComponent<Health>().Dead && Mathf.Abs(enemy.transform.position.x - gameObject.transform.position.x) < 30)
            {
                return true;
            }
        }
        return false;
    }

    void CaptureBase(bool playerCaptured)
    {
        foreach (GameObject wall in walls)
        {
            wall.GetComponent<BoxCollider2D>().isTrigger = playerCaptured;
            wall.SetActive(true);
            if (wall.gameObject.tag == "Buildable")
            {
                wall.SetActive(playerCaptured);
                //Debug.Log("We are making the walls triggers because we have captured the base");
                
                //wall.GetComponent<Health>().SetHealth();
            }

        }
        playerOwned = playerCaptured;
        if (playerCaptured)
        {
            flag.GetComponent<SpriteRenderer>().color = Color.green;
            gm.CounterAttack(true, gameObject);
        }
        else
        {
            flag.GetComponent<SpriteRenderer>().color = Color.red;
            myAlert.SetActive(false);
        }
    }
}
