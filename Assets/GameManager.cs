using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    int coins;
    bool countering = false;
    float counterColorMod = 1f;
    public GameObject coinTxt;
    public GameObject counterAttackTxt;
    List<GameObject> Bases;
	// Use this for initialization
	void Start () {
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
	}

    public void AddCoins(int amount)
    {
        coins += amount;
        coinTxt.GetComponent<Text>().text = "Coins: " + coins;
    }


    public void CounterAttack()
    {
        countering = true;
        counterAttackTxt.SetActive(true);
        Color newColor = counterAttackTxt.GetComponent<Text>().color;
        newColor.a = 0;
        counterAttackTxt.GetComponent<Text>().color = newColor;
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject go in gos)
        {
            go.GetComponent<EnemyMove>().speedModifier *= 2;
        }
        foreach (GameObject go in Bases)
        {
            go.GetComponent<Base>().RespawnSpeedOverride(true, 1f);
        }
    }
}
