using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<TurretHealth>().upgradeLevel > 0)
        {
            //if turret is built...
            RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(transform.position.x, 1), transform.right);
        }
	}
}
