using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    float seconds = 0;
    public GameObject scoreboard;
    public GameObject scoreHolder;
    public GameObject scoreText;
	// Use this for initialization
	void Start () {
        scoreboard.GetComponent<dreamloLeaderBoard>().LoadScores();

    }
	
	// Update is called once per frame
	void Update () {
        seconds += Time.deltaTime;
        if (scoreHolder.activeSelf)
        {
            //UpdateScoreboard();
        }
	}

    public void GameOver()
    {
        scoreboard.GetComponent<dreamloLeaderBoard>().AddScore(System.Environment.UserName, GetComponent<GameManager>().day, Mathf.FloorToInt(seconds), "v0");
        UpdateScoreboard();
    }
    void UpdateScoreboard()
    {
        var list = scoreboard.GetComponent<dreamloLeaderBoard>().ToListHighToLow();
        scoreHolder.SetActive(true);
        foreach (var item in list)
        {
            string s = item.playerName + " | " + item.score + " | " + item.seconds + " | " + item.shortText;
            GameObject text = (GameObject)Instantiate(scoreText, scoreHolder.transform);
            //text.transform.localScale = Vector3.one;
            text.GetComponent<Text>().text = s;
            //text.transform.SetParent(scoreHolder.transform);
        }
        GameObject t = (GameObject)Instantiate(scoreText, scoreHolder.transform);
        //text.transform.localScale = Vector3.one;
        t.GetComponent<Text>().text = "YOU: " + System.Environment.UserName + " | " + GetComponent<GameManager>().day + " | " + Mathf.FloorToInt(seconds);
    }
}
