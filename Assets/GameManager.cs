using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    int coins;
    bool countering = false;
    float counterColorMod = 1f;
    float counterTimer;
    public GameObject coinTxt;
    public GameObject nightTxt;
    public GameObject counterAttackTxt;
    public GameObject basePrefab;
    float respawnSpeedOverride = 2f;
    List<GameObject> Bases;
    Color oldcolor;
    float dayDuration = 60f;
    float nightDuration = 60f;
    float dayNightTimer;
    bool night = false;
    Color oldBG;

    public bool Night
    {
        get
        {
            return night;
        }

        /*set
        {
            night = value;
        }*/
    }

    public bool Countering
    {
        get
        {
            return countering;
        }

        /*set
        {
            countering = value;
        }*/
    }

    // Use this for initialization
    void Start () {
        oldcolor = coinTxt.GetComponent<Text>().color;
        counterAttackTxt.SetActive(false);
        nightTxt.SetActive(false);
        Bases = new List<GameObject>();
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Base");
        foreach (GameObject go in gos)
        {
            Bases.Add(go);
        }
        dayNightTimer = dayDuration;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("main");//restarts level
        }
        if (countering)
        {
            Color newColor = counterAttackTxt.GetComponent<Text>().color;
            if (newColor.a >= 1)
            {
                counterColorMod = -1;
            }
            else if (newColor.a <= 0)
            {
                counterColorMod = 1;
            }
            newColor.a += (Time.deltaTime * .7f) * counterColorMod;
            //Debug.Log(newColor.ToString());
            counterAttackTxt.GetComponent<Text>().color = newColor;
        }
        if (counterTimer > 0)
        {
            counterTimer -= Time.deltaTime;
        }else
        {
            CounterAttack(false);
        }
        if (dayNightTimer <= 0)
        {
            night = !night;
            SetNight(night);
        }
        else
        {
            dayNightTimer -= Time.deltaTime;
            if (dayNightTimer <= 30f && !night)
            {
                nightTxt.SetActive(true);
                nightTxt.GetComponent<Text>().text = "Night arrives in: " + dayNightTimer.ToString("F2");
            }
        }
	}
    void SetNight(bool makeNight)
    {
        if (makeNight)
        {
            dayNightTimer = nightDuration;
            nightTxt.SetActive(false);
            oldBG = Camera.main.backgroundColor;
            Camera.main.backgroundColor = new Color(0,0,.1f);//dark blue to simulate night without being pitch black and ruining transparancy
        }
        else
        {
            Camera.main.backgroundColor = oldBG;
            dayNightTimer = dayDuration;
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyMove>().SetSprinting(makeNight);
        }
        foreach (GameObject go in Bases)
        {
            go.GetComponent<Base>().RespawnSpeedOverride(makeNight, respawnSpeedOverride);
        }
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateText();
    }
    private void UpdateText()
    {
        coinTxt.GetComponent<Text>().text = "Coins: " + coins;
    }
    public bool SpendMoney(int amount)
    {
        if (amount <= coins)
        {
            coins -= amount;
            UpdateText();
            return true;
        }
        else
        {
            StopCoroutine(TempChangeColor());
            StartCoroutine(TempChangeColor());
            return false;
        }
    }

    IEnumerator TempChangeColor()
    {
        //TODO Play buzzer noise here
        coinTxt.GetComponent<Text>().color = Color.red;
        yield return new WaitForSeconds(2f);
        coinTxt.GetComponent<Text>().color = oldcolor;
    }
    public void CounterAttack(bool b)
    {
        if (b)
        {
            counterTimer = 30f;
        }
        countering = b;
        counterAttackTxt.SetActive(b);
        Color newColor = counterAttackTxt.GetComponent<Text>().color;
        newColor.a = 0;
        counterAttackTxt.GetComponent<Text>().color = newColor;
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject go in gos)
        {
            go.GetComponent<EnemyMove>().SetSprinting(b);
        }
        foreach (GameObject go in Bases)
        {
            go.GetComponent<Base>().RespawnSpeedOverride(b, respawnSpeedOverride);
        }
    }
    public void CounterAttack(bool b, GameObject caller)
    {
        if (b)
        {
            if (caller.GetComponent<Base>().LeftBase == null)
            {
                Vector3 newpos = caller.transform.position;
                newpos.x -= 100;
                GameObject newbase = (GameObject)Instantiate(basePrefab, newpos, Quaternion.identity);
                newbase.GetComponent<Base>().RightBase = caller;
                Bases.Add(newbase);
                caller.GetComponent<Base>().LeftBase = newbase;
            }
            if (caller.GetComponent<Base>().RightBase == null)
            {
                Vector3 newpos = caller.transform.position;
                newpos.x += 100;
                GameObject newbase = (GameObject)Instantiate(basePrefab, newpos, Quaternion.identity);
                newbase.GetComponent<Base>().LeftBase = newbase;
                Bases.Add(newbase);
                caller.GetComponent<Base>().RightBase = newbase;
            }
        }
        CounterAttack(b);
    }
}
