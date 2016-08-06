using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {
    float seconds = 0;
    public GameObject scoreboard;
	// Use this for initialization
	void Start () {
        scoreboard.GetComponent<dreamloLeaderBoard>().LoadScores();

    }
	
	// Update is called once per frame
	void Update () {
        seconds += Time.deltaTime;
	}

    public void GameOver()
    {
        scoreboard.GetComponent<dreamloLeaderBoard>().AddScore(System.Environment.UserName, GetComponent<GameManager>().day, Mathf.FloorToInt(seconds), "v0");
        Debug.Log(scoreboard.GetComponent<dreamloLeaderBoard>().ToListHighToLow().ToString());
    }
}
