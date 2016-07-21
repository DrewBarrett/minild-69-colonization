using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Base : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject flag;
    public GameObject[] walls;
    public Button upgradeBtn;
    GameManager gm;
    public bool playerOwned = false;
    float RespawnSpeed = 5f;
    float OldRespawnSpeed;
    float timer;
    // Use this for initialization
    void Start()
    {
        timer = RespawnSpeed;
        OldRespawnSpeed = RespawnSpeed;
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //gm.AddBase(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerOwned && timer <= 0)
        {
            SpawnEnemy();
            timer = RespawnSpeed + Random.Range(0f,3f);
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    public void RespawnSpeedOverride(bool shouldOverride, float newSpeed)
    {
        if (shouldOverride)
        {
            RespawnSpeed = newSpeed;
        }
        else
        {
            RespawnSpeed = OldRespawnSpeed;
        }
    }
    public void RespawnSpeedOverride(bool shouldOverride)
    {
        RespawnSpeedOverride(shouldOverride);
    }
    void SpawnEnemy()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            return;
        }
        GameObject enemy = (GameObject)Instantiate(enemyPrefab, new Vector3(gameObject.transform.position.x + 4, 1.3f), Quaternion.identity);
        if (OldRespawnSpeed != RespawnSpeed)
        {
            enemy.GetComponent<EnemyMove>().speedModifier *= 2;
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
        //Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.GetComponent<Health>().dead)
            {
                return;//dead people cant capture bases
            }
            playerOwned = false;
            flag.GetComponent<SpriteRenderer>().color = Color.red;
            return;
        }
        GameObject[] go = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in go)
        {
            if (!enemy.GetComponent<Health>().dead && Mathf.Abs(enemy.transform.position.x - gameObject.transform.position.x) < 30)
            {
                //we need to clear the area
                return;
            }
        }
        //capture base
        flag.GetComponent<SpriteRenderer>().color = Color.green;
        upgradeBtn.gameObject.SetActive(true);
        playerOwned = true;
        gm.CounterAttack();
        foreach (GameObject wall in walls)
        {
            wall.GetComponent<BoxCollider2D>().isTrigger = true;
            wall.GetComponent<Health>().SetHealth();
        }
    }
}
