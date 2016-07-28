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
    public GameObject counterAttackTxt;
    public GameObject basePrefab;
    List<GameObject> Bases;
    Color oldcolor;
    // Use this for initialization
    void Start () {
        oldcolor = coinTxt.GetComponent<Text>().color;
        counterAttackTxt.SetActive(false);
        Bases = new List<GameObject>();
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Base");
        foreach (GameObject go in gos)
        {
            Bases.Add(go);
        }
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
            go.GetComponent<Base>().RespawnSpeedOverride(b, 1f);
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
