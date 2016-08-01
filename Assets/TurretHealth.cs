using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TurretHealth : Health {
    public bool built = false;
    int buildCost = 25;
    internal int upgradeLevel = 0;
    override protected void Start()
    {
        base.Start();
        if (!built)
        {
            Dead = true;
        }
        GetComponentInChildren<Text>(true).text = "Activate to build turret for 25 coins";
    }
    /*void Update()
    {
        if (upgradeLevel > 0)
        {
            //if we have been built

        }
    }*/
    public override void Build()
    {
        built = true;
        buildCost = 25;
        upgradeLevel++;
        if (upgradeLevel > 1)
        {
            GetComponent<Turret>().damage += 1;
            GetComponent<Turret>().range += 1;
            GetComponent<Turret>().shootCooldown -= .2f;
        }
        //gameObject.tag = "UnTagged";
        SetHealth();
        GetComponentInChildren<Canvas>(true).gameObject.SetActive(false);
        GetComponentInChildren<Text>(true).text = "Activate to upgrade turret for 25 coins (Infinite uses)";
    }
    public override int BuildCost()
    {
        return buildCost;
    }
    public override void SetHealth()
    {
        base.SetHealth();
        GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }
    internal void Destroy()
    {
        built = false;
        upgradeLevel = 0;
    }
}
