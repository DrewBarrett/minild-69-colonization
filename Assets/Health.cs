using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
    protected float health;
    public float maxhealth;
    public AudioClip deathsound;
    public bool dead = false;
	// Use this for initialization
	void Start () {
        health = maxhealth;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
    virtual public void SetHealth()
    {
        //revive
        health = maxhealth;
        dead = false;
    }
    virtual public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Die();
        }
    }
    virtual protected void Die()
    {
        dead = true;
        GetComponent<AudioSource>().PlayOneShot(deathsound);
        GetComponent<BoxCollider2D>().isTrigger = true;
    }
}
