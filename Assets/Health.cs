using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
    protected float health;
    public float maxhealth;
    public AudioClip deathsound;
    private bool dead = false;

    public bool Dead
    {
        get
        {
            return dead;
        }

        set
        {
            dead = value;
        }
    }


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
        Dead = false;
    }
    virtual public void AddHealth(float amount)
    {
        health += amount;
        if (health > maxhealth)
        {
            health = maxhealth;
        }
    }
    virtual public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Die(true);
        }
    }
    virtual protected void Die(bool die)
    {
        Dead = die;
        if (die)
        {
            GetComponent<AudioSource>().PlayOneShot(deathsound);
        }
        
        //GetComponent<BoxCollider2D>().isTrigger = true;
    }
}
